﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using ExtrusionUI.Logic.Filament;
using ExtrusionUI.Logic.Navigation;
using ExtrusionUI.WindowForms.ZedGraphUserControl;
using System.Windows.Media;
using ExtrusionUI.Logic.SerialCommunications;
using ExtrusionUI.Core;
using ExtrusionUI.Logic.UI_Intelligence;
using System.Windows;
using System.Diagnostics;

namespace ExtrusionUI.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {

        private Stopwatch timer1 = new Stopwatch();
        private long minTimeDelay = 1000;
        private long previousMillis1;
        private long previousMillis2;
        private long previousMillis3;
        private long previousMillis4;



        private readonly ISerialService _serialService;
        private readonly IUI_IntelligenceService _iui_IntelligenceService;
        public IFilamentService _filamentService { get; }
        public DelegateCommand ResetGraph { get; private set; }
        public DelegateCommand StartCapture { get; private set; }
        public DelegateCommand StopCapture { get; private set; }
        public DelegateCommand Settings { get; private set; }

        private DelegateCommand<object> _homeCommand;
        public DelegateCommand<object> HomeCommand => _homeCommand ?? (_homeCommand = new DelegateCommand<object>(ExecuteHomeCommand));

        private DelegateCommand<object> _runCommand;
        public DelegateCommand<object> RunCommand => _runCommand ?? (_runCommand = new DelegateCommand<object>(ExecuteRunCommand));

