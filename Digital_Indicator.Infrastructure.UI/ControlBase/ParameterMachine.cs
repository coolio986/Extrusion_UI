using Digital_Indicator.Infrastructure.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Infrastructure.UI.ControlBase
{
    public class ParameterMachine : ViewModelBase
    {
        public string ImageSource { get; set; }

        public event EventHandler SelectionChanged;

        public bool IsSelected
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    SelectionChanged?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
