using Digital_Indicator.Logic.SerialCommunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Traverse
{
    public class TraverseService : ITraverseService
    {


        public event EventHandler SpoolRPMChanged;

        ISerialService _serialService;

        private string spoolRPM;
        public string SpoolRPM
        {
            get
            {
                return spoolRPM;
            }
            set
            {
                spoolRPM = value;
                SpoolRPMChanged?.Invoke(value, null);
            }

        }




        public TraverseService(ISerialService serialService)
        {
            _serialService = serialService;
            _serialService.TraverseDataChanged += _serialService_TraverseDataChanged;

        }

        private void _serialService_TraverseDataChanged(object sender, EventArgs e)
        {
            string[] stringArray = sender.ToString().Replace("\r", "").Split(';');
            SerialCommand command = new SerialCommand();



            if (stringArray.Length == 3)
            {
                command.DeviceID = stringArray[0];
                command.Command = stringArray[1];
                command.Value = stringArray[2].Replace("\0", string.Empty); //remove nulls
                spoolRPM = command.Value;
                SpoolRPMChanged?.Invoke(command.Value, null);

            }
           
        }
    }
}


