using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.SerialCommunications
{
    public class SerialPortClass
    {
        public string SerialPort_FriendlyName { get; set; }
        public string SerialPort_PortName { get; set; }

        public HARDWARETYPES MyHardwareType { get; set; }

        private readonly SerialPort SerialPort;

        public int BufferSize { get; set; }

        public SerialPort GetSerialPort() => SerialPort;


        public SerialPortClass(SerialPort serialPort)
        {
            SerialPort = serialPort;
        }

        public SerialPortClass()
        {

        }
    }
}
