using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.SerialCommunications
{
    public class SerialService : ISerialService
    {
        private SerialPort serialPort;

        public event EventHandler DiameterChanged;

        public SerialService()
        {
            serialPort = new SerialPort();

        }

        public void ConnectToSerialPort(string portName)
        {
            serialPort.PortName = portName;
            serialPort.Open();
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buf = new byte[serialPort.BytesToRead];
            serialPort.Read(buf, 0, buf.Length);

            string buildString = string.Empty;

            buildString = System.Text.Encoding.ASCII.GetString(buf);


            


            DiameterChanged?.Invoke(buildString, null);
        }

        public List<string> GetSerialPortList()
        {
            return SerialPort.GetPortNames().ToList();
        }
    }
}
