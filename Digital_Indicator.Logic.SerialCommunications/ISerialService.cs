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
        event EventHandler SpoolerDataChanged;
        event EventHandler TraverseDataChanged;
        event EventHandler GeneralDataChanged;

        bool IsSimulationModeActive { get; set; }
        bool PortDataIsSet { get; }

        void SendSerialData(SerialCommand command);
    }
}
