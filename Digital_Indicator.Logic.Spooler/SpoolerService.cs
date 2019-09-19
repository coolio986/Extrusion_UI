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
        private string currentSpoolerRPM;
        private string spoolerRPMSetpoint = "0";

        public bool SpoolerServiceIsEnabled
        {
            get { return true; }
            set { }
        }
        public string CurrentSpoolerRPM
        {
            get { return currentSpoolerRPM; }
            set { currentSpoolerRPM = value; }
        }

        public string SpoolerRPMSetpoint
        {
            get
            {
                return (Math.Abs((float)Convert.ChangeType(spoolerRPMSetpoint, typeof(float)))).ToString(); 
            }
            set
            {
                spoolerRPMSetpoint = (-Math.Abs((float)Convert.ChangeType(value, typeof(float)))).ToString();
                SendSerialData(new SerialCommand() { Command = "velocity", Value = spoolerRPMSetpoint, DeviceID = ((int)ConnectedDeviceTypes.SPOOLER).ToString() });
            }
        }

        public event EventHandler SpoolerRPMChanged;


        private void SendSerialData(SerialCommand serialCommand)
        {

            
            _serialService.SendSerialData(serialCommand);
        }

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
                        if (str.Length == 2)
                        {
                            command.Command = str[0];
                            command.Value = str[1];
                            currentSpoolerRPM = command.Value;
                            SpoolerRPMChanged?.Invoke(command.Value, null);
                        }
                        continue;

                    }
                    if (stringArray[i].Contains("ERROR"))
                    {

                    }
                    if (i == 0) { command.DeviceID = stringArray[i]; }
                }
            }
        }


    }
}
