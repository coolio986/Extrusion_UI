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
        List<SerialPortClass> GetComPorts();

        void ConnectToSerialPort(string portName, int deviceId);
        void ConnectToSerialPort2(string portName, int deviceId);

        event EventHandler DiameterChanged;
        event EventHandler SpoolerDataChanged;
        event EventHandler TraverseDataChanged;
        event EventHandler GeneralDataChanged;
        event EventHandler SerialBufferSizeChanged;

        bool IsSimulationModeActive { get; set; }

        void SendSerialData(SerialCommand command);
        Task<SerialCommand> CheckIfDeviceExists(SerialPortClass serialport, string[] deviceTypes);
    }
}
