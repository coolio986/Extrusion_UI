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

        public ObservableCollection<string> SerialPortList { get; }

        private IList<DataPoint> DiameterPoints { get; set; }

        private IList<DataPoint> DiameterReference { get; set; }

        public PlotModel RealTimeModel { get; private set; }

        public PlotModel HistoricalModel { get; private set; }

        private string diameter;
        public string Diameter
        {
            get { return diameter; }
            set { SetProperty(ref diameter, value); }
        }

        private double? highestValue;
        public double? HighestValue
        {
            get { return highestValue; }
            set { SetProperty(ref highestValue, value); }
        }

        private double? lowestValue;
        public double? LowestValue
        {
            get { return lowestValue; }
            set { SetProperty(ref lowestValue, value); }
        }

        public DiameterViewModel(ISerialService serialService)
        {
            _serialService = serialService;
            _serialService.DiameterChanged += _serialService_DiameterChanged;

            SerialPortList = new ObservableCollection<string>(_serialService.GetSerialPortList());
            DiameterPoints = new List<DataPoint>();
            DiameterReference = new List<DataPoint>();


            SetupRealTimeView();
            SetupHistoricalView();
            StartHistoricalTimer();

            ResetGraph = new DelegateCommand(ResetGraph_Click);

            //************TEST DATA**********//
            _serialService.ConnectToSerialPort("COM3");
            //*******************************//

        }

        private void ResetGraph_Click()
        {
            HistoricalModel.ResetAllAxes();
        }

        private void SetupRealTimeView()
        {
            var lineSeriesDiameterData = new LineSeries();
            lineSeriesDiameterData.ItemsSource = DiameterPoints;
            lineSeriesDiameterData.Color = OxyColor.FromRgb(0, 153, 255);

            this.RealTimeModel = new PlotModel { Title = "Realtime Diameter" };
            this.RealTimeModel.Series.Add(lineSeriesDiameterData);

            RealTimeModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            RealTimeModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = .5, });

            this.RealTimeModel.Series.Add(GetNominalDiameter());
        }

        private void SetupHistoricalView()
        {
            var lineSeriesDiameterData = new LineSeries();
            lineSeriesDiameterData.ItemsSource = DiameterPoints;
            lineSeriesDiameterData.Color = OxyColor.FromRgb(0, 153, 255);

            this.HistoricalModel = new PlotModel { Title = "Historical Diameter" };
            this.HistoricalModel.Series.Add(lineSeriesDiameterData);

            HistoricalModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            HistoricalModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = .5, });

            this.HistoricalModel.Series.Add(GetNominalDiameter());
        }

        private LineSeries GetNominalDiameter()
        {
            var lineSeriesNominal = new LineSeries();
            lineSeriesNominal.Title = "1.75";
            lineSeriesNominal.StrokeThickness = 1;
            lineSeriesNominal.ItemsSource = DiameterReference;
            lineSeriesNominal.Color = OxyColor.FromRgb(255, 0, 0);

            return lineSeriesNominal;
        }

        private void StartHistoricalTimer()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    HistoricalModel.InvalidatePlot(true);
                    Thread.Sleep(5000);
                }
            });
        }

        private void _serialService_DiameterChanged(object sender, EventArgs e)
        {
            Diameter = sender.ToString();

            UpdateRealTimePlot();
            UpdateHighsAndLows();
        }

        private void UpdateRealTimePlot()
        {
            if (RealTimeModel != null)
            {
                this.DiameterPoints.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(Diameter)));
                this.DiameterReference.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 1.75));

                if (RealTimeModel.Axes.Count > 1)
                {
                    RealTimeModel.Axes[0].Zoom(DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(-5000)), DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(200)));
                    RealTimeModel.InvalidatePlot(true);

                }
            }
        }

        private void UpdateHighsAndLows()
        {
            HighestValue = highestValue == null ? (double)diameter.GetDouble() : highestValue.GetDouble() < diameter.GetDouble() ? diameter.GetDouble() : highestValue;
            LowestValue = lowestValue == null ? (double)diameter.GetDouble() : lowestValue.GetDouble() > diameter.GetDouble() ? diameter.GetDouble() : lowestValue;
        }
    }
}
