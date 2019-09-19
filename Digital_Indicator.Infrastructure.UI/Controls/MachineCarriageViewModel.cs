using System;


namespace Digital_Indicator.Infrastructure.UI.Controls
{
    public class MachineCarriageViewModel : ViewModelBase
    {
        public int CarriageNumber { get; set; }

        private bool linkedArrowLeft;
        private bool linkedArrowRight;
        private bool linkedCenter;
        private bool linkedBarLeft;
        private bool linkedBarRight;

        public bool LinkedArrowLeft
        {
            get { return linkedArrowLeft; }
            set { linkedArrowLeft = value; RaisePropertyChanged(); }
        }
        public bool LinkedArrowRight
        {
            get { return linkedArrowRight; }
            set { linkedArrowRight = value; RaisePropertyChanged(); }
        }
        public bool LinkedCenter
        {
            get { return linkedCenter; }
            set { linkedCenter= value; RaisePropertyChanged(); }
        }
        public bool LinkedBarLeft
        {
            get { return linkedBarLeft; }
            set { linkedBarLeft = value; RaisePropertyChanged(); }
        }
        public bool LinkedBarRight
        {
            get { return linkedBarRight; }
            set { linkedBarRight = value; RaisePropertyChanged(); }
        }

        public event EventHandler SelectionChanged;
        public event EventHandler UpdateLinkingChanged;

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

        public void UpdateLinking(object sender)
        {
            UpdateLinkingChanged?.Invoke(sender, new EventArgs());
        }

    }
}
