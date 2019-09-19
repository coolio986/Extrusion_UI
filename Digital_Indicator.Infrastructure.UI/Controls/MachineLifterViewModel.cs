using System;

namespace Digital_Indicator.Infrastructure.UI.Controls
{
    public class MachineLifterViewModel : ViewModelBase
    {
        public int LifterNumber { get; set; }
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
