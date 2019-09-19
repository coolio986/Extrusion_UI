using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.SerialCommunications
{

    public enum ConnectedDeviceTypes
    {
        INDICATOR = 0, //0 = indicator
        SPOOLER = 1, //1 = spooler wheel motor
        EXTRUDER = 2, //2 = extruder
        TRAVERSE = 3, //3 = Traverse
        SCREEN = 99,

        INTERNALDEVICE = 100, // 100 = internal
    }
}
