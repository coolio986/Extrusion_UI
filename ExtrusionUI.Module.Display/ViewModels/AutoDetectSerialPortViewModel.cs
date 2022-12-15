using ExtrusionUI.Logic.Navigation;
using ExtrusionUI.Logic.SerialCommunications;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Module.Display.ViewModels
{
    public class AutoDetectSerialPortViewModel : BindableBase
    {
        private readonly ISerialService _serialService;
        private readonly INavigationService _naviService;


        private string _spoolerPortNumber;
        public string SpoolerPortNumber
        {
            get { return _spoolerPortNumber; }
            set { SetProperty(ref _spoolerPortNumber, value); }
        }

        private string _bufferPortNumber;
        public string BufferPortNumber
        {
            get { return _bufferPortNumber; }
            set { SetProperty(ref _bufferPortNumber, value); }
        }

        public ObservableCollection<SerialPortClass> SerialPortList { get; private set; }

        private DelegateCommand<object> _autoDetectDevices;
        public DelegateCommand<object> AutoDetectDevices => _autoDetectDevices ?? (_autoDetectDevices = new DelegateCommand<object>(ExecuteAutoDetectDevices));

        private DelegateCommand<object> _acceptDevices;
        public DelegateCommand<object> AcceptDevices => _acceptDevices ?? (_acceptDevices = new DelegateCommand<object>(ExecuteAcceptDevices));

        public AutoDetectSerialPortViewModel(ISerialService serialService, INavigationService naviService)
        {
            _serialService = serialService;
            _naviService = naviService;
        }

        private void ExecuteAutoDetectDevices(object obj)
        {
            SerialPortList = new ObservableCollection<SerialPortClass>(_serialService.GetSerialPortList());
            string[] hardwareTypes = { "1", "2" }; //TODO get enums from firmware

            foreach (var serialPort in SerialPortList)
            {
                SerialCommand serialCommand = null;
                serialCommand = _serialService.CheckIfDeviceExists(serialPort, hardwareTypes).Result;
                
                if (serialCommand != null && serialCommand.DeviceID != "0" && !string.IsNullOrEmpty(serialCommand.DeviceID))
                {
                    switch (Convert.ToInt32(serialCommand.DeviceID))
                    {
                        case (int)HARDWARETYPES.Spooler:
                            SpoolerPortNumber = serialPort.SerialPort_PortName;
                            serialPort.MyHardwareType = HARDWARETYPES.Spooler;
                            break;
                        case (int)HARDWARETYPES.Buffer:
                            BufferPortNumber = serialPort.SerialPort_PortName;
                            serialPort.MyHardwareType = HARDWARETYPES.Buffer;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void ExecuteAcceptDevices(object obj)
        {
            if (!string.IsNullOrEmpty(SpoolerPortNumber))
                _serialService.ConnectToSerialPort(SpoolerPortNumber, (int)HARDWARETYPES.Spooler);
            if (!string.IsNullOrEmpty(BufferPortNumber))
                _serialService.ConnectToSerialPort2(BufferPortNumber, (int)HARDWARETYPES.Buffer);

            _naviService.NavigateTo("DiameterView");
        }
    }
}