        private DelegateCommand<object> _stopCommand;
        public DelegateCommand<object> StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand<object>(ExecuteStopCommand));

        private DelegateCommand<object> _eStopCommand;
        public DelegateCommand<object> EStopCommand => _eStopCommand ?? (_eStopCommand = new DelegateCommand<object>(ExecuteEStopCommand));

        private DelegateCommand<object> _runTraverseToStart;
        public DelegateCommand<object> RunTraverseToStart => _runTraverseToStart ?? (_runTraverseToStart = new DelegateCommand<object>(ExecuteRunTraverseToStartCommand, CanExecuteRunTraverseToStartCommand));

        private int _comSpoolerFrameSize;
        public int COMSpoolerFrameSize
        {
            get { return _comSpoolerFrameSize; }
            set
            {
                if (value != _comSpoolerFrameSize)
                {
                    //SetProperty(ref _comSpoolerFrameSize, value);
                    _comSpoolerFrameSize = value;
                    if (timer1.ElapsedMilliseconds > previousMillis1 + minTimeDelay)
                    {
                        RaisePropertyChanged();
                        previousMillis1 = timer1.ElapsedMilliseconds;
                    }
                }
            }
        }

        private int _comBufferFrameSize;
        public int COMBufferFrameSize
        {
            get { return _comBufferFrameSize; }
            set
            {
                if (value != _comBufferFrameSize)
                {
                    _comBufferFrameSize = value;
                    if (timer1.ElapsedMilliseconds > previousMillis2 + minTimeDelay)
                    {
                        RaisePropertyChanged();
                        previousMillis2 = timer1.ElapsedMilliseconds;
                    }
                    //SetProperty(ref _comBufferFrameSize, value);

                }
            }
        }

        private int _comSpoolerFrameHighest;
        public int COMSpoolerFrameHighest
        {
            get { return _comSpoolerFrameHighest; }
            set
            {
                if (value != _comSpoolerFrameHighest)
                {
                    _comSpoolerFrameHighest = value;
                    if (timer1.ElapsedMilliseconds > previousMillis3 + minTimeDelay)
                    {
                        RaisePropertyChanged();
                        previousMillis3 = timer1.ElapsedMilliseconds;
                    }
                    //SetProperty(ref _comSpoolerFrameHighest, value);
                }
            }
        }

        private int _comBufferFrameHighest;
        public int COMBufferFrameHighest
        {
            get { return _comBufferFrameHighest; }
            set
            {
                if (value != _comBufferFrameHighest)
                {
                    _comBufferFrameHighest = value;
                    if (timer1.ElapsedMilliseconds > previousMillis4 + minTimeDelay)
                    {
                        RaisePropertyChanged();
                        previousMillis4 = timer1.ElapsedMilliseconds;
                    }
                    //SetProperty(ref _comBufferFrameHighest, value);
                }
            }
        }

        public bool HomeTraverseIsVisible
        {
            get { return CanStartCapture(); }

        }

        private bool startButtonMask = false;
        private void ExecuteHomeCommand(object obj)
        {
            _serialService.SendSerialData(new SerialCommand() { Command = "home", DeviceID = "1" });
        }

        private void ExecuteRunCommand(object obj)
        {
            _serialService.SendSerialData(new SerialCommand() { Command = "run", DeviceID = "1" });
        }

        private void ExecuteStopCommand(object obj)
        {
            _serialService.SendSerialData(new SerialCommand() { Command = "stop", DeviceID = "1" });
        }

        private void ExecuteEStopCommand(object obj)
        {
            _serialService.SendSerialData(new SerialCommand() { Command = "estop", DeviceID = "1" });
        }

        private void ExecuteRunTraverseToStartCommand(object obj)
        {
            _serialService.SendSerialData(new SerialCommand() { Command = "RunTraverseToStart", DeviceID = "1" });
        }

        private bool CanExecuteRunTraverseToStartCommand(object arg)
        {
            return _filamentService.FilamentServiceVariables[StaticStrings.TRAVERSEMOTIONSTATUS] == "Stopped" &&
                   (_filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] == "Stopped" ||
                   _filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] == "None");

        }

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
            get { return _filamentService.FilamentServiceVariables[StaticStrings.ACTUALDIAMETER]; }
        }

        public string HighestValue
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.HIGHESTVALUE]; }
        }

        public string LowestValue
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.LOWESTVALUE]; }
        }

        public string SpoolNumber
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.SPOOLNUMBER]; }
        }

        public string SpoolRPM
        {
            get { return _filamentService.FilamentServiceVariables["SpoolRPM"]; }
        }

        public string Tolerance
        {
            get { return _filamentService.FilamentServiceVariables["Tolerance"]; }
        }

        public string SpecificGravity
        {
            get { return _filamentService.FilamentServiceVariables["SpecificGravity"]; }
        }

        public bool CaptureStarted
        {
            get { return _filamentService.CaptureStarted; }
        }

        public string PullerRPM
        {
            get { return _filamentService.FilamentServiceVariables["velocity"]; }
        }

        public string FilamentLength
        {
            get { return _filamentService.FilamentServiceVariables["FilamentLength"]; }
        }

        public string SpoolWeight
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.SPOOLWEIGHT]; }
        }

        public string Feedrate
        {
            get { return _filamentService.FilamentServiceVariables["Feedrate"]; }
        }

        public string OutputRate
        {
            get { return _filamentService.FilamentServiceVariables["OutputRate"]; }
        }

        public string Duration
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.DURATION]; }
        }

        public string RemainingTime
        {
            get { return _filamentService.FilamentServiceVariables["RemainingTime"]; }
        }

        public string KeepAlive
        {
            get { return _filamentService.FilamentServiceVariables["KeepAlive"]; }
        }

        public string TraverseMotionStatus
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.TRAVERSEMOTIONSTATUS]; }
        }

        public string SpoolMotionStatus
        {
            get { return _filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS]; }
        }

        public object HighestForgroundColor
        {
            get
            {

                double highLimit = Convert.ToDouble(_filamentService.FilamentServiceVariables[StaticStrings.FILAMENTUPPERLIMIT]);
                if (!string.IsNullOrEmpty(HighestValue))
                {
                    double highestValue = Convert.ToDouble(HighestValue);
                    return highestValue > highLimit ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Black;
                }
                return System.Windows.Media.Brushes.Black;
            }
        }

        public object LowestForgroundColor
        {
            get
            {
                double lowLimit = Convert.ToDouble(_filamentService.FilamentServiceVariables[StaticStrings.FILAMENTLOWERLIMIT]);
                if (!string.IsNullOrEmpty(LowestValue))
                {
                    double lowestValue = Convert.ToDouble(LowestValue);
                    return lowestValue < lowLimit ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Black;
                }
                return System.Windows.Media.Brushes.Black;
            }
        }


        private object settingsView;
        public object SettingsView
        {
            get { return settingsView; }
            private set { settingsView = value; RaisePropertyChanged(); }
        }

        public DiameterViewModel(IFilamentService filamentService, INavigationService navigationService, ISerialService serialService, IUI_IntelligenceService iui_IntelligenceService)
        {
            _filamentService = filamentService;
            _navigationService = navigationService;
            _navigationService.RegionCleared += _navigationService_RegionCleared;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;
            _filamentService.StopWatchedTimeChanged += _filamentService_StopWatchedTimeChanged;
            _serialService = serialService;
            _iui_IntelligenceService = iui_IntelligenceService;

            ResetGraph = new DelegateCommand(ResetGraph_Click);
            StartCapture = new DelegateCommand(StartCapture_Click, CanStartCapture);
            StopCapture = new DelegateCommand(StopCapture_Click, CanStopCapture);
            Settings = new DelegateCommand(Settings_Click);

            StartCapture.RaiseCanExecuteChanged();
            _serialService.SerialBufferSizeChanged += _serialService_SerialBufferSizeChanged;

            timer1.Start();
        }

        private void _serialService_SerialBufferSizeChanged(object sender, EventArgs e)
        {
            if (sender is SerialPortClass comPort)
            {

                switch (comPort.MyHardwareType)
                {
                    case HARDWARETYPES.Spooler:
                        COMSpoolerFrameSize = comPort.BufferSize;
                        break;
                    case HARDWARETYPES.Buffer:
                        COMBufferFrameSize = comPort.BufferSize;
                        break;
                    default:
                        break;
                }
                if (_filamentService.CaptureStarted)
                {
                    COMSpoolerFrameHighest = COMSpoolerFrameSize > COMSpoolerFrameHighest ? COMSpoolerFrameSize : COMSpoolerFrameHighest;
                    COMBufferFrameHighest = COMBufferFrameSize > COMBufferFrameHighest ? COMBufferFrameSize : COMBufferFrameHighest;
                }
                else
                {
                    COMSpoolerFrameHighest = 0;
                    COMBufferFrameHighest = 0;
                }
            }
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
            RaisePropertyChanged("SpoolNumber");
            RaisePropertyChanged("BatchNumber");
            RaisePropertyChanged("HighestForgroundColor");
            RaisePropertyChanged("LowestForgroundColor");

            //Console.WriteLine(((SerialCommand)sender).Command);
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                RunTraverseToStart.RaiseCanExecuteChanged();
                StartCapture.RaiseCanExecuteChanged();
                StopCapture.RaiseCanExecuteChanged();

                if (HomeTraverseIsVisible)
                    RaisePropertyChanged("HomeTraverseIsVisible");
            }));

            if (_filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] == "Stopped")
            {

            }
            if (_filamentService.CaptureStarted && _filamentService.FilamentServiceVariables.ContainsKey(StaticStrings.SPOOLMOTIONSTATUS) &&
                (_filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] == "Stopped" ||
                _filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] == "None")
                && _filamentService.FilamentServiceVariables.ContainsKey(StaticStrings.LOGGERMOTIONSTATE) &&
                _filamentService.FilamentServiceVariables[StaticStrings.LOGGERMOTIONSTATE] == "StopCapture")
            {
                //if(startButtonMask)
                //{
                //    startButtonMask = false;
                //    return;
                //}

                _filamentService.CaptureStarted = false;
                RaisePropertyChanged("CaptureStarted");
                RaisePropertyChanged("StartButtonGradientCollection");
                RaisePropertyChanged("StopButtonGradientCollection");
                RaisePropertyChanged("SpoolNumber");
                _filamentService.FilamentServiceVariables[StaticStrings.LOGGERMOTIONSTATE] = "StopCapture";

            }
            if (_filamentService.FilamentServiceVariables.ContainsKey(StaticStrings.LOGGERMOTIONSTATE)
                && _filamentService.FilamentServiceVariables[StaticStrings.LOGGERMOTIONSTATE] == "RunCapture"
                )
            {
                StartCapture_Click();
            }
            if (_filamentService.FilamentServiceVariables.ContainsKey(StaticStrings.LOGGERMOTIONSTATE)
                && _filamentService.FilamentServiceVariables[StaticStrings.LOGGERMOTIONSTATE] == "StopCapture"
                && _filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] != "Stopping"
                )
            {
                StopCapture_Click();
            }
        }

        private void ResetGraph_Click()
        {
            ZedGraphPlotModel.GetPlot("HistoricalModel").ZoomOutAll();
        }

        private void StartCapture_Click()
        {
            if (!_filamentService.CaptureStarted)
            {
                //startButtonMask = true;
                _filamentService.CaptureStarted = true;
                RaisePropertyChanged("CaptureStarted");
                RaisePropertyChanged("RealTimeModel");
                RaisePropertyChanged("StartButtonGradientCollection");
                RaisePropertyChanged("StopButtonGradientCollection");
                RaisePropertyChanged("SpoolNumber");
                _filamentService.FilamentServiceVariables[StaticStrings.LOGGERMOTIONSTATE] = "RunCapture";
                ExecuteRunCommand(null);
            }
        }

        private bool CanStartCapture()
        {
            return _filamentService.FilamentServiceVariables[StaticStrings.TRAVERSEMOTIONSTATUS] != "None"
                && _filamentService.FilamentServiceVariables[StaticStrings.TRAVERSEMOTIONSTATUS] != "Homing";
        }

        private void StopCapture_Click()
        {
            if (_filamentService.CaptureStarted)
            {
                //_filamentService.CaptureStarted = false;
                //RaisePropertyChanged("CaptureStarted");
                //RaisePropertyChanged("StartButtonGradientCollection");
                //RaisePropertyChanged("StopButtonGradientCollection");
                //RaisePropertyChanged("SpoolNumber");
                ExecuteStopCommand(null);
            }
        }

        private bool CanStopCapture()
        {
            return _filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] != "None" && _filamentService.FilamentServiceVariables[StaticStrings.SPOOLMOTIONSTATUS] != "Stopping";
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

