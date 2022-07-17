using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.SerialCommunications
{
    public interface ISerialService
    {
        List<SerialPortClass> GetSerialPortList();

        void ConnectToSerialPort(string portName);

        event EventHandler DiameterChanged;
        event EventHandler SpoolerDataChanged;
        event EventHandler TraverseDataChanged;
        event EventHandler GeneralDataChanged;
        event EventHandler SerialBufferSizeChanged;

        bool IsSimulationModeActive { get; set; }
        int SerialBufferSize { get; }

        void SendSerialData(SerialCommand command);
        Task<SerialCommand> CheckIfDeviceExists(string portName, string[] deviceTypes);
    }
}
