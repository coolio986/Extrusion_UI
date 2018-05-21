using Digital_Indicator.Logic.SerialCommunications;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Digital_Indicator.Logic.Helpers;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class DiameterViewModel : BindableBase
    {
        private ISerialService _serialService;
        public DelegateCommand ResetGraph { get; set; }
        public DelegateCommand StartCapture { get; set; }
        public DelegateCommand StopCapture { get; set; }

        private IList<DataPoint> DiameterPoints { get; set; }

        private IList<DataPoint> DiameterReferenceNominal { get; set; }

        private IList<DataPoint> DiameterReferenceUpperLimit { get; set; }

        private IList<DataPoint> DiameterReferenceLowerLimit { get; set; }

        private PlotModel realTimeModel;
        public PlotModel RealTimeModel
        {
            get { return realTimeModel; }
            private set { SetProperty(ref realTimeModel, value); }
        }

        private PlotModel historicalModel;
        public PlotModel HistoricalModel
        {
            get { return historicalModel; }
            set { SetProperty(ref historicalModel, value); }
        }

        private bool IsStarted;


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

        public DiameterViewModel(ISerialService serialService)
        {
            //TODO Move Plot data to service
            _serialService = serialService;
            _serialService.DiameterChanged += _serialService_DiameterChanged;

            SetupDataPoints();
            SetupRealTimeView();
            SetupHistoricalView();
            StartHistoricalTimer();

            ResetGraph = new DelegateCommand(ResetGraph_Click);
            StartCapture = new DelegateCommand(StartCapture_Click);
            StopCapture = new DelegateCommand(StopCapture_Click);
        }

        private void ResetGraph_Click()
        {
            HistoricalModel.ResetAllAxes();
            HistoricalModel.InvalidatePlot(true);
        }

        private void StartCapture_Click()
        {
            IsStarted = true;
            SetupDataPoints();
            SetupRealTimeView();
            SetupHistoricalView();
            StartHistoricalTimer();
        }

        private void StopCapture_Click()
        {
            IsStarted = false;
            
        }

        private void SetupDataPoints()
        {
            DiameterPoints = new List<DataPoint>();
            DiameterReferenceNominal = new List<DataPoint>();
            DiameterReferenceUpperLimit = new List<DataPoint>();
            DiameterReferenceLowerLimit = new List<DataPoint>();
        }

        private void SetupRealTimeView()
        {
            var lineSeriesDiameterData = new LineSeries();
            lineSeriesDiameterData.ItemsSource = DiameterPoints;
            lineSeriesDiameterData.Color = OxyColor.FromRgb(0, 153, 255);

            this.RealTimeModel = new PlotModel { Title = "Realtime Diameter" };
            this.RealTimeModel.Series.Add(lineSeriesDiameterData);

            this.RealTimeModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            this.RealTimeModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 1.5, });

            this.RealTimeModel.Series.Add(GetUpperLimitDiameter());
            this.RealTimeModel.Series.Add(GetNominalDiameter());
            this.RealTimeModel.Series.Add(GetLowerLimitDiameter());
        }

        private void SetupHistoricalView()
        {
            var lineSeriesDiameterData = new LineSeries();
            lineSeriesDiameterData.ItemsSource = DiameterPoints;
            lineSeriesDiameterData.Color = OxyColor.FromRgb(0, 153, 255);

            this.HistoricalModel = new PlotModel { Title = "Historical Diameter" };
            this.HistoricalModel.Series.Add(lineSeriesDiameterData);

            HistoricalModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            HistoricalModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 1.5, });
            
            this.HistoricalModel.Series.Add(GetUpperLimitDiameter());
            this.HistoricalModel.Series.Add(GetNominalDiameter());
            this.HistoricalModel.Series.Add(GetLowerLimitDiameter());
        }

        private LineSeries GetNominalDiameter()
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = "1.75";
            lineSeries.StrokeThickness = 2;
            lineSeries.ItemsSource = DiameterReferenceNominal;
            lineSeries.Color = OxyColor.FromRgb(64, 191, 67);

            return lineSeries;
        }

        private LineSeries GetUpperLimitDiameter()
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = "1.80";
            lineSeries.StrokeThickness = 1;
            lineSeries.ItemsSource = DiameterReferenceUpperLimit;
            lineSeries.Color = OxyColor.FromRgb(255, 0, 0);

            return lineSeries;
        }

        private LineSeries GetLowerLimitDiameter()
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = "1.70";
            lineSeries.StrokeThickness = 1;
            lineSeries.ItemsSource = DiameterReferenceLowerLimit;
            lineSeries.Color = OxyColor.FromRgb(255, 0, 0);

            return lineSeries;
        }

        private void StartHistoricalTimer()
        {
            //acts as a timer
            Task.Factory.StartNew(() =>
            {
                while (IsStarted)
                {
                    HistoricalModel.InvalidatePlot(true);
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
                UpdateHighsAndLows();
            }
        }

        private void UpdateRealTimePlot()
        {
            if (RealTimeModel != null)
            {
                this.DiameterPoints.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(Diameter)));
                this.DiameterReferenceNominal.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 1.75));
                this.DiameterReferenceUpperLimit.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 1.80));
                this.DiameterReferenceLowerLimit.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 1.70));

                if (RealTimeModel.Axes.Count > 1)
                {
                    RealTimeModel.Axes[0].Zoom(DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(-5000)), DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(200)));
                    RealTimeModel.InvalidatePlot(true);
                }
            }
        }

        private void UpdateHighsAndLows()
        {
            HighestValue = highestValue == null ? diameter : highestValue.GetDouble() < diameter.GetDouble() ? diameter : highestValue;
            LowestValue = lowestValue == null ? diameter : lowestValue.GetDouble() > diameter.GetDouble() ? diameter : lowestValue;
        }
    }
}

