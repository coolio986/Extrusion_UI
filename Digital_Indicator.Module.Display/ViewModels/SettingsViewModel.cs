using Digital_Indicator.Logic.Filament;
using Digital_Indicator.Logic.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Digital_Indicator.Module.Display.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        IFilamentService _filamentService;
        INavigationService _navigationService;
        private DelegateCommand closeSettingsView;
        public DelegateCommand CloseSettingsView
        {
            get { return closeSettingsView; }
            set { SetProperty(ref closeSettingsView, value); }
        }

        public string FilamentDiameter
        {
            get { return _filamentService.NominalDiameter; }
            set { _filamentService.NominalDiameter = value; RaisePropertyChanged(); }
        }

        public string UpperLimit
        {
            get { return _filamentService.UpperLimit; }
            set { _filamentService.UpperLimit = value; RaisePropertyChanged(); }
        }

        public string LowerLimit
        {
            get { return _filamentService.LowerLimit; }
            set { _filamentService.LowerLimit = value; RaisePropertyChanged(); }
        }

        public string FilamentDescription
        {
            get { return _filamentService.Description; }
            set { _filamentService.Description = value; RaisePropertyChanged(); }
        }

        public string SpoolNumber
        {
            get { return _filamentService.SpoolNumber; }
            set { _filamentService.SpoolNumber = value; }
        }

        public string BatchNumber
        {
            get { return _filamentService.BatchNumber; }
            set { _filamentService.BatchNumber = value; }
        }

        public SettingsViewModel(IFilamentService filamentService, INavigationService navigationService)
        {
            _filamentService = filamentService;
            _navigationService = navigationService;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            CloseSettingsView = new DelegateCommand(CloseView_Click);


        }

        private void CloseView_Click()
        {
            _navigationService.ClearRegion("SettingsRegion");
        }

        private void _filamentService_PropertyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SpoolNumber");
        }
    }
}
