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
        private LineSeries lineSeries;
        private IList<DataPoint> diameterPoints;
        private IList<DataPoint> diameterReferenceNominal;
        private IList<DataPoint> diameterReferenceUpperLimit;
        private IList<DataPoint> diameterReferenceLowerLimit;

        private LineSeries NominalDiameterLineSeries;
        private LineSeries UpperLimitDiameterLineSeries;
        private LineSeries LowerLimitDiameterLineSeries;

        public LinearSeriesPlotModel()
        {
            lineSeries = new LineSeries();
            diameterPoints = new List<DataPoint>();
            diameterReferenceNominal = new List<DataPoint>();
            diameterReferenceUpperLimit = new List<DataPoint>();
            diameterReferenceLowerLimit = new List<DataPoint>();

            this.Series.Add(GetDiameterPointsLineSeries());

            this.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "hh:mm:ss" });
            this.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 1.65, });
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
