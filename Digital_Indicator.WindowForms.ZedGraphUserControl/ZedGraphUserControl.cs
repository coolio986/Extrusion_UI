using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using Digital_Indicator.Infrastructure;

namespace Digital_Indicator.WindowForms.ZedGraphUserControl
{
    public partial class ZedGraphUserControl : UserControl
    {
        public ZedGraphControl ZedGraph { get; set; }

        private PointPairList diameterList;
        private PointPairList nominalDiameterList;
        private PointPairList upperLimitList;
        private PointPairList lowerLimitList;

        private LineItem diameterCurve;

        private FilteredPointList filteredDiameter;

        int counter;

        private string nominalDiameter;
        public string NominalDiameter
        {
            get { return nominalDiameter; }
            set
            {
                //if (NominalDiameterLineSeries != null)
                //    this.Series.Remove(NominalDiameterLineSeries);

                nominalDiameter = value;
                //NominalDiameterLineSeries = GetNominalDiameterLineSeries();
                //this.Series.Add(NominalDiameterLineSeries);
            }
        }

        private string upperLimitDiameter;
        public string UpperLimitDiameter
        {
            get { return upperLimitDiameter; }
            set
            {
                //if (UpperLimitDiameterLineSeries != null)
                //    this.Series.Remove(UpperLimitDiameterLineSeries);

                upperLimitDiameter = value;
                //UpperLimitDiameterLineSeries = GetUpperLimitDiameterLineSeries();
                //this.Series.Add(UpperLimitDiameterLineSeries);
            }
        }

        private string lowerLimitDiameter;
        public string LowerLimitDiameter
        {
            get { return lowerLimitDiameter; }
            set
            {
                //if (LowerLimitDiameterLineSeries != null)
                //    this.Series.Remove(LowerLimitDiameterLineSeries);

                lowerLimitDiameter = value;
                //LowerLimitDiameterLineSeries = GetLowerLimitDiameterLineSeries();
                //this.Series.Add(LowerLimitDiameterLineSeries);
            }
        }

        public string Title
        {
            get { return zedGraphControl1.GraphPane.Title.Text; }
            set { zedGraphControl1.GraphPane.Title.Text = value; }
        }

        public bool IsHistorical { get; set; }

        public ZedGraphUserControl()
        {
            InitializeComponent();
            ZedGraph = this.zedGraphControl1;
            diameterList = new PointPairList();
            nominalDiameterList = new PointPairList();
            upperLimitList = new PointPairList();
            lowerLimitList = new PointPairList();

            AddDiameter();
            AddNominalDiameter();
            AddUpperLimit();
            AddLowerLimit();

            counter = 0;
        }

        public void ClearPlots()
        {
            diameterList.Clear();
            upperLimitList.Clear();
            lowerLimitList.Clear();
            nominalDiameterList.Clear();
        }

        private void AddDiameter()
        {
            if (diameterList != null && !IsHistorical)
            {
                diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Diameter", diameterList, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);
            }
        }

        private void AddNominalDiameter()
        {
            if (diameterList != null)
                this.zedGraphControl1.GraphPane.AddCurve("Nominal Diameter", nominalDiameterList, System.Drawing.Color.FromArgb(64, 191, 67), SymbolType.None).Line.Width = 2.0F;
        }

        private void AddUpperLimit()
        {
            if (upperLimitList != null)
                this.zedGraphControl1.GraphPane.AddCurve("Upper Limit", upperLimitList, System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;
        }

        private void AddLowerLimit()
        {
            if (lowerLimitList != null)
                this.zedGraphControl1.GraphPane.AddCurve("Lower Limit", lowerLimitList, System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;
        }

        public void AddDataPoint(string diameter)
        {
            diameterList.Add(new XDate(DateTime.Now), Convert.ToDouble(diameter));
            nominalDiameterList.Add(new XDate(DateTime.Now), Convert.ToDouble(nominalDiameter));
            upperLimitList.Add(new XDate(DateTime.Now), Convert.ToDouble(upperLimitDiameter));
            lowerLimitList.Add(new XDate(DateTime.Now), Convert.ToDouble(lowerLimitDiameter));

            if (counter == 5000 && IsHistorical)
            {
                double[] dlbtime = new double[diameterList.Count];
                double[] dbldiameter = new double[diameterList.Count];

                for (int i = 0; i < diameterList.Count; i++)
                {
                    dlbtime[i] = diameterList[i].X;
                    dbldiameter[i] = diameterList[i].Y;
                }
                filteredDiameter = new FilteredPointList(dlbtime, dbldiameter);
                filteredDiameter.SetBounds(0, 1e20, 5000);

                this.zedGraphControl1.GraphPane.CurveList.Remove(diameterCurve);

                diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Diameter", filteredDiameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);

                counter = 0;

            }


            ZedGraph.AxisChange();

            counter++;
        }

        public void ZoomOutAll()
        {
            this.zedGraphControl1.ZoomOutAll(this.zedGraphControl1.GraphPane);
        }

        public HashSet<DataListXY> GetDataPoints()
        {
            HashSet<DataListXY> dataList = new HashSet<DataListXY>();
            foreach (PointPair dataPoint in diameterList)
            {
                dataList.Add(new DataListXY(dataPoint.X, dataPoint.Y));
            }
            return dataList;
        }
    }
}
