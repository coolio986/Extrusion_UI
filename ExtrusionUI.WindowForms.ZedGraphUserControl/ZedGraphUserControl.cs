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
using ExtrusionUI.Infrastructure;

namespace ExtrusionUI.WindowForms.ZedGraphUserControl
{
    public partial class ZedGraphUserControl : UserControl
    {
        private HashSet<DataListXY> diameterList;
        private LineObj nominalLine;
        private LineObj upperLimitLine;
        private LineObj lowerLimitLine;
        private LineItem diameterCurve;
        private FilteredPointList filteredDiameter;


        public ZedGraphControl ZedGraph { get; set; }
        public bool IsZoomed { get; private set; }


        public bool AllowZoom
        {
            get { return zedGraphControl1.IsEnableWheelZoom; }
            set
            {
                zedGraphControl1.IsEnableWheelZoom = value;
                zedGraphControl1.ZoomButtons = MouseButtons.None;
                zedGraphControl1.ZoomButtons2 = MouseButtons.None;
            }
        }


        private string nominalDiameter;
        public string NominalDiameter
        {
            get { return nominalDiameter; }
            set { nominalDiameter = value; }
        }

        private string upperLimitDiameter;
        public string UpperLimitDiameter
        {
            get { return upperLimitDiameter; }
            set { upperLimitDiameter = value; }
        }

        private string lowerLimitDiameter;
        public string LowerLimitDiameter
        {
            get { return lowerLimitDiameter; }
            set { lowerLimitDiameter = value; }
        }

        public string Title
        {
            get { return zedGraphControl1.GraphPane.Title.Text; }
            set { zedGraphControl1.GraphPane.Title.Text = value; }
        }

        public bool IsHistorical { get; set; }

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public ZedGraphUserControl()
        {
            InitializeComponent();
            ZedGraph = this.zedGraphControl1;

            diameterList = new HashSet<DataListXY>();

            //add static reference lines
            AddNominalDiameter();
            AddUpperLimit();
            AddLowerLimit();

            this.zedGraphControl1.ZoomEvent += ZedGraphControl1_ZoomEvent;
        }

        private void ZedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            if (AllowZoom)
            {
                IsZoomed = true;
                // The maximum number of point to displayed is based on the width of the graphpane, and the visible range of the X axis
                if(filteredDiameter != null)
                    filteredDiameter.SetBounds(sender.GraphPane.XAxis.Scale.Min, sender.GraphPane.XAxis.Scale.Max, (int)sender.GraphPane.Rect.Width);

                ReZoomLineObjs();

                // This refreshes the graph when the button is released after a panning operation
                if (newState.Type == ZoomState.StateType.Pan)
                    sender.Invalidate();
            }
        }

        public void ClearPlots()
        {
            diameterList.Clear();
        }

        private void AddNominalDiameter()
        {
            if (diameterList != null)
                this.zedGraphControl1.GraphPane.AddCurve("Nominal Diameter", new double[0], new double[0], System.Drawing.Color.FromArgb(64, 191, 67), SymbolType.None).Line.Width = 2.0F;
        }

        private void AddUpperLimit()
        {
            this.zedGraphControl1.GraphPane.AddCurve("Upper Limit", new double[0], new double[0], System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;
        }

        private void AddLowerLimit()
        {
            this.zedGraphControl1.GraphPane.AddCurve("Lower Limit", new double[0], new double[0], System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;
        }

        public void AddDataPoint(string diameter)
        {
            //if (!stopwatch.IsRunning)
            //    stopwatch.Start();

            diameterList.Add(new DataListXY(new XDate(DateTime.Now), Convert.ToDouble(diameter)));
            //diameterList.Add(new DataListXY(stopwatch.ElapsedMilliseconds, Convert.ToDouble(diameter)));

            double[] dblTime = new double[diameterList.Count];
            double[] dblDiameter = new double[diameterList.Count];

            int index = 0;
            foreach (DataListXY item in diameterList.ToList())
            {
                if (item != null)
                {
                    dblTime[index] = item.X;
                    dblDiameter[index] = item.Y;
                    index++;
                }
            }

            if (IsHistorical)
            {
                if (!IsZoomed)
                {
                    filteredDiameter = new FilteredPointList(dblTime, dblDiameter);
                    filteredDiameter.SetBounds(this.zedGraphControl1.GraphPane.XAxis.Scale.Min, new XDate(DateTime.Now.AddMilliseconds(2)), (int)this.zedGraphControl1.GraphPane.Rect.Width);

                    this.zedGraphControl1.GraphPane.CurveList.Remove(diameterCurve);

                    diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Diameter", filteredDiameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);

                    //***********************//
                    //Adding the reference lines here updates the visual faster than adding them in a function
                    this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                    if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                    {
                        upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(upperLimitDiameter), dblTime[dblTime.Count() - 1], Convert.ToDouble(upperLimitDiameter));
                        upperLimitLine.Line.Width = 2.0F;
                        this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                    }

                    this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                    if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                    {
                        lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(lowerLimitDiameter), dblTime[dblTime.Count() - 1], Convert.ToDouble(lowerLimitDiameter));
                        lowerLimitLine.Line.Width = 2.0F;
                        this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                    }

                    this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                    if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                    {
                        nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(nominalDiameter), dblTime[dblTime.Count() - 1], Convert.ToDouble(nominalDiameter));
                        nominalLine.Line.Width = 2.0F;
                        this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
                    }
                    //***********************//

                }
            }
            else
            {
                filteredDiameter = new FilteredPointList(dblTime, dblDiameter);
                filteredDiameter.SetBounds(new XDate(DateTime.Now.AddMilliseconds(-5000)), new XDate(DateTime.Now.AddMilliseconds(2)), (int)this.zedGraphControl1.GraphPane.Rect.Width);

                this.zedGraphControl1.GraphPane.CurveList.Remove(diameterCurve);

                diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Diameter", filteredDiameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);


                //***********************//
                //Adding the reference lines here updates the visual faster than adding them in a function
                this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                {
                    upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), new XDate(DateTime.Now.AddMilliseconds(-5000)), Convert.ToDouble(upperLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(upperLimitDiameter));
                    upperLimitLine.Line.Width = 2.0F;
                    this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                }

                this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                {
                    lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), new XDate(DateTime.Now.AddMilliseconds(-5000)), Convert.ToDouble(lowerLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(lowerLimitDiameter));
                    lowerLimitLine.Line.Width = 2.0F;
                    this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                }

                this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                {
                    nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), new XDate(DateTime.Now.AddMilliseconds(-5000)), Convert.ToDouble(nominalDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(nominalDiameter));
                    nominalLine.Line.Width = 2.0F;
                    this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
                }
                //***********************//

            }
        }

        public void ZoomOutAll()
        {
            this.zedGraphControl1.ZoomOutAll(this.zedGraphControl1.GraphPane);
            this.zedGraphControl1.Refresh();
            IsZoomed = false;
        }

        public HashSet<DataListXY> GetDataPoints()
        {
            return diameterList;
        }

        public void ReZoomLineObjs()
        {
            this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
            {
                upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(upperLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(upperLimitDiameter));
                upperLimitLine.Line.Width = 2.0F;
                this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
            }

            this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
            {
                lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(lowerLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(lowerLimitDiameter));
                lowerLimitLine.Line.Width = 2.0F;
                this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
            }

            this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
            {

                nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(nominalDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(nominalDiameter));
                nominalLine.Line.Width = 2.0F;
                this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
            }
        }
    }
}
