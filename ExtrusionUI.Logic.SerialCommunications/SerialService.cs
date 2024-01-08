using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ExtrusionUI.Logic.FileOperations;
using ExtrusionUI.Logic.Helpers;

namespace ExtrusionUI.Logic.SerialCommunications
{
    public class SerialService : ISerialService
    {
        //private SerialPort serialPort;

        public event EventHandler X_DiameterChanged;
        public event EventHandler Y_DiameterChanged;

        public event EventHandler SpoolerDataChanged;
        public event EventHandler TraverseDataChanged;
        public event EventHandler GeneralDataChanged;
        public event EventHandler SerialBufferSizeChanged;
        bool autoDetectionCheckFinished = false;

        private IFileService _fileService;
        SerialPort serialPort = null; //make serialport global to help minimize garbage collection dispose


        public SerialService(IFileService fileService)
        {
            _fileService = fileService;

            //if (!IsSimulationModeActive)
            //    serialPort = new SerialPort();

            //serialQueue = new ConcurrentQueue<SerialCommand>();
        }



        private int serialBufferSize = 0;
        public int SerialBufferSize
        {
            get => serialBufferSize;
            set
            {
                if (serialBufferSize != value)
                {
                    serialBufferSize = value;
                    SerialBufferSizeChanged?.Invoke(serialBufferSize, null);
                }
            }
        }


        public bool IsSimulationModeActive { get; set; }

        //public bool PortDataIsSet { get; private set; }

        public void BindHandlers()
        {
            //serialPort.DataReceived += SerialPort_DataReceived;
        }
        public void UnbindHandlers()
        {
            //serialPort.DataReceived -= SerialPort_DataReceived;
        }

        public void ConnectToSerialPort(string portName, int deviceId)
        {
            if (!IsSimulationModeActive)
            {
                //PortName = portName;
                //UnbindHandlers();
                serialPort = SetPort(portName);
                //BindHandlers();
                ConcurrentQueue<SerialCommand> serialQueue = new ConcurrentQueue<SerialCommand>();
                int CCqueueCount = 0;
                ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
                StartSerialQueue(serialPort, deviceId, serialQueue, CCqueueCount, lock_);
                //serialPort.BaudRate = 115200;
                serialPort.Open();
            }
            else
            {
                RunSimulation();
            }

            StartSerialReceive(serialPort, null, retryOpen: true);
        }

        private SerialPort SetPort(string portName)
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = 230400;
            serialPort.Handshake = Handshake.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;

            serialPort.DtrEnable = true;
            serialPort.RtsEnable = false; //setting to true causes the esp32 to reset as per the firmware flash routine. don't use
            serialPort.DiscardNull = true;
            serialPort.ReadTimeout = 1;
            serialPort.WriteTimeout = 1;
            serialPort.NewLine = Environment.NewLine;
            //serialPort.ReceivedBytesThreshold = 2048;
            //serialPort.ReadBufferSize = 8092;
            serialPort.ErrorReceived += SerialPort_ErrorReceived;
            //PortDataIsSet = true;
            return serialPort;
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            SerialError serialError = e.EventType;
            _fileService.AppendLog(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + " Serial in-> " + e.EventType + " Error");
            //throw new Exception("serial port error");
        }

        public async Task<SerialCommand> CheckIfDeviceExists(string portName, string[] deviceTypes)
        {
            serialPort = SetPort(portName);
            serialPort.Open();

            SerialCommand returnCommand = new SerialCommand();
            StartSerialReceive(serialPort, returnCommand, ReturnIDFunction);

            SerialCommand command = new SerialCommand() { Command = "GetHardwareID", DeviceID = "100" };
            string serialCommand = command.AssembleCommand();
            int checksum = GetCheckSum(serialCommand);
            serialCommand += checksum.ToString() + ";";


            serialPort.WriteLine(serialCommand);
            while (serialPort.BytesToWrite > 0)
            {
                Console.WriteLine("writing bytes");
                Thread.Sleep(10);
            }

            var myTask = Task.Run(() =>
            {
                int counterInMillis = 0;
                int timeDelay = 500;
                while (autoDetectionCheckFinished == false)
                {
                    if (counterInMillis >= timeDelay)
                        break;

                    Thread.Sleep(1);
                    counterInMillis++;
                }
                serialPort.Close();
                autoDetectionCheckFinished = false;
                return returnCommand;

            });
            return await myTask.ConfigureAwait(false);

        }
        private object ReturnIDFunction(object sender, SerialPort serialPort, SerialCommand returnCommand)
        {
            string[] returnData = sender as string[];

