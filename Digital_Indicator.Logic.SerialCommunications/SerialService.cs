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
        private double? previousDiameter;
        private long previousMillis;
        private Stopwatch stopWatch;

        public SerialService()
        {
            if (!IsSimulationModeActive)
                serialPort = new SerialPort();

            stopWatch = new Stopwatch();
            stopWatch.Start();
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
            if (!IsSimulationModeActive)
            {
                PortName = portName;
                serialPort.Open();
            }
            else
            {
                RunSimulation();
            }
        }

        private void SetPort()
        {
            serialPort.PortName = portName;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buf = new byte[serialPort.BytesToRead];
            serialPort.Read(buf, 0, buf.Length);

            bool dataValid = false;

            if (buf.Length == 54) //buffer must be exactly 54 in length 
            {
                for (int i = 0; i <= 15; i++)
                {
                    if (buf[i] != 49)
                    {
                        dataValid = false; //if first 15 array indexes are not 49 (1111), then data not valid
                        break;
                    }
                    else
                        dataValid = true;
                }
            }

            if (dataValid)
            {
                string asciiConvertedBytes = string.Empty;
                asciiConvertedBytes = System.Text.Encoding.ASCII.GetString(buf).Replace("\r", "").Replace("\n", "");

                byte[] bytes = new byte[asciiConvertedBytes.Length / 4];

                for (int i = 0; i < 13; ++i)
                {
                    bytes[i] = Convert.ToByte(asciiConvertedBytes.Substring(4 * i, 4).Reverse().ToString(), 2);
                }


                string diameterStringBuilder = string.Empty;
                for (int i = 5; i <= 10; i++)
                {
                    diameterStringBuilder += bytes[i].ToString();
                }

                try
                {
                    //bytes[11] is the decmial position from right
                    diameterStringBuilder = diameterStringBuilder.Insert(diameterStringBuilder.Length - bytes[11], ".");

                    double diameter = 0;

                    if (Double.TryParse(diameterStringBuilder, out diameter))
                    {
                        Convert.ToDouble(diameterStringBuilder);

                        if (previousDiameter == null)
                            previousDiameter = diameter;

                        previousDiameter = diameter;

                        string formatString = "0.";
                        for (int i = 0; i < bytes[11]; i++)
                        {
                            formatString += "0";
                        }

                        DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                    }
                    else
                    {

                    }
                }
                catch { }
            }
            previousMillis = stopWatch.ElapsedMilliseconds;
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


                    DiameterChanged?.Invoke(diameter.ToString(formatString), null);
                    Thread.Sleep(1);
                }
            });
        }

        private double GetRandomNumber(double minimum, double maximum, int decimalPlaces)
        {
            //Random random = new Random();
            //return random.NextDouble() * (maximum - minimum) + minimum;
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
    }
}
