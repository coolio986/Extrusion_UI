using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
            SimulationModeActive = true;

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

            buildString = System.Text.Encoding.ASCII.GetString(buf);

            DiameterChanged?.Invoke(buildString, null);
        }

        private void RunSimulation()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    double diameter = GetRandomNumber(1.12, 1.98);
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

        public List<string> GetSerialPortList()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public bool IsSimulationModeActive()
        {
            return SimulationModeActive;
        }
    }
}
