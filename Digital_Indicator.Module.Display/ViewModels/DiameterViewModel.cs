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

        public DiameterViewModel(IFilamentService filamentService)
        {
            _filamentService = filamentService;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            SetupPlots();

            ResetGraph = new DelegateCommand(ResetGraph_Click);
            StartCapture = new DelegateCommand(StartCapture_Click);
            StopCapture = new DelegateCommand(StopCapture_Click);
            Settings = new DelegateCommand(Settings_Click);
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
            SetupPlots();
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

        private void SetupPlots()
        {
            LinearSeriesPlotModel.CreatePlots(_filamentService.UpperLimit, _filamentService.NominalDiameter, _filamentService.LowerLimit);
            RealTimeModel = LinearSeriesPlotModel.GetPlot("RealTimeModel");
            HistoricalModel = LinearSeriesPlotModel.GetPlot("HistoricalModel");
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Diameter");
            RaisePropertyChanged("HighestValue");
            RaisePropertyChanged("LowestValue");


            if (_filamentService.CaptureStarted)
            {
                UpdatePlots();
            }
        }

        private void UpdatePlots()
        {
            if (RealTimeModel != null && HistoricalModel != null)
            {
                RealTimeModel.AddDataPoint(Diameter);
                HistoricalModel.AddDataPoint(Diameter);
            }
        }
    }
}

