using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Infrastructure.UI
{
    public class LinearSeriesPlotModel : PlotModel
    {
        //private PlotModel plotModel;
        private LineSeries lineSeries;
        private IList<DataPoint> diameterPoints;
        private IList<DataPoint> diameterReferenceNominal;
        private IList<DataPoint> diameterReferenceUpperLimit;
        private IList<DataPoint> diameterReferenceLowerLimit;

        public LinearSeriesPlotModel()
        {
            lineSeries = new LineSeries();
            //plotModel = new PlotModel();
            diameterPoints = new List<DataPoint>();
            diameterReferenceNominal = new List<DataPoint>();
            diameterReferenceUpperLimit = new List<DataPoint>();
            diameterReferenceLowerLimit = new List<DataPoint>();


            //var lineSeriesDiameterData = new LineSeries();
            //lineSeriesDiameterData.ItemsSource = DiameterPoints;
            //lineSeriesDiameterData.Color = OxyColor.FromRgb(0, 153, 255);

            //plotModel = new PlotModel { Title = "Realtime Diameter" };
            //plotModel.Series.Add(lineSeriesDiameterData);
            this.Series.Add(GetDiameterPointsLineSeries());

            this.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            this.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 1.5, });

            //this.Series.Add(GetUpperLimitDiameterLineSeries());
            //this.Series.Add(GetNominalDiameterLineSeries());
            //this.Series.Add(GetLowerLimitDiameterLineSeries());

        }

        private string nominalDiameter;
        public string NominalDiameter
        {
            get { return nominalDiameter; }
            set
            {
                nominalDiameter = value;
                this.Series.Add(GetNominalDiameterLineSeries());
            }
        }

        private string upperLimitDiameter;
        public string UpperLimitDiameter
        {
            get { return upperLimitDiameter; }
            set
            {
                upperLimitDiameter = value;
                this.Series.Add(GetUpperLimitDiameterLineSeries());
            }
        }

        private string lowerLimitDiameter;
        public string LowerLimitDiameter
        {
            get { return lowerLimitDiameter; }
            set
            {
                lowerLimitDiameter = value;
                this.Series.Add(GetLowerLimitDiameterLineSeries());
            }
        }

        public void AddDataPoint(string diameterToAdd)
        {
            diameterPoints.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(diameterToAdd)));
            diameterReferenceNominal.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(nominalDiameter)));
            diameterReferenceUpperLimit.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(upperLimitDiameter)));
            diameterReferenceLowerLimit.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), Convert.ToDouble(lowerLimitDiameter)));
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
    }

}
