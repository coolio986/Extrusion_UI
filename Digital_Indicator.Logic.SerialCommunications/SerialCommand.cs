using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.SerialCommunications
{
    public class SerialCommand
    {

        string deviceID = string.Empty;
        string command = string.Empty;
        string value = string.Empty;

        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }
        public string Command
        {
            get { return command; }
            set { command = value; }
        }
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string AssembleCommand()
        {
            string assembledCommand = string.Empty;
            if (deviceID == string.Empty || command == string.Empty || value == string.Empty)
            {
                return "ERROR";
            }
            else
            {
                return deviceID + ";" + command + " " + this.value + ";\r\n";
            }
        }
    }
}