            if (returnData[1] != "HardwareID")
                return null;

            returnCommand.Command = returnData[1];
            returnCommand.DeviceID = returnData[0];
            returnCommand.Value = returnData[2];

            autoDetectionCheckFinished = true;
            //serialPort.Close();
            return sender;

        }
        private void StartSerialReceive(SerialPort serialPort, SerialCommand returnCommand, Func<object, SerialPort, SerialCommand, object> func = null, bool retryOpen = false)
        {
            if (serialPort.IsOpen)
            {
                byte[] buffer = new byte[65536];
                string ret = string.Empty;
                Action kickoffRead = null;
                LineSplitter lineSplitter = new LineSplitter();
                try
                {
                    kickoffRead = (Action)(() => serialPort.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
                    {
                        if (serialPort.IsOpen)
                        {
                            int count = serialPort.BaseStream.EndRead(ar);
                            byte[] dst = lineSplitter.OnIncomingBinaryBlock(this, buffer, count);
                            OnDataReceived(dst, returnCommand, serialPort, func);

                            if (serialPort.IsOpen)
                                kickoffRead();

                        }
                        if (!serialPort.IsOpen)
                        {
                            Console.WriteLine("Serial port closed");
                            _fileService.AppendLog(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + " Serial port closed");
                            _fileService.AppendLog(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + $" Bytes left in buffer: {lineSplitter.leftover.Length}");
                            if (retryOpen)
                            {
                                _fileService.AppendLog(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + " reopening");

                                string comPortName = serialPort.PortName;
                                serialPort.Close();
                                serialPort.Dispose();
                                GC.Collect();
                                SetPort(comPortName);

                                serialPort.Open();

                            }
                        }
                    }, null));

                    kickoffRead();
                }
                catch
                {

                }
            }
        }

        public virtual void OnDataReceived(byte[] data, SerialCommand returnCommand, SerialPort serialPort = null, Func<object, SerialPort, SerialCommand, object> func = null)
        {
            string dataIn = string.Empty;

            if (null != data)
            {
                dataIn = Encoding.ASCII.GetString(data).TrimEnd('\r', '\n');

                string[] splitData = dataIn.Replace("\r", "").Replace("\n", "").Replace("\0", "").Split(';');
                if (splitData.Length == 5)
                {
                    bool checkSumError = !ChecksumPassed(splitData[3], dataIn);
                    if (checkSumError)
                    {
                        //Console.WriteLine("Checksum Error");
                        _fileService.AppendLog(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + " Serial in-> " + dataIn + " Checksum Error");

                    }

                    if (!checkSumError)
                    {
                        if (func == null)
                        {
                            switch (splitData[1]) //switch is faster
                            {
                                case "d1":
                                    d1(splitData);
                                    return;
                                case "d2":
                                    d2(splitData);
                                    return;
                                default:
                                    break;
                            }

                            Type type = this.GetType();
                            MethodInfo method = type.GetMethod(splitData[1]);

                            if (method == null) //try indirect GetMethod
                            {
                                //Console.WriteLine(splitData[1] + " Trying indirect method, no function defined");
                                method = type.GetMethod("ProcessIndirectFunction");
                            }
                            if (method == null) { throw new Exception(); }
                            method.Invoke(this, new object[] { splitData });
                        }
                        else
                        {
                            func(splitData, serialPort, returnCommand);
                        }
                    }
                }
            }
        }

        private class LineSplitter
        {
            // based on: http://www.sparxeng.com/blog/software/reading-lines-serial-port
            public byte Delimiter = (byte)'\n';
            public byte[] leftover;


            public byte[] OnIncomingBinaryBlock(object sender, byte[] buffer, int bytesInBuffer)
            {
                leftover = ConcatArray(leftover, buffer, 0, bytesInBuffer);

                if (sender is SerialService serialService)
                    serialService.SerialBufferSize = leftover.Length;


                int newLineIndex = Array.IndexOf(leftover, Delimiter);
                if (newLineIndex >= 0)
                {
                    byte[] result = new byte[newLineIndex + 1];
                    Array.Copy(leftover, result, result.Length);
                    byte[] newLeftover = new byte[leftover.Length - result.Length];
                    System.Buffer.BlockCopy(leftover, newLineIndex + 1, newLeftover, 0, newLeftover.Length);
                    //Array.Copy(leftover, newLineIndex + 1, newLeftover, 0, newLeftover.Length);
                    leftover = newLeftover;

                    return result;
                }
                return null;
            }

            static byte[] ConcatArray(byte[] head, byte[] tail, int tailOffset, int tailCount)
            {
                byte[] result;

                if (head == null)
                {
                    result = new byte[tailCount];
                    Array.Copy(tail, tailOffset, result, 0, tailCount);
                }
                else
                {
                    result = new byte[head.Length + tailCount];
                    //int size = sizeof(byte);
                    //int length = head.Length * size;
                    System.Buffer.BlockCopy(head, 0, result, 0, head.Length);


                    //head.CopyTo(result, 0);
                    Array.Copy(tail, tailOffset, result, head.Length, tailCount);
                }

                return result;
            }
        }

        private void RunSimulation()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    double diameter = GetRandomNumber(1.7000, 1.8000, 4);

                    string formatString = "0.";
                    for (int i = 0; i < 4; i++)
                    {
                        formatString += "0";
                    }

                    X_DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                    Thread.Sleep(50);
                }
            });
        }

        private double GetRandomNumber(double minimum, double maximum, int decimalPlaces)
        {
            int dPlaces = (int)Math.Pow(10, decimalPlaces);

            Random random = new Random();
            int r = random.Next((int)(minimum * dPlaces), (int)(maximum * dPlaces)); //+1 as end is excluded.
            return (Double)r / dPlaces;
        }

        public void SendSerialData(SerialCommand command)
        {
            SerialQueueClass.Enqueue(command);
            //serialQueue.Enqueue(command);
            //Interlocked.Increment(ref CCqueueCount);

        }

        private void StartSerialQueue(SerialPort serialPort, int deviceId, ConcurrentQueue<SerialCommand> serialQueue, int CCqueueCount, ReaderWriterLockSlim lock_)
        {
            Task.Factory.StartNew(() =>
            {
                SerialQueueClass.AddSerialPortToQueue(serialPort, deviceId, serialQueue, CCqueueCount, lock_);

                while (true)
                {
                    SerialCommand command;

                    if (serialPort.IsOpen)
                    {

                        while (serialQueue.TryDequeue(out command))
                        {

                            //Interlocked.Decrement(ref CCqueueCount);
                            SerialQueueClass.DecrementQueueLock(command);
                            string serialCommand = command.AssembleCommand();

                            int checksum = GetCheckSum(serialCommand);


                            serialCommand += checksum.ToString() + ";";
                            //Console.WriteLine(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + " Serial out-> " + serialCommand);

                            lock_.EnterWriteLock();
                            try
                            {
                                _fileService.AppendLog(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString() + " Serial out-> " + serialCommand);
                            }
                            catch (Exception oe)
                            {
                                Console.WriteLine("serial error " + oe.Message);
                            }
                            finally
                            {
                                lock_.ExitWriteLock();
                            }

                            serialPort.WriteLine(serialCommand);
                            while (serialPort.BytesToWrite > 0)
                            {
                                Console.WriteLine("writing bytes");
                                Thread.Sleep(15);
                            }
                        }


                    }
                    Thread.Sleep(10);
                }
            });

        }

        private bool ChecksumPassed(string checksum, string stringToCheck)
        {
            int tokenCount = 0;
            int tokenPosition = 0;
            string checksumString = string.Empty;
            tokenCount = stringToCheck.Split(';').Length;

            if (tokenCount == 5)
            {

                int tCount = 0;

                for (int i = 0; i < stringToCheck.Length; ++i)
                {
                    if (tCount != 3)
                    {
                        checksumString += stringToCheck[i];
                    }
                    else
                    {
                        if (stringToCheck[i] == ';')
                        {
                            tCount++;
                        }
                    }
                    if (stringToCheck[i] == ';' && tCount < 3)
                    {
                        tokenPosition = i;
                        tCount++;
                    }


                }
            }

            checksumString = checksumString.Replace("\r", "").Replace("\0", "");

            int checksumValue = GetCheckSum(checksumString);

            Int16 sum = -1;
            Int16.TryParse(checksum, out sum);

            return checksumValue == sum;
        }

        private int GetCheckSum(string checksumString)
        {
            int checksumValue = 0;
            for (int i = 0; i < checksumString.Length; ++i)
            {
                checksumValue = checksumValue ^ checksumString[i];
            }

            return checksumValue;
        }

        public List<SerialPortClass> GetSerialPortList()
        {
            List<SerialPortClass> ListOfSerialPortClass = new List<SerialPortClass>();

            ManagementObjectCollection managementObjectCollection = null;
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\cimv2",
            "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

            managementObjectCollection = managementObjectSearcher.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                string friendlyName = managementObject["Name"].ToString();

                Match match = new Regex(@"(?!\()COM([0-9]|[0-9][0-9])(?=\))").Match(friendlyName);

                if (match.Success)
                    ListOfSerialPortClass.Add(new SerialPortClass()
                    {
                        SerialPort_FriendlyName = friendlyName,
                        SerialPort_PortName = match.Value,
                    });
            }

            return ListOfSerialPortClass;
        }

        public void diameter(string[] splitData) //reflection calls this
        {
            double diameter = 0;

            try //if anything fails, skip it and wait for the next serial event
            {

                if (Double.TryParse(splitData[2], out diameter)) //if it can convert to double, do it
                {
                    diameter = diameter / 10000;
                    string formatString = "N3";

                    X_DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                }
            }
            catch (Exception oe)
            {

            }
        }

        public void d1(string[] splitData) //reflection calls this
        {
            double diameter = 0;

            try //if anything fails, skip it and wait for the next serial event
            {

                if (Double.TryParse(splitData[2], out diameter)) //if it can convert to double, do it
                {
                    diameter = diameter / 10000;
                    string formatString = "N3";

                    X_DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                }
            }
            catch (Exception oe)
            {

            }
        }

        public void d2(string[] splitData) //reflection calls this
        {
            double diameter = 0;

            try //if anything fails, skip it and wait for the next serial event
            {

                if (Double.TryParse(splitData[2], out diameter)) //if it can convert to double, do it
                {
                    diameter = diameter / 10000;
                    string formatString = "N3";

                    Y_DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                }
            }
            catch (Exception oe)
            {

            }
        }

        public void TraverseMotionStatus(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void SpoolMotionStatus(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
            Debug.WriteLine("Spool Motion Status: {0};{1};{2};{3}", splitData[0], splitData[1], splitData[2], splitData[3]);
        }
        public void FilamentNominalDiameter(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void FilamentUpperLimit(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void FilamentLowerLimit(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void TraverseHomeOffset(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void SpoolWidth(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void TraverseRPM(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void TraverseLeadRPM(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void TraverseLeadWidth(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void TraverseStartPosition(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void BufferStatus(string[] splitData) //reflection calls this
        {
            SerialCommand serialCommand = processSerialCommand(splitData);

            //need to sent to spooler
            serialCommand.DeviceID = ((int)HARDWARETYPES.Spooler).ToString();
            serialCommand.Command = "From" + serialCommand.Command;
            sendToDevice(serialCommand);
        }
        public void BufferPosition(string[] splitData) //reflection calls this
        {
            SerialCommand serialCommand = processSerialCommand(splitData);

            //need to sent to spooler
            serialCommand.DeviceID = ((int)HARDWARETYPES.Spooler).ToString();
            serialCommand.Command = "From" + serialCommand.Command;
            sendToDevice(serialCommand);
        }

        public void LoggerMotionState(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
            Debug.WriteLine("Logger Motion State: {0};{1};{2};{3}", splitData[0], splitData[1], splitData[2], splitData[3]);
        }

        public void TraversePositionStatus(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }

        private SerialCommand processSerialCommand(string[] splitData)
        {
            SerialCommand command = new SerialCommand();

            if (splitData.Length >= 3)
            {
                command.DeviceID = splitData[0];
                command.Command = splitData[1];
                command.Value = splitData[2].Replace("\0", string.Empty).Replace("nan", "-1.00"); //remove nulls

                GeneralDataChanged?.Invoke(command, null);
                return command;
            }
            return null;
        }

        private void sendToDevice(SerialCommand serialCommand)
        {
            SendSerialData(serialCommand);
        }
    }
}
