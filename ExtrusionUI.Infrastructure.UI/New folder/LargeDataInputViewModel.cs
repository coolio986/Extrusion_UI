using Digital_Indicator.Infrastructure.UI.ControlBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Infrastructure.UI.Controls
{
    public class LargeDataInputViewModel : ViewModelBase
    {
        public string GetValue()
        {
            return (string)Convert.ChangeType(base.Value, typeof(string));
        }
        public string Unit { get; set; }
    }
}
