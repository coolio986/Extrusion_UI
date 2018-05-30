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
using Digital_Indicator.Logic.FileOperations;
using System.Collections.Generic;
using Digital_Indicator.Infrastructure;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {
        private IXmlService _xmlService;
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

        public string SpoolNumber
        {
            get { return _filamentService.SpoolNumber; }
        }

        public string BatchNumber
        {
            get { return _filamentService.BatchNumber; }
        }

        public bool CaptureStarted
        {
            get { return _filamentService.CaptureStarted; }
        }

        private object settingsView;
        public object SettingsView
        {
            get { return settingsView; }
            set { SetProperty(ref settingsView, value); }
        }

        Stopwatch stopWatch;
        long previousMillis;

        public DiameterViewModel(IFilamentService filamentService, IXmlService xmlService)
        {
            _xmlService = xmlService;
            _filamentService = filamentService;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            SetupRealTimeView();
            SetupHistoricalView();

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
            RealTimeModel.UpperLimitDiameter = _filamentService.UpperLimit;
            RealTimeModel.NominalDiameter = _filamentService.NominalDiameter;
            RealTimeModel.LowerLimitDiameter = _filamentService.LowerLimit;

            HistoricalModel.UpperLimitDiameter = _filamentService.UpperLimit;
            HistoricalModel.NominalDiameter = _filamentService.NominalDiameter;
            HistoricalModel.LowerLimitDiameter = _filamentService.LowerLimit;

            RaisePropertyChanged("SpoolNumber");
            RaisePropertyChanged("BatchNumber");
        }

        private void ResetGraph_Click()
        {
            HistoricalModel.ResetAllAxes();
            HistoricalModel.InvalidatePlot(true);
        }

        private void StartCapture_Click()
        {
            _filamentService.CaptureStarted = true;
            RaisePropertyChanged("CaptureStarted");
            SetupRealTimeView();
            SetupHistoricalView();
            //StartHistoricalTimer();
        }

        private void StopCapture_Click()
        {
            _filamentService.SaveHistoricalData(realTimeModel.GetDataPoints());
            _filamentService.CaptureStarted = false;
            RaisePropertyChanged("CaptureStarted");

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

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Diameter");
            RaisePropertyChanged("HighestValue");
            RaisePropertyChanged("LowestValue");


            if (_filamentService.CaptureStarted)
            {
                UpdateRealTimePlot();
                //UpdateHistoricalPlot();
            }
        }

        private void UpdateRealTimePlot()
        {
            if (RealTimeModel != null)
            {

                RealTimeModel.AddDataPoint(Diameter);
                HistoricalModel.AddDataPoint(Diameter);
                UpdateHistoricalPlot();

                if (RealTimeModel.Axes.Count > 1)
                {
                    RealTimeModel.Axes[0].Zoom(DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(-5000)), DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(200)));
                    Task.Factory.StartNew(() =>
                    {
                        RealTimeModel.InvalidatePlot(true);
                    });

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

