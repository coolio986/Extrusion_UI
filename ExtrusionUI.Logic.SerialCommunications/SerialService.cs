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

        public event EventHandler DiameterChanged;

        public event EventHandler SpoolerDataChanged;
        public event EventHandler TraverseDataChanged;
        public event EventHandler GeneralDataChanged;
        public event EventHandler SerialBufferSizeChanged;
        bool autoDetectionCheckFinished = false;

        private List<SerialPortClass> SerialPortList { get; set; }

        private IFileService _fileService;

        //ConcurrentQueue<SerialCommand> serialQueue;
        //int CCqueueCount = 0;

        //private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();

        public SerialService(IFileService fileService)
        {
            _fileService = fileService;

            //if (!IsSimulationModeActive)
            //    serialPort = new SerialPort();

            //serialQueue = new ConcurrentQueue<SerialCommand>();
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
            SerialPort serialPort = null;
            if (!IsSimulationModeActive)
            {
                //PortName = portName;
                //UnbindHandlers();
                serialPort = SetPort(portName);
                SerialPortClass serialPortClass = new SerialPortClass(serialPort);

                if (SerialPortList.Any(x => x.SerialPort_PortName == serialPort.PortName))
                {
                    var existingSerialPortClass = SerialPortList.FirstOrDefault(x => x.SerialPort_PortName == serialPort.PortName);
                    if (existingSerialPortClass != null && existingSerialPortClass.GetSerialPort() == null)
                    {
                        serialPortClass.MyHardwareType = existingSerialPortClass.MyHardwareType;
                        serialPortClass.SerialPort_FriendlyName = existingSerialPortClass.SerialPort_FriendlyName;
                        serialPortClass.SerialPort_PortName = existingSerialPortClass.SerialPort_PortName;

                        SerialPortList.Remove(existingSerialPortClass);
                        SerialPortList.Add(serialPortClass);
                    }
                }
                ConcurrentQueue<SerialCommand> serialQueue = new ConcurrentQueue<SerialCommand>();
                int CCqueueCount = 0;
                ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
                StartSerialQueue(serialPort, deviceId, serialQueue, CCqueueCount, lock_);
                //serialPort.BaudRate = 115200;
                serialPort.Open();

                StartSerialReceive(serialPortClass, null);
            }
            else
            {
                RunSimulation();
            }
        }

        private SerialPort SetPort(string portName)
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = portName;
            //serialPort.DtrEnable = true;
            //serialPort.RtsEnable = true;
            serialPort.BaudRate = 115200;
            serialPort.Handshake = Handshake.None;
            serialPort.DtrEnable = true;
            serialPort.ReadTimeout = 100;
            serialPort.WriteTimeout = 20;
            serialPort.NewLine = Environment.NewLine;
            serialPort.ReceivedBytesThreshold = 2048;
            //PortDataIsSet = true;
            return serialPort;
        }

        public async Task<SerialCommand> CheckIfDeviceExists(SerialPortClass serialPortClass, string[] deviceTypes)
        {
            SerialPort serialPort = SetPort(serialPortClass.SerialPort_PortName);
            serialPort.Open();
            serialPortClass = new SerialPortClass(serialPort)
            {
                MyHardwareType = serialPortClass.MyHardwareType,
                SerialPort_FriendlyName = serialPortClass.SerialPort_FriendlyName,
                SerialPort_PortName = serialPortClass.SerialPort_PortName,
            };


            SerialCommand returnCommand = new SerialCommand();
            StartSerialReceive(serialPortClass, returnCommand, ReturnIDFunction);

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
        private void StartSerialReceive(SerialPortClass serialPortClass, SerialCommand returnCommand, Func<object, SerialPort, SerialCommand, object> func = null)
        {
            SerialPort serialPort = serialPortClass.GetSerialPort();
            if (serialPort.IsOpen)
            {
                byte[] buffer = new byte[5000];
                string ret = string.Empty;
                Action kickoffRead = null;
                LineSplitter lineSplitter = new LineSplitter();
                
                kickoffRead = (Action)(() => serialPort.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
                {
                    if (serialPort.IsOpen)
                    {
                        int count = serialPort.BaseStream.EndRead(ar);
                        byte[] dst = lineSplitter.OnIncomingBinaryBlock(
                            this,
                            serialPortClass,
                            buffer,
                            count);
                        OnDataReceived(dst, returnCommand, serialPort, func);

                        if (serialPort.IsOpen)
                            kickoffRead();
                    }
                }, null)); kickoffRead();
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
            byte[] leftover;


            public byte[] OnIncomingBinaryBlock(object sender, SerialPortClass serialPort, byte[] buffer, int bytesInBuffer)
            {
                leftover = ConcatArray(leftover, buffer, 0, bytesInBuffer);
                serialPort.BufferSize = leftover.Length;
                if (sender is SerialService serialService)
                    serialService.SerialBufferSizeChanged?.Invoke(serialPort, null);

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

        private void Timer_Elapsed(object serialCounter, ElapsedEventArgs e)
        {


        }

        //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    string dataIn = serialPort.ReadLine();

        //    string asciiConvertedBytes = string.Empty;

        //    asciiConvertedBytes = dataIn.Replace("\r", "").Replace("\n", "");

        //    if (asciiConvertedBytes.Length == 52) //if data is valid
        //    {
        //        byte[] bytes = new byte[asciiConvertedBytes.Length / 4];

        //        for (int i = 0; i < 13; ++i) //split each 4 bit nibble into array
        //        {
        //            bytes[i] = Convert.ToByte(asciiConvertedBytes.Substring(4 * i, 4).Reverse().ToString(), 2);
        //        }

        //        string diameterStringBuilder = string.Empty;
        //        for (int i = 5; i <= 10; i++) //diamter resides in array positions 5 to 10
        //        {
        //            diameterStringBuilder += bytes[i].ToString();
        //        }

        //        try //if anything fails, skip it and wait for the next serial event
        //        {
        //            //bytes[11] is the decmial position from right
        //            diameterStringBuilder = diameterStringBuilder.Insert(diameterStringBuilder.Length - bytes[11], ".");

        //            double diameter = 0;

        //            if (Double.TryParse(diameterStringBuilder, out diameter)) //if it can convert to double, do it
        //            {
        //                string formatString = "0.";
        //                for (int i = 0; i < bytes[11]; i++) //format the string for number of decimal places
        //                {
        //                    formatString += "0";
        //                }

        //                DiameterChanged?.Invoke(diameter.ToString(formatString), null);
        //            }

        //        }
        //        catch { }
        //    }
        //}

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

                    DiameterChanged?.Invoke(diameter.ToString(formatString), null);
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
                                if (!IsInterMCUCommunication(command))
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
                                //Thread.Sleep(15);
                            }
                            Thread.Sleep(10);
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

            SerialPortList = ListOfSerialPortClass;
            return ListOfSerialPortClass;
        }

        public List<SerialPortClass> GetComPorts()
        => SerialPortList;

        private bool IsInterMCUCommunication(SerialCommand serialCommand)
        {
            if (serialCommand.DeviceID == Convert.ToInt32(HARDWARETYPES.Spooler).ToString())
            {
                return serialCommand.Command == "FromBufferPosition" || serialCommand.Command == "FromBufferStatus";
            }

            return false;
        }
        public void diameter(string[] splitData) //reflection calls this
        {
            double diameter = 0;

            try //if anything fails, skip it and wait for the next serial event
            {

                if (Double.TryParse(splitData[2], out diameter)) //if it can convert to double, do it
                {
                    //string formatString = "0.";
                    //splitData[2] = splitData[2].Replace("\0", string.Empty); //remove nulls

                    diameter = diameter / 10000;
                    string formatString = "N3";

                    //for (int i = 0; i < splitData[2].Split('.')[1].ToString().Length; i++) //format the string for number of decimal places
                    //{
                    //    formatString += "0";
                    //}

                    DiameterChanged?.Invoke(diameter.ToString(formatString), null);
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

        public void BufPos1RPM(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void BufPos2RPM(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void BufPos3RPM(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }
        public void BufPos4RPM(string[] splitData) //reflection calls this
        {
            processSerialCommand(splitData);
        }

        public void SpoolWindUp(string[] splitData)
        {
            processSerialCommand(splitData);
        }

        public void SpoolWindDown(string[] splitData)
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
