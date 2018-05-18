using Prism.Mvvm;

namespace Digital_Indicator.Startup.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Digital Indicator for SPC";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
