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

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {
        private ISerialService _serialService;
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

        private string diameter;
        public string Diameter
        {
            get { return diameter; }
            set { SetProperty(ref diameter, value); }
        }

        private string highestValue;
        public string HighestValue
        {
            get { return highestValue; }
            set { SetProperty(ref highestValue, value); }
        }

        private string lowestValue;
        public string LowestValue
        {
            get { return lowestValue; }
            set { SetProperty(ref lowestValue, value); }
        }

        private object settingsView;
        public object SettingsView
        {
            get { return settingsView; }
            set { SetProperty(ref settingsView, value); }
        }

        Stopwatch stopWatch;
        long previousMillis;

        public DiameterViewModel(ISerialService serialService)
        {
            _serialService = serialService;
            _serialService.DiameterChanged += _serialService_DiameterChanged;

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

        private void ResetGraph_Click()
        {
            HistoricalModel.ResetAllAxes();
            HistoricalModel.InvalidatePlot(true);
        }

        private bool IsStarted;
        private void StartCapture_Click()
        {
            IsStarted = true;
            SetupRealTimeView();
            SetupHistoricalView();
            StartHistoricalTimer();
        }

        private void StopCapture_Click()
        {
            IsStarted = false;
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
                UpperLimitDiameter = "1.80",
                NominalDiameter = "1.75",
                LowerLimitDiameter = "1.70",
            };
        }

        private void SetupHistoricalView()
        {
            HistoricalModel = new LinearSeriesPlotModel()
            {
                Title = "Historical Diameter",
                UpperLimitDiameter = "1.80",
                NominalDiameter = "1.75",
                LowerLimitDiameter = "1.70",
            };
        }

        private void StartHistoricalTimer()
        {
            //acts as a timer
            Task.Factory.StartNew(() =>
            {
                while (IsStarted)
                {
                    //Update plot on main UI thread, prevents cross thread violations
                    //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    HistoricalModel.InvalidatePlot(true);
                    //}));

                    //HistoricalModel.InvalidatePlot(true);

                    Thread.Sleep(5000);
                }
            });
        }

        private void _serialService_DiameterChanged(object sender, EventArgs e)
        {
            Diameter = sender.ToString();

            if (IsStarted)
            {
                UpdateRealTimePlot();
                UpdateHistoricalPlot();
                UpdateHighsAndLows();
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

        private void UpdateHighsAndLows()
        {
            HighestValue = highestValue == null ? diameter : highestValue.GetDouble() < diameter.GetDouble() ? diameter : highestValue;
            LowestValue = lowestValue == null ? diameter : lowestValue.GetDouble() > diameter.GetDouble() ? diameter : lowestValue;
        }
    }
}

