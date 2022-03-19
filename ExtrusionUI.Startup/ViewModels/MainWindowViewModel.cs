using Prism.Mvvm;

namespace ExtrusionUI.Startup.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Filalogger - Filament Diameter Measurement";
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
