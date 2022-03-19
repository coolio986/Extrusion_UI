using System;

namespace ExtrusionUI.Infrastructure.UI.Controls
{
    public class DashboardItemViewModel : ViewModelBase
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
