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

        public ObservableCollection<ViewModelBase> Machine
        {
            get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Machine"]; }
        }

        public ObservableCollection<ViewModelBase> Production
        {
            get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Production"]; }
        }

        //public ObservableCollection<ViewModelBase> Traverse
        //{
        //    get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Traverse"]; }
        //}

        //public ObservableCollection<ViewModelBase> Pulling
        //{
        //    get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Pulling"]; }
        //}

        //public ObservableCollection<ViewModelBase> Info
        //{
        //    get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Info"]; }
        //}

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

        //public string FilamentDiameter
        //{
        //    get { return _filamentService.FilamentServiceVariables["NominalDiameter"]; }
        //    set { _filamentService.FilamentServiceVariables["NominalDiameter"] = value; RaisePropertyChanged(); }
        //}

        //public string UpperLimit
        //{
        //    get { return _filamentService.FilamentServiceVariables["UpperLimit"]; }
        //    set { _filamentService.FilamentServiceVariables["UpperLimit"] = value; RaisePropertyChanged(); }
        //}

        //public string LowerLimit
        //{
        //    get { return _filamentService.FilamentServiceVariables["LowerLimit"]; }
        //    set { _filamentService.FilamentServiceVariables["LowerLimit"] = value; RaisePropertyChanged(); }
        //}

        //public string FilamentDescription
        //{
        //    get { return _filamentService.FilamentServiceVariables["Description"]; }
        //    set { _filamentService.FilamentServiceVariables["Description"] = value; RaisePropertyChanged(); }
        //}

        //public string SpoolNumber
        //{
        //    get { return _filamentService.FilamentServiceVariables["SpoolNumber"]; }
        //    set { _filamentService.FilamentServiceVariables["SpoolNumber"] = value; }
        //}

        //public string SpoolerRPMSetpoint
        //{
        //    get { return _spoolerService.SpoolerRPMSetpoint; }
        //    set { _spoolerService.SpoolerRPMSetpoint = value; }
        //}

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
