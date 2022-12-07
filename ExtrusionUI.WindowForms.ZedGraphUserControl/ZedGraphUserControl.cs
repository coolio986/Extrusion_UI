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

        //private double[] dblTime = new double[1];
        //private double[] dblDiameter = new double[1];
        ResizableArray<double> resizableArrayDateTime = new ResizableArray<double>();
        ResizableArray<double> resizableArrayDiameter = new ResizableArray<double>();


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
                //IsZoomed = true;
                IsZoomed = sender.GraphPane.ZoomStack.Count > 1;
                // The maximum number of point to displayed is based on the width of the graphpane, and the visible range of the X axis
                if (filteredDiameter != null)
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
            resizableArrayDateTime = new ResizableArray<double>();
            resizableArrayDiameter = new ResizableArray<double>();
            
            this.zedGraphControl1.GraphPane.CurveList.Remove(diameterCurve);
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
            DateTime dateTime = DateTime.Now;

            double dblDateTime = new XDate(dateTime);
            double dblDiameter = Convert.ToDouble(diameter);
            //diameterList.Add(new DataListXY(stopwatch.ElapsedMilliseconds, Convert.ToDouble(diameter)));

            int timeLength = diameterList.Count;
            
            diameterList.Add(new DataListXY(dblDateTime, dblDiameter));

            resizableArrayDateTime.Add(dblDateTime);
            resizableArrayDiameter.Add(dblDiameter);

            double dblLowerLimitDiameter = Convert.ToDouble(lowerLimitDiameter) - 0.025;
            double dblUpperLimitDiameter = Convert.ToDouble(upperLimitDiameter) + 0.025;

            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min != dblLowerLimitDiameter)
                this.zedGraphControl1.GraphPane.YAxis.Scale.Min = dblLowerLimitDiameter;

            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max != dblUpperLimitDiameter)
                this.zedGraphControl1.GraphPane.YAxis.Scale.Max = dblUpperLimitDiameter;

            if (IsHistorical)
            {
                if (!IsZoomed)
                {
                    //sometimes zedgraph gets an invalid index while starting. Need to investigate
                    //for now just skip and continue
                    try
                    {

                        filteredDiameter = new FilteredPointList(resizableArrayDateTime.InternalArray.Where(x => x != 0).ToArray(), resizableArrayDiameter.InternalArray.Where(y => y != 0).ToArray());
                        filteredDiameter.SetBounds(this.zedGraphControl1.GraphPane.XAxis.Scale.Min, new XDate(DateTime.Now.AddMilliseconds(2)), (int)this.zedGraphControl1.GraphPane.Rect.Width);


                        this.zedGraphControl1.GraphPane.CurveList.Remove(diameterCurve);

                        diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Diameter", filteredDiameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);

                        //***********************//
                        //Adding the reference lines here updates the visual faster than adding them in a function
                        this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                        if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                        {
                            upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(upperLimitDiameter), resizableArrayDateTime.InternalArray[resizableArrayDateTime.Count - 1], Convert.ToDouble(upperLimitDiameter));
                            upperLimitLine.Line.Width = 2.0F;
                            this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                        }

                        this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                        if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                        {
                            lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(lowerLimitDiameter), resizableArrayDateTime.InternalArray[resizableArrayDateTime.Count - 1], Convert.ToDouble(lowerLimitDiameter));
                            lowerLimitLine.Line.Width = 2.0F;
                            this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                        }

                        this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                        if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                        {
                            nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(nominalDiameter), resizableArrayDateTime.InternalArray[resizableArrayDateTime.Count - 1], Convert.ToDouble(nominalDiameter));
                            nominalLine.Line.Width = 2.0F;
                            this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
                        }
                    }
                    catch(Exception oe)
                    {
                        
                    }
                    //***********************//

                }
            }
            else
            {
                double lowerBoundTime = new XDate(dateTime.AddMilliseconds(-5000));
                //double higherBoundTime = new XDate(dateTime.AddMilliseconds(2));
                double higherBoundTime = new XDate(dateTime.AddMilliseconds(2));


                ResizableArray<double> realTimeGraphTimes = new ResizableArray<double>();
                ResizableArray<double> realTimeDiameters = new ResizableArray<double>();


                for (int i = resizableArrayDateTime.Count - 1; i > 0; i--)
                {
                    if (resizableArrayDateTime.InternalArray[i] > lowerBoundTime)
                    {
                        realTimeGraphTimes.Add(resizableArrayDateTime.InternalArray[i]);
                        realTimeDiameters.Add(resizableArrayDiameter.InternalArray[i]);
                    }
                    else
                        break;
                }

                filteredDiameter = new FilteredPointList(realTimeGraphTimes.InternalArray, realTimeDiameters.InternalArray);
                filteredDiameter.SetBounds(lowerBoundTime, higherBoundTime, (int)this.zedGraphControl1.GraphPane.Rect.Width);

                this.zedGraphControl1.GraphPane.CurveList.Remove(diameterCurve);

                diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Diameter", filteredDiameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);

                

                zedGraphControl1.GraphPane.XAxis.Scale.Min = lowerBoundTime;
                zedGraphControl1.GraphPane.XAxis.Scale.Max = higherBoundTime;


                //***********************//
                //Adding the reference lines here updates the visual faster than adding them in a function
                this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                {
                    upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), lowerBoundTime, Convert.ToDouble(upperLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(upperLimitDiameter));
                    upperLimitLine.Line.Width = 2.0F;
                    this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                }

                this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                {
                    lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), lowerBoundTime, Convert.ToDouble(lowerLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(lowerLimitDiameter));
                    lowerLimitLine.Line.Width = 2.0F;
                    this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                }

                this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                {
                    nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), lowerBoundTime, Convert.ToDouble(nominalDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(nominalDiameter));
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
    public class ResizableArray<T>
    {
        T[] m_array;
        int m_count;

        public ResizableArray(int? initialCapacity = null)
        {
            m_array = new T[initialCapacity ?? 4]; // or whatever
        }

        internal T[] InternalArray { get { return m_array; } }

        public int Count { get { return m_count; } }

        public void Add(T element)
        {
            if (m_count == m_array.Length)
            {
                Array.Resize(ref m_array, m_array.Length * 2);
                //Array.Resize(ref m_array, m_array.Length + 1);
            }

            m_array[m_count++] = element;
        }
    }
}


