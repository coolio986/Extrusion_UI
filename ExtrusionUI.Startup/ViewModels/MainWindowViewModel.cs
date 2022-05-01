using Prism.Mvvm;

namespace ExtrusionUI.Startup.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Fusion Filaments - Extrusion UI";
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
