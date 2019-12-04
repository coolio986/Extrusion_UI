using Prism.Commands;
using Prism.Mvvm;
using System;
using Digital_Indicator.Logic.Filament;
using Digital_Indicator.Logic.Navigation;
using Digital_Indicator.WindowForms.ZedGraphUserControl;
using System.Windows.Media;
using Digital_Indicator.Logic.Spooler;
using System.ComponentModel;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {
        public IFilamentService _filamentService { get; }
        public DelegateCommand ResetGraph { get; private set; }
        public DelegateCommand StartCapture { get; private set; }
        public DelegateCommand StopCapture { get; private set; }
        public DelegateCommand Settings { get; private set; }
        public ISpoolerService _spoolerService { get; }

        private bool settingsOpen;
        public bool SettingsOpen
        {
            get { return settingsOpen; }
            private set { settingsOpen = value; RaisePropertyChanged(); }
        }

        public GradientStopCollection StartButtonGradientCollection
        {
            get { return GetStartButtonGradient(); }
        }

        public GradientStopCollection StopButtonGradientCollection
        {
            get { return GetStopButtonGradient(); }
        }

        private INavigationService _navigationService;

        public string Diameter
        {
            get { return _filamentService.FilamentServiceVariables["ActualDiameter"]; }
        }

        public string HighestValue
        {
            get { return _filamentService.FilamentServiceVariables["HighestValue"]; }
        }

        public string LowestValue
        {
            get { return _filamentService.FilamentServiceVariables["LowestValue"]; }
        }

        public string SpoolNumber
        {
            get { return _filamentService.FilamentServiceVariables["SpoolNumber"]; }
        }

        public string SpoolRPM
        {
            get { return _filamentService.FilamentServiceVariables["SpoolRPM"]; }
        }

        public string Tolerance
        {
            get { return _filamentService.FilamentServiceVariables["Tolerance"]; }
        }

        public bool CaptureStarted
        {
            get { return _filamentService.CaptureStarted; }
        }

        public string PullerRPM
        {
            get { return _filamentService.FilamentServiceVariables["PullerRPM"]; }
        }

        public string FilamentLength
        {
            get { return _filamentService.FilamentServiceVariables["FilamentLength"]; }
        }

        public string SpoolWeight
        {
            get { return _filamentService.FilamentServiceVariables["SpoolWeight"]; }
        }

        public string Duration
        {
            get
            {
                return (_filamentService.stopWatch.Elapsed.Hours.ToString("0") + ":" +
                  _filamentService.stopWatch.Elapsed.Minutes.ToString("0#") + ":" +
                  _filamentService.stopWatch.Elapsed.Seconds.ToString("0#"));
            }
        }

        private object settingsView;
        public object SettingsView
        {
            get { return settingsView; }
            private set { settingsView = value; RaisePropertyChanged(); }
        }

        public DiameterViewModel(IFilamentService filamentService, INavigationService navigationService, ISpoolerService spoolerService)
        {
            _filamentService = filamentService;
            _spoolerService = spoolerService;
            
            _navigationService = navigationService;
            _navigationService.RegionCleared += _navigationService_RegionCleared;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;
            _filamentService.StopWatchedTimeChanged += _filamentService_StopWatchedTimeChanged;


            ResetGraph = new DelegateCommand(ResetGraph_Click);
            StartCapture = new DelegateCommand(StartCapture_Click);
            StopCapture = new DelegateCommand(StopCapture_Click);
            Settings = new DelegateCommand(Settings_Click);
        }

        private void _filamentService_StopWatchedTimeChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Duration");
        }

        private void _navigationService_RegionCleared(object sender, EventArgs e)
        {
            if (sender.ToString() == "SettingsRegion")
            {
                SettingsOpen = false;
            }
        }

        private void _filamentService_PropertyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(((PropertyChangedEventArgs)e).PropertyName);
        }

        private void ResetGraph_Click()
        {
            ZedGraphPlotModel.GetPlot("HistoricalModel").ZoomOutAll();
        }

        private void StartCapture_Click()
        {
            if (!_filamentService.CaptureStarted)
            {
                _filamentService.CaptureStarted = true;
                RaisePropertyChanged("CaptureStarted");
                RaisePropertyChanged("RealTimeModel");
                RaisePropertyChanged("StartButtonGradientCollection");
                RaisePropertyChanged("StopButtonGradientCollection");
            }
        }

        private void StopCapture_Click()
        {
            if (_filamentService.CaptureStarted)
            {
                _filamentService.CaptureStarted = false;
                RaisePropertyChanged("CaptureStarted");
                RaisePropertyChanged("StartButtonGradientCollection");
                RaisePropertyChanged("StopButtonGradientCollection");
            }
        }

        private void _traverseService_SpoolRPMChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SpoolRPM");
        }

        private void Settings_Click()
        {
            SettingsOpen = true;
            _navigationService.NavigateToRegion("SettingsRegion", "SettingsView");
        }
        private GradientStopCollection GetStartButtonGradient()
        {
            GradientStopCollection gradientStartCollection = new GradientStopCollection();

            if (_filamentService.CaptureStarted)
            {
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF00FF00"), Offset = 0 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF00FC00"), Offset = 0.21 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF00F200"), Offset = 0.38 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF00E100"), Offset = 0.53 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF00C900"), Offset = 0.67 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF00AA00"), Offset = 0.81 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF008500"), Offset = 0.94 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF007300"), Offset = 1 });

            }
            else
            {
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#e6e6e6"), Offset = 0 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#d9d9d9"), Offset = 0.21 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#cccccc"), Offset = 0.38 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#bfbfbf"), Offset = 0.53 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#b3b3b3"), Offset = 0.67 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#a6a6a6"), Offset = 0.81 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#999999"), Offset = 0.94 });
                gradientStartCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#8c8c8c"), Offset = 1 });
            }
            return gradientStartCollection;

        }

        private GradientStopCollection GetStopButtonGradient()
        {
            GradientStopCollection gradientStopCollection = new GradientStopCollection();
            if (_filamentService.CaptureStarted)
            {
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#e6e6e6"), Offset = 0 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#d9d9d9"), Offset = 0.21 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#cccccc"), Offset = 0.38 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#bfbfbf"), Offset = 0.53 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#b3b3b3"), Offset = 0.67 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#a6a6a6"), Offset = 0.81 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#999999"), Offset = 0.94 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#8c8c8c"), Offset = 1 });
            }


            else
            {
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FFFF0000"), Offset = 0 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FFFC0000"), Offset = 0.21 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FFF20000"), Offset = 0.38 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FFE10000"), Offset = 0.53 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FFC90000"), Offset = 0.67 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FFAA0000"), Offset = 0.81 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF850000"), Offset = 0.94 });
                gradientStopCollection.Add(new GradientStop { Color = (Color)ColorConverter.ConvertFromString("#FF730000"), Offset = 1 });
            }
            return gradientStopCollection;
        }
    }
}

