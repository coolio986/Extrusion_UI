using Digital_Indicator.Logic.Navigation;
using Digital_Indicator.Logic.SerialCommunications;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class SerialPortSelectionViewModel : BindableBase
    {
        private ISerialService _serialService;
        private INavigationService _naviService;

        public ObservableCollection<SerialPortClass> SerialPortList { get; }

        public DelegateCommand NextScreen { get; }

        private SerialPortClass serialPortSelection;
        public SerialPortClass SerialPortSelection
        {
            get
            {
                return serialPortSelection;
            }
            set
            {
                serialPortSelection = value;
                SetSerialPort();
            }
        }


        public SerialPortSelectionViewModel(ISerialService serialService, INavigationService naviService)
        {
            _serialService = serialService;
            _naviService = naviService;
            SerialPortList = new ObservableCollection<SerialPortClass>(_serialService.GetSerialPortList());

            NextScreen = new DelegateCommand(NextScreen_Click);

        }

        private void SetSerialPort()
        {
            _serialService.ConnectToSerialPort(serialPortSelection.SerialPort_PortName);
        }

        private void NextScreen_Click()
        {
            _naviService.NavigateTo("DiameterView");
        }

    }
}
