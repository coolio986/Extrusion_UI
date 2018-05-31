using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Infrastructure.UI
{
    public class LinearSeriesPlotModel : PlotModel
    {
        private LineSeries lineSeries;
        private IList<DataPoint> diameterPoints;
        private IList<DataPoint> diameterReferenceNominal;
        private IList<DataPoint> diameterReferenceUpperLimit;
        private IList<DataPoint> diameterReferenceLowerLimit;

        private LineSeries NominalDiameterLineSeries;
        private LineSeries UpperLimitDiameterLineSeries;
        private LineSeries LowerLimitDiameterLineSeries;

        private Stopwatch updateTime;
        private long previousMillis;

        private bool updateSlow;
        public bool UpdateSlow
        {
            get { return updateSlow; }
            set
            {
                updateSlow = value;
                if (value)
                {
                    updateTime.Start();
                    previousMillis = updateTime.ElapsedMilliseconds;
                }
            }
        }

        public LinearSeriesPlotModel()
        {
            lineSeries = new LineSeries();
            diameterPoints = new List<DataPoint>();
            diameterReferenceNominal = new List<DataPoint>();
            diameterReferenceUpperLimit = new List<DataPoint>();
            diameterReferenceLowerLimit = new List<DataPoint>();

            updateTime = new Stopwatch();

            this.Series.Add(GetDiameterPointsLineSeries());

            this.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            this.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 1.60, });
        }

        private string nominalDiameter;
        public string NominalDiameter
        {
            get { return nominalDiameter; }
            set
            {
                if (NominalDiameterLineSeries != null)
                    this.Series.Remove(NominalDiameterLineSeries);

                nominalDiameter = value;
                NominalDiameterLineSeries = GetNominalDiameterLineSeries();
                this.Series.Add(NominalDiameterLineSeries);
            }
        }

        private string upperLimitDiameter;
        public string UpperLimitDiameter
        {
            get { return upperLimitDiameter; }
            set
            {
                if (UpperLimitDiameterLineSeries != null)
                    this.Series.Remove(UpperLimitDiameterLineSeries);

                upperLimitDiameter = value;
                UpperLimitDiameterLineSeries = GetUpperLimitDiameterLineSeries();
                this.Series.Add(UpperLimitDiameterLineSeries);
            }
        }

        private string lowerLimitDiameter;
        public string LowerLimitDiameter
        {
            get { return lowerLimitDiameter; }
            set
            {
                if (LowerLimitDiameterLineSeries != null)
                    this.Series.Remove(LowerLimitDiameterLineSeries);

                lowerLimitDiameter = value;
                LowerLimitDiameterLineSeries = GetLowerLimitDiameterLineSeries();
                this.Series.Add(LowerLimitDiameterLineSeries);
            }
        }

        public void AddDataPoint(string diameterToAdd)
        {
            diameterPoints.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(diameterToAdd)));
            diameterReferenceUpperLimit.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(upperLimitDiameter)));
            diameterReferenceNominal.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(nominalDiameter)));
            diameterReferenceLowerLimit.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(lowerLimitDiameter)));

            if (UpdateSlow)
            {
                if (updateTime.ElapsedMilliseconds >= previousMillis + 5000)
                {
                    Task.Factory.StartNew(() =>
                    {
                        this.InvalidatePlot(true);
                        previousMillis = updateTime.ElapsedMilliseconds;
                    });
                }
            }
            else
            {
                if (this.Axes.Count > 1)
                {
                    this.Axes[0].Zoom(DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(-5000)), DateTimeAxis.ToDouble(DateTime.Now.AddMilliseconds(200)));
                    Task.Factory.StartNew(() =>
                    {
                        this.InvalidatePlot(true);
                    });
                }
            }
        }

        public List<DataListXY> GetDataPoints()
        {
            List<DataListXY> dataList = new List<DataListXY>();
            foreach (DataPoint dataPoint in diameterPoints)
            {
                dataList.Add(new DataListXY(dataPoint.X, dataPoint.Y));
            }
            return dataList;
        }

        private LineSeries GetDiameterPointsLineSeries()
        {
            var lineSeries = new LineSeries();
            //lineSeries.Title = nominalDiameter;
            lineSeries.StrokeThickness = 2;
            lineSeries.ItemsSource = diameterPoints;
            lineSeries.Color = OxyColor.FromRgb(0, 153, 255);

            return lineSeries;
        }

        private LineSeries GetNominalDiameterLineSeries()
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = nominalDiameter;
            lineSeries.StrokeThickness = 2;
            lineSeries.ItemsSource = diameterReferenceNominal;
            lineSeries.Color = OxyColor.FromRgb(64, 191, 67);

            return lineSeries;
        }

        private LineSeries GetUpperLimitDiameterLineSeries()
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = upperLimitDiameter;
            lineSeries.StrokeThickness = 1;
            lineSeries.ItemsSource = diameterReferenceUpperLimit;
            lineSeries.Color = OxyColor.FromRgb(255, 0, 0);

            return lineSeries;
        }

        private LineSeries GetLowerLimitDiameterLineSeries()
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = lowerLimitDiameter;
            lineSeries.StrokeThickness = 1;
            lineSeries.ItemsSource = diameterReferenceLowerLimit;
            lineSeries.Color = OxyColor.FromRgb(255, 0, 0);

            return lineSeries;
        }

        static Dictionary<string, LinearSeriesPlotModel> plotModelDict;

        public static LinearSeriesPlotModel GetPlot(string plotName)
        {
            return plotModelDict[plotName];
        }

        public static List<LinearSeriesPlotModel> GetPlots()
        {
            return plotModelDict.Select(x => { return x.Value; }).ToList();
        }
        public static void CreatePlots(string upperLimit, string nominalDiameter, string lowerLimit)
        {
            if (plotModelDict != null)
            {
                plotModelDict.Select(x =>
                {
                    x.Value.diameterPoints.Clear();
                    x.Value.diameterReferenceNominal.Clear();
                    x.Value.diameterReferenceUpperLimit.Clear();
                    x.Value.diameterReferenceLowerLimit.Clear();
                    x.Value.InvalidatePlot(true);
                    return x;
                }).ToList();
            }
            else
            {
                plotModelDict = new Dictionary<string, LinearSeriesPlotModel>();

                //plotModelDict = new Dictionary<string, LinearSeriesPlotModel>();
                plotModelDict.Add("HistoricalModel",
                 new LinearSeriesPlotModel()
                 {
                     Title = "Historical Diameter",
                     UpperLimitDiameter = upperLimit,
                     NominalDiameter = nominalDiameter,
                     LowerLimitDiameter = lowerLimit,
                     UpdateSlow = true,
                 }
                );

                plotModelDict.Add("RealTimeModel",
                 new LinearSeriesPlotModel()
                 {
                     Title = "RealTime Diameter",
                     UpperLimitDiameter = upperLimit,
                     NominalDiameter = nominalDiameter,
                     LowerLimitDiameter = lowerLimit,
                 }
                );
            }
        }
    }
}
