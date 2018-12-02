using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.SerialCommunications
{
    public interface ISerialService
    {
        List<SerialPortClass> GetSerialPortList();

        void ConnectToSerialPort(string portName);

        event EventHandler DiameterChanged;

        bool IsSimulationModeActive { get; set; }
        bool PortDataIsSet { get; }
    }
}
