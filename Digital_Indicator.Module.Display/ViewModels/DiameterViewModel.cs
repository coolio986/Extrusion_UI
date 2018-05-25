using Digital_Indicator.Logic.SerialCommunications;
using OxyPlot;
using OxyPlot.Axes;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Digital_Indicator.Logic.Helpers;
using Digital_Indicator.Module.Display.Views;
using Digital_Indicator.Infrastructure.UI;
using Digital_Indicator.Logic.Filament;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {
        //private ISerialService _serialService;
        private IFilamentService _filamentService;
        public DelegateCommand ResetGraph { get; set; }
        public DelegateCommand StartCapture { get; set; }
        public DelegateCommand StopCapture { get; set; }
        public DelegateCommand Settings { get; set; }

        private LinearSeriesPlotModel realTimeModel;
        public LinearSeriesPlotModel RealTimeModel
        {
            get { return realTimeModel; }
            private set { SetProperty(ref realTimeModel, value); }
        }

        private LinearSeriesPlotModel historicalModel;
        public LinearSeriesPlotModel HistoricalModel
        {
            get { return historicalModel; }
            set { SetProperty(ref historicalModel, value); }
        }

        public string Diameter
        {
            get { return _filamentService.ActualDiameter; }
        }

        public string HighestValue
        {
            get { return _filamentService.HighestValue; }
        }

        public string LowestValue
        {
            get { return _filamentService.LowestValue; }
        }

        private object settingsView;
        public object SettingsView
        {
            get { return settingsView; }
            set { SetProperty(ref settingsView, value); }
        }

        Stopwatch stopWatch;
        long previousMillis;

        public DiameterViewModel(IFilamentService filamentService)
        {
            _filamentService = filamentService;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            SetupRealTimeView();
            SetupHistoricalView();
            StartHistoricalTimer();

            ResetGraph = new DelegateCommand(ResetGraph_Click);
            StartCapture = new DelegateCommand(StartCapture_Click);
            StopCapture = new DelegateCommand(StopCapture_Click);
            Settings = new DelegateCommand(Settings_Click);

            stopWatch = new Stopwatch(); //For timing historical plot
            stopWatch.Start();
            previousMillis = stopWatch.ElapsedMilliseconds;
        }

        private void _filamentService_PropertyChanged(object sender, EventArgs e)
        {
            RealTimeModel.NominalDiameter = _filamentService.NominalDiameter;
            RealTimeModel.UpperLimitDiameter = _filamentService.UpperLimit;
            RealTimeModel.LowerLimitDiameter = _filamentService.LowerLimit;

            HistoricalModel.NominalDiameter = _filamentService.NominalDiameter;
            HistoricalModel.UpperLimitDiameter = _filamentService.UpperLimit;
            HistoricalModel.LowerLimitDiameter = _filamentService.LowerLimit;
        }

        private void ResetGraph_Click()
        {
            HistoricalModel.ResetAllAxes();
            HistoricalModel.InvalidatePlot(true);
        }

        private void StartCapture_Click()
        {
            _filamentService.CaptureStarted = true;
            SetupRealTimeView();
            SetupHistoricalView();
            StartHistoricalTimer();
        }

        private void StopCapture_Click()
        {
            _filamentService.CaptureStarted = false;
        }

        private void Settings_Click()
        {
            SettingsView = new SettingsView();
        }

        private void SetupRealTimeView()
        {
            RealTimeModel = new LinearSeriesPlotModel()
            {
                Title = "RealTime Diameter",
                UpperLimitDiameter = _filamentService.UpperLimit,
                NominalDiameter = _filamentService.NominalDiameter,
                LowerLimitDiameter = _filamentService.LowerLimit,
            };
            
        }

        private void SetupHistoricalView()
        {
            HistoricalModel = new LinearSeriesPlotModel()
            {
                Title = "Historical Diameter",
                UpperLimitDiameter = _filamentService.UpperLimit,
                NominalDiameter = _filamentService.NominalDiameter,
                LowerLimitDiameter = _filamentService.LowerLimit,
            };
        }

        private void StartHistoricalTimer()
        {
            //acts as a timer
            //Task.Factory.StartNew(() =>
            //{
            //    while (IsStarted)
            //    {
            //        //Update plot on main UI thread, prevents cross thread violations
            //        //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //        //{
            //        //    HistoricalModel.InvalidatePlot(true);
            //        //}));

            //        //HistoricalModel.InvalidatePlot(true);

            //        Thread.Sleep(5000);
            //    }
            //});
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Diameter");
            RaisePropertyChanged("HighestValue");
            RaisePropertyChanged("LowestValue");


            if (_filamentService.CaptureStarted)
            {
                UpdateRealTimePlot();
                UpdateHistoricalPlot();
            }
        }

        private void UpdateRealTimePlot()
        {
            if (RealTimeModel != null)
            {
                RealTimeModel.AddDataPoint(Diameter);
                HistoricalModel.AddDataPoint(Diameter);

                if (RealTimeModel.Axes.Count > 1)
                {
                    RealTimeModel.Axes[0].Zoom(DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(-5000)), DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(200)));
                    RealTimeModel.InvalidatePlot(true);
                }
            }
        }

        private void UpdateHistoricalPlot()
        {
            if (stopWatch.ElapsedMilliseconds >= previousMillis + 5000)
            {
                Task.Factory.StartNew(() =>
                {
                    HistoricalModel.InvalidatePlot(true);
                    previousMillis = stopWatch.ElapsedMilliseconds;
                });
            }
        }

        
    }
}

