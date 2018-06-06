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

namespace Digital_Indicator.WindowForms.ZedGraphUserControl
{
    public partial class ZedGraphUserControl : UserControl
    {
        public ZedGraphControl ZedGraph { get; set; }

        private PointPairList diameterList;
        private PointPairList upperLimitList;
        private PointPairList lowerLimitList;

        

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

        public ZedGraphUserControl()
        {
            InitializeComponent();
            ZedGraph = this.zedGraphControl1;
            diameterList = new PointPairList();
            upperLimitList = new PointPairList();
            lowerLimitList = new PointPairList();
        }

        public void ClearPlots()
        {
            diameterList.Clear();
            upperLimitList.Clear();
            lowerLimitList.Clear();
        }

        private void AddCurve()
        {
            if (diameterList != null)
                this.zedGraphControl1.GraphPane.AddCurve("Diameter", diameterList, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);
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
    }
}
