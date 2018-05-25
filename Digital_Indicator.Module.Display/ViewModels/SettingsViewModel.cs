using Digital_Indicator.Logic.Filament;
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
        private DelegateCommand closeSettingsView;
        public DelegateCommand CloseSettingsView
        {
            get { return closeSettingsView; }
            set { SetProperty(ref closeSettingsView, value); }
        }

        private string filamentDiameter;
        public string FilamentDiameter
        {
            get { return filamentDiameter; }
            set { SetProperty(ref filamentDiameter, value); }
        }

        private string upperLimit;
        public string UpperLimit
        {
            get { return upperLimit; }
            set { SetProperty(ref upperLimit, value); }
        }

        private string lowerLimit;
        public string LowerLimit
        {
            get { return lowerLimit; }
            set { SetProperty(ref lowerLimit, value); }
        }

        private string filamentDescription;
        public string FilamentDescription
        {
            get { return filamentDescription; }
            set { SetProperty(ref filamentDescription, value); }
        }

        public SettingsViewModel(IFilamentService filamentService)
        {
            _filamentService = filamentService;

            FilamentDescription = _filamentService.Description;
            FilamentDiameter = _filamentService.NominalDiameter;
            UpperLimit = _filamentService.UpperLimit;
            LowerLimit = _filamentService.LowerLimit;
        }
    }
}
