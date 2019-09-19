using Digital_Indicator.Infrastructure.UI.Controls;
using Digital_Indicator.Logic.Filament;
using Digital_Indicator.Logic.FileOperations;
using Digital_Indicator.Logic.Navigation;
using Digital_Indicator.Logic.Spooler;
using Digital_Indicator.Logic.UI_Intelligence;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        IUI_IntelligenceService _iui_IntelligenceService;
        private DelegateCommand openSpoolDataFolder;
        private DelegateCommand closeSettingsView;
        public ObservableCollection<ViewModelBase> settingItems;

        public ObservableCollection<ViewModelBase> SettingItems
        {
            get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings(); }
        }

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


        public SettingsViewModel(IFilamentService filamentService, INavigationService navigationService, IFileService fileService, ISpoolerService spoolerService, IUI_IntelligenceService iui_IntelligenceService)
        {
            _filamentService = filamentService;
            _fileService = fileService;
            _spoolerService = spoolerService;
            _navigationService = navigationService;
            _iui_IntelligenceService = iui_IntelligenceService;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;
            _spoolerService.SpoolerRPMChanged += _spoolerService_SpoolerRPMChanged;
            

            CloseSettingsView = new DelegateCommand(CloseView_Click);
            OpenSpoolDataFolder = new DelegateCommand(OpenSpoolDataFolder_Click);

            //settingItems = new ObservableCollection<ViewModelBase>();

            //DataInputViewModel spooler = new DataInputViewModel();
            //spooler.ParameterName = "Spooler RPM Setpoint";
            //spooler.HardwareType = "1";
            //spooler.SerialCommand = "velocity";
            //spooler.PropertyChanged += SpoolerDataChanged;

            //settingItems.Add(spooler);
            //RaisePropertyChanged("SettingItems");

            

        }

        private void _spoolerService_SpoolerRPMChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("CurrentSpoolerRPM");
        }

        private void SpoolerDataChanged(object sender, EventArgs e)
        {
            _spoolerService.SpoolerRPMSetpoint = ((DataInputViewModel)sender).Value.ToString();

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
