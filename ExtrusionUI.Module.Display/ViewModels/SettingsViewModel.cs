﻿using ExtrusionUI.Core;
using ExtrusionUI.Infrastructure.UI.Controls;
using ExtrusionUI.Logic.Filament;
using ExtrusionUI.Logic.FileOperations;
using ExtrusionUI.Logic.Navigation;
using ExtrusionUI.Logic.UI_Intelligence;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExtrusionUI.Module.Display.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        IFilamentService _filamentService;
        IFileService _fileService;
        INavigationService _navigationService;
        IUI_IntelligenceService _iui_IntelligenceService;
        private DelegateCommand openSpoolDataFolder;
        private DelegateCommand closeSettingsView;
        public ObservableCollection<ViewModelBase> settingItems;

        public ObservableCollection<ViewModelBase> Traverse
        {
            get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()[StaticStrings.TRAVERSE]; }
        }

        public ObservableCollection<ViewModelBase> Production
        {
            get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Production"]; }
        }

        public ObservableCollection<ViewModelBase> Debug
        {
            get { return (ObservableCollection<ViewModelBase>)_iui_IntelligenceService.GetSettings()["Debug"]; }
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

        //public string FilamentDiameter
        //{
        //    get { return _filamentService.NominalDiameter; }
        //    set { _filamentService.NominalDiameter = value; RaisePropertyChanged(); }
        //}

        //public string UpperLimit
        //{
        //    get { return _filamentService.UpperLimit; }
        //    set { _filamentService.UpperLimit = value; RaisePropertyChanged(); }
        //}

        //public string LowerLimit
        //{
        //    get { return _filamentService.LowerLimit; }
        //    set { _filamentService.LowerLimit = value; RaisePropertyChanged(); }
        //}

        //public string FilamentDescription
        //{
        //    get { return _filamentService.Description; }
        //    set { _filamentService.Description = value; RaisePropertyChanged(); }
        //}

        //public string SpoolNumber
        //{
        //    get { return _filamentService.SpoolNumber; }
        //    set { _filamentService.SpoolNumber = value; }
        //}

        //public string BatchNumber
        //{
        //    get { return _filamentService.BatchNumber; }
        //    set { _filamentService.BatchNumber = value; }
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

        public SettingsViewModel(IFilamentService filamentService, INavigationService navigationService, IFileService fileService, IUI_IntelligenceService iui_IntelligenceService)
        {
            _filamentService = filamentService;
            _fileService = fileService;
            _navigationService = navigationService;
            _iui_IntelligenceService = iui_IntelligenceService;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            CloseSettingsView = new DelegateCommand(CloseView_Click);
            OpenSpoolDataFolder = new DelegateCommand(OpenSpoolDataFolder_Click);

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
