using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.SerialCommunications
{
    public class SerialService : ISerialService
    {
        private SerialPort serialPort;

        public event EventHandler DiameterChanged;

        public SerialService()
        {
            SimulationModeActive = false;

            if (!SimulationModeActive)
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

        private bool SimulationModeActive { get; set; }

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
            if (!SimulationModeActive)
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

            string buildString = string.Empty;

            buildString = System.Text.Encoding.ASCII.GetString(buf).TrimEnd('\r', '\n').Insert(4, ".");

            try
            {
                double diameter = Convert.ToDouble(buildString);
                DiameterChanged?.Invoke(diameter.ToString("0.00"), null);

            }
            catch
            {

            }

            
        }

        private void RunSimulation()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    double diameter = GetRandomNumber(1.65, 1.85);
                    DiameterChanged?.Invoke(diameter.ToString("0.00"), null);
                    Thread.Sleep(50);
                }
            });
        }

        private double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
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

        public bool IsSimulationModeActive()
        {
            return SimulationModeActive;
        }
    }
}
