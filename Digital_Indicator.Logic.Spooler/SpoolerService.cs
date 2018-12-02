using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digital_Indicator.Logic.SerialCommunications;


namespace Digital_Indicator.Logic.Spooler
{
    public class SpoolerService : ISpoolerService
    {
        ISerialService _serialService;
        private string spoolerRPM;

        public bool SpoolerServiceIsEnabled
        {
            get { return true; }
            set { }
        }
        public string SpoolerRPM
        {
            get { return spoolerRPM; }
            set { spoolerRPM = value; }
        }

        public event EventHandler SpoolerRPMChanged;
        
        public SpoolerService(ISerialService serialService)
        {
            _serialService = serialService;

            //SpoolerRPMChanged += SpoolerService_SpoolerRPMChanged;
            _serialService.SpoolerDataChanged += _serialService_SpoolerDataChanged;

        }

        private void _serialService_SpoolerDataChanged(object sender, EventArgs e)
        {
            string[] stringArray = sender.ToString().Replace("\r", "").Split(';');
            SerialCommand command = new SerialCommand();

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] != string.Empty)
                {
                    if (stringArray[i].Contains("="))
                    {
                        string[] str = stringArray[i].Replace(" ", "").Split('=');
                        if(str.Length == 2)
                        {
                            command.Command = str[0];
                            command.Value = str[1];
                            spoolerRPM = command.Value;
                            SpoolerRPMChanged?.Invoke(command.Value, null);
                        }
                        continue;

                    }
                    if (i == 0) { command.DeviceID = stringArray[i]; }
                }
            }
        }

        
    }
}
