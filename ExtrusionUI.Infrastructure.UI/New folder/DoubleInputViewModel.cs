using Digital_Indicator.Infrastructure.UI.ControlBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Infrastructure.UI.Controls
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
