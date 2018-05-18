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
    public class DiameterViewModel : BindableBase
    {
        private ISerialService _serialService;

        public ObservableCollection<string> SerialPortList { get; }

        private string diameter;
        public string Diameter
        {
            get { return diameter; }
            set { SetProperty(ref diameter, value); }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DiameterViewModel(ISerialService serialService)
        {
            Message = "View A from your Prism Module";

            _serialService = serialService;
            _serialService.DiameterChanged += _serialService_DiameterChanged;

            SerialPortList = new ObservableCollection<string>(_serialService.GetSerialPortList());

            if (_serialService.IsSimulationModeActive())
            {
                //************TEST DATA**********//
                _serialService.ConnectToSerialPort("COM3");
                //*******************************//
            }
            
        }

        private void _serialService_DiameterChanged(object sender, EventArgs e)
        {
            Diameter = sender.ToString();
        }
    }
}
