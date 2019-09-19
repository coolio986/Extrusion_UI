using Digital_Indicator.Infrastructure.UI.ControlBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Infrastructure.UI.Controls
{
    public class DoubleInputViewModel : ViewModelBase
    {
        public double GetValue()
        {
            return (double)Convert.ChangeType(base.Value, typeof(double));
        }

        public string Unit { get; set; }
    }
}
