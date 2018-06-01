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
using System.Windows.Threading;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {
        public IFilamentService _filamentService { get; }
        public DelegateCommand ResetGraph { get; private set; }
        public DelegateCommand StartCapture { get; private set; }
        public DelegateCommand StopCapture { get; private set; }
        public DelegateCommand Settings { get; private set; }

        public LinearSeriesPlotModel RealTimeModel
        {
            get { return LinearSeriesPlotModel.GetPlot("RealTimeModel"); }
        }

        public LinearSeriesPlotModel HistoricalModel
        {
            get { return LinearSeriesPlotModel.GetPlot("HistoricalModel"); }
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
            private set { SetProperty(ref settingsView, value); }
        }

        public DiameterViewModel(IFilamentService filamentService)
        {
            _filamentService = filamentService;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            ResetGraph = new DelegateCommand(ResetGraph_Click);
            StartCapture = new DelegateCommand(StartCapture_Click);
            StopCapture = new DelegateCommand(StopCapture_Click);
            Settings = new DelegateCommand(Settings_Click);
        }

        private void _filamentService_PropertyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SpoolNumber");
            RaisePropertyChanged("BatchNumber");
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {
            //RaisePropertyChanged("Diameter");
            //RaisePropertyChanged("HighestValue");
            //RaisePropertyChanged("LowestValue");
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
            RaisePropertyChanged("RealTimeModel");
        }

        private void StopCapture_Click()
        {
            _filamentService.CaptureStarted = false;
            RaisePropertyChanged("CaptureStarted");
        }

        private void Settings_Click()
        {
            SettingsView = new SettingsView();
        }
    }
}

