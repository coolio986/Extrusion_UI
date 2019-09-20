using System;
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
using Digital_Indicator.Logic.Helpers;

namespace Digital_Indicator.Logic.SerialCommunications
{
    public class SerialService : ISerialService
    {
        private SerialPort serialPort;

        public event EventHandler DiameterChanged;
        public event EventHandler SpoolerDataChanged;
        public event EventHandler TraverseDataChanged;

        public SerialService()
        {
            if (!IsSimulationModeActive)
                serialPort = new SerialPort();
        }

        private string portName;
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                UnbindHandlers();
                SetPort();
                BindHandlers();
            }
        }

        public bool IsSimulationModeActive { get; set; }

        public bool PortDataIsSet { get; private set; }

        public void BindHandlers()
        {

        }
        public void UnbindHandlers()
        {

        }

        public void ConnectToSerialPort(string portName)
        {
            if (IsSimulationModeActive && (portName != null || portName == string.Empty))
            {
                PortName = portName;
                serialPort.Open();
                RunSimulation();
                QueryUpdates();
                return;
            }
            if (!IsSimulationModeActive)
            {
                PortName = portName;
                serialPort.Open();
            }
            else
            {
                RunSimulation();
            }

            StartSerialReceive();
            QueryUpdates();

        }

        private void SetPort()
        {
            serialPort.PortName = portName;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.BaudRate = 115200;
            PortDataIsSet = true;
        }

        private void StartSerialReceive()
        {
            Task.Factory.StartNew(() =>
            {
                if (PortDataIsSet)
                {
                    while (true)
                    {
                        try
                        {
                            string dataIn = serialPort.ReadLine();

                            //string functionName = GetFunctionName(dataIn);

                            //string asciiConvertedBytes = string.Empty;
                            //asciiConvertedBytes = dataIn.Replace("\r", "").Replace("\n", "");
                            string[] splitData = dataIn.Replace("\r", "").Replace("\n", "").Split(';');
                            if (splitData.Length >= 2)
                            {
                                Type type = this.GetType();
                                MethodInfo method = type.GetMethod(splitData[1]);
                                method.Invoke(this, new object[] { splitData });
                            }


                        }
                        catch (Exception oe)
                        {
                            Console.WriteLine("Serial Error: " + oe.Message);
                        }

                        Thread.Sleep(1);
                    }
                }
            });
        }

        private void QueryUpdates()
        {
            Task.Factory.StartNew(() =>
            {
                if (PortDataIsSet)
                {

                    //TESTING UNCOMMENT WHEN DONE
                    //while (true)
                    //{
                    //    SerialCommand command = new SerialCommand()
                    //    {
                    //        DeviceID = ((int)ConnectedDeviceTypes.SPOOLER).ToString() + ";",
                    //        Command = "getrpm;",
                    //        Value = null,
                    //    };
                    //    SendSerialData(command);
                    //    Thread.Sleep(1000);
                    //}
                }
            });

        }

        private void RunSimulation()
        {
            Task.Factory.StartNew(() =>
            {
                if (PortDataIsSet)
                {
                    serialPort.WriteLine("1;IsInSimulationMode = true");
                }
                else
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

        string GetFunctionName(string serialString)
        {

            try
            {
                if (serialString != string.Empty)
                {
                    string[] stringArray = serialString.ToString().Replace("\r", "").Split(';');

                    if (stringArray.Length >= 1)
                    {
                        return stringArray[1];
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error: " + serialString);
            }
            return string.Empty;
        }

        public void SendSerialData(SerialCommand command)
        {
            string serialCommand = command.AssembleCommand();
            serialPort.WriteLine(serialCommand);
        }
        
        public void Diameter(string[] splitData) //reflection calls this
        {
            double diameter = 0;

            try //if anything fails, skip it and wait for the next serial event
            {

                if (Double.TryParse(splitData[2], out diameter)) //if it can convert to double, do it
                {
                    string formatString = "0.";
                    splitData[2] = splitData[2].Replace("\0", string.Empty); //remove nulls
                    for (int i = 0; i < splitData[2].Split('.')[1].ToString().Length; i++) //format the string for number of decimal places
                    {
                        formatString += "0";
                    }

                    DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                }
            }
            catch (Exception oe)
            {

            }
        }

        public void spoolRpm(string[] splitData) //reflection calls this
        {
            SerialCommand command = new SerialCommand();

            if (splitData.Length == 3)
            {
                command.DeviceID = splitData[0];
                command.Command = splitData[1];
                command.Value = splitData[2].Replace("\0", string.Empty); //remove nulls

                TraverseDataChanged?.Invoke(command.Value, null);    

            }
        }
    }
}
