using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Spooler
{
    public interface ISpoolerService
    {
        bool SpoolerServiceIsEnabled { get; set; }
        string CurrentSpoolerRPM { get; set; }
        string SpoolerRPMSetpoint { get; set; }
        event EventHandler SpoolerRPMChanged;
    }
}
