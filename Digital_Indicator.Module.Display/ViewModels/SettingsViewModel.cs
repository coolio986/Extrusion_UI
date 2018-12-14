using Digital_Indicator.Logic.Filament;
using Digital_Indicator.Logic.FileOperations;
using Digital_Indicator.Logic.Navigation;
using Digital_Indicator.Logic.Spooler;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Digital_Indicator.Module.Display.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        IFilamentService _filamentService;
        IFileService _fileService;
        ISpoolerService _spoolerService { get; set; }
        INavigationService _navigationService;
        private DelegateCommand openSpoolDataFolder;
        private DelegateCommand closeSettingsView;
        public DelegateCommand CloseSettingsView
        {
            get { return closeSettingsView; }
            set { SetProperty(ref closeSettingsView, value); }
        }

        public DelegateCommand OpenSpoolDataFolder
        {
            get { return openSpoolDataFolder; }
            set { SetProperty(ref openSpoolDataFolder, value); }
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

        public string CurrentSpoolerRPM
        {
            get { return _spoolerService.CurrentSpoolerRPM; }
            set { }
        }

        
        public string SpoolerRPMSetpoint
        {
            get { return _spoolerService.SpoolerRPMSetpoint; }
            set { _spoolerService.SpoolerRPMSetpoint = value; }
        }

        public string VersionNumber
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public SettingsViewModel(IFilamentService filamentService, INavigationService navigationService, IFileService fileService, ISpoolerService spoolerService)
        {
            _filamentService = filamentService;
            _fileService = fileService;
            _spoolerService = spoolerService;
            _navigationService = navigationService;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;
            _spoolerService.SpoolerRPMChanged += _spoolerService_SpoolerRPMChanged;

            CloseSettingsView = new DelegateCommand(CloseView_Click);
            OpenSpoolDataFolder = new DelegateCommand(OpenSpoolDataFolder_Click);

        }

        private void _spoolerService_SpoolerRPMChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("CurrentSpoolerRPM");
        }

        private void CloseView_Click()
        {
            
        }

        private void OpenSpoolDataFolder_Click()
        {
            Process.Start(_fileService.EnvironmentDirectory);
        }
        private void _filamentService_PropertyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SpoolNumber");
        }

        public void CloseSettings()
        {
            _navigationService.ClearRegion("SettingsRegion");
        }
    }
}
