using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
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
            serialPort.DataReceived += SerialPort_DataReceived;
        }
        public void UnbindHandlers()
        {
            serialPort.DataReceived -= SerialPort_DataReceived;
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

            QueryUpdates();
        }

        private void SetPort()
        {
            serialPort.PortName = portName;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            PortDataIsSet = true;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            string dataIn = serialPort.ReadLine();
            ConnectedDeviceTypes deviceType = GetDeviceType(dataIn);

            if (deviceType == ConnectedDeviceTypes.INDICATOR) { ProcessIndicatorData(dataIn); };
            if (deviceType == ConnectedDeviceTypes.SPOOLER) { ProcessSpoolerData(dataIn); };
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

        private void QueryUpdates()
        {
            Task.Factory.StartNew(() =>
            {
                if (PortDataIsSet)
                {
                    while (true)
                    {
                        SerialCommand command = new SerialCommand()
                        {
                            DeviceID = "2;",
                            Command = "getrpm;",
                            Value = null,
                        };
                        SendSerialData(command);
                        Thread.Sleep(1000);
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

        ConnectedDeviceTypes GetDeviceType(string serialString)
        {
            if (serialString != string.Empty)
            {
                string deviceID = serialString.Substring(0, serialString.IndexOf(";"));
                return (ConnectedDeviceTypes)Convert.ChangeType(deviceID, typeof(int));
            }
            return 0;
        }

        public void SendSerialData(SerialCommand command)
        {
            string serialCommand = command.AssembleCommand();
            serialPort.WriteLine(serialCommand);
        }
        public void ProcessIndicatorData(string data)
        {
            string asciiConvertedBytes = string.Empty;

            asciiConvertedBytes = data.Replace("3;", "").Replace("\r", "").Replace("\n", "");

            if (asciiConvertedBytes.Length == 52) //if data is valid
            {
                byte[] bytes = new byte[asciiConvertedBytes.Length / 4];

                for (int i = 0; i < 13; ++i) //split each 4 bit nibble into array
                {
                    bytes[i] = Convert.ToByte(asciiConvertedBytes.Substring(4 * i, 4).Reverse().ToString(), 2);
                }

                string diameterStringBuilder = string.Empty;
                for (int i = 5; i <= 10; i++) //diamter resides in array positions 5 to 10
                {
                    diameterStringBuilder += bytes[i].ToString();
                }

                try //if anything fails, skip it and wait for the next serial event
                {
                    //bytes[11] is the decmial position from right
                    diameterStringBuilder = diameterStringBuilder.Insert(diameterStringBuilder.Length - bytes[11], ".");

                    double diameter = 0;

                    if (Double.TryParse(diameterStringBuilder, out diameter)) //if it can convert to double, do it
                    {
                        string formatString = "0.";
                        for (int i = 0; i < bytes[11]; i++) //format the string for number of decimal places
                        {
                            formatString += "0";
                        }

                        DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                    }

                }
                catch { }
            }
        }
        public void ProcessSpoolerData(string data)
        {
            SpoolerDataChanged?.Invoke(data, null);
        }
    }
}
