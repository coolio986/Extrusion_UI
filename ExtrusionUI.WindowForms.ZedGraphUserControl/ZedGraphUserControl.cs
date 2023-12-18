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
        private LineItem X_diameterCurve;
        private LineItem Y_diameterCurve;
        private FilteredPointList filtered_X_Diameter;
        private FilteredPointList filtered_Y_Diameter;

        private double[] dblTime = new double[1];
        private double[] dblDiameter = new double[1];
        ResizableArray<double> resizable_X_ArrayDateTime = new ResizableArray<double>();
        ResizableArray<double> resizable_X_ArrayDiameter = new ResizableArray<double>();

        ResizableArray<double> resizable_Y_ArrayDateTime = new ResizableArray<double>();
        ResizableArray<double> resizable_Y_ArrayDiameter = new ResizableArray<double>();


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
                if(filtered_X_Diameter != null)
                    filtered_X_Diameter.SetBounds(sender.GraphPane.XAxis.Scale.Min, sender.GraphPane.XAxis.Scale.Max, (int)sender.GraphPane.Rect.Width);

                if (filtered_Y_Diameter != null)
                    filtered_Y_Diameter.SetBounds(sender.GraphPane.XAxis.Scale.Min, sender.GraphPane.XAxis.Scale.Max, (int)sender.GraphPane.Rect.Width);

                ReZoomLineObjs();

                // This refreshes the graph when the button is released after a panning operation
                if (newState.Type == ZoomState.StateType.Pan)
                    sender.Invalidate();
            }
        }

        public void ClearPlots()
        {
            diameterList.Clear();
            resizable_X_ArrayDateTime = new ResizableArray<double>();
            resizable_X_ArrayDiameter = new ResizableArray<double>();
            resizable_Y_ArrayDateTime = new ResizableArray<double>();
            resizable_Y_ArrayDiameter = new ResizableArray<double>();

            this.zedGraphControl1.GraphPane.CurveList.Remove(X_diameterCurve);
            this.zedGraphControl1.GraphPane.CurveList.Remove(Y_diameterCurve);
            
            X_diameterCurve = new LineItem("");
            Y_diameterCurve = new LineItem("");
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

        public void AddDataPointX(string diameter)
        {
            //if (!stopwatch.IsRunning)
            //    stopwatch.Start();
            DateTime dateTime = DateTime.Now;

            double dblDateTime = new XDate(dateTime);
            double dblDiameter = Convert.ToDouble(diameter);
            //diameterList.Add(new DataListXY(stopwatch.ElapsedMilliseconds, Convert.ToDouble(diameter)));

            int timeLength = diameterList.Count;
            
            diameterList.Add(new DataListXY(dblDateTime, dblDiameter));

            resizable_X_ArrayDateTime.Add(dblDateTime);
            resizable_X_ArrayDiameter.Add(dblDiameter);

            double dblLowerLimitDiameter = Convert.ToDouble(lowerLimitDiameter) - 0.025;
            double dblUpperLimitDiameter = Convert.ToDouble(upperLimitDiameter) + 0.025;

            if (this.zedGraphControl1.GraphPane == null)
                return;

            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min != dblLowerLimitDiameter)
                this.zedGraphControl1.GraphPane.YAxis.Scale.Min = dblLowerLimitDiameter;

            if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max != dblUpperLimitDiameter)
                this.zedGraphControl1.GraphPane.YAxis.Scale.Max = dblUpperLimitDiameter;

            if (this.zedGraphControl1.GraphPane == null)
                return;

            if (IsHistorical)
            {
                if (!IsZoomed)
                {
                    filtered_X_Diameter = new FilteredPointList(resizable_X_ArrayDateTime.InternalArray, resizable_X_ArrayDiameter.InternalArray);
                    filtered_X_Diameter.SetBounds(this.zedGraphControl1.GraphPane.XAxis.Scale.Min, new XDate(DateTime.Now.AddMilliseconds(2)), (int)this.zedGraphControl1.GraphPane.Rect.Width);

                    this.zedGraphControl1.GraphPane.CurveList.Remove(X_diameterCurve);

                    X_diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("X Diameter", filtered_X_Diameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);

                    //***********************//
                    //Adding the reference lines here updates the visual faster than adding them in a function
                    this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                    if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                    {
                        upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(upperLimitDiameter), resizable_X_ArrayDateTime.InternalArray[resizable_X_ArrayDateTime.Count - 1], Convert.ToDouble(upperLimitDiameter));
                        upperLimitLine.Line.Width = 2.0F;
                        this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                    }

                    this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                    if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                    {
                        lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(lowerLimitDiameter), resizable_X_ArrayDateTime.InternalArray[resizable_X_ArrayDateTime.Count - 1], Convert.ToDouble(lowerLimitDiameter));
                        lowerLimitLine.Line.Width = 2.0F;
                        this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                    }

                    this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                    if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                    {
                        nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(nominalDiameter), resizable_X_ArrayDateTime.InternalArray[resizable_X_ArrayDateTime.Count - 1], Convert.ToDouble(nominalDiameter));
                        nominalLine.Line.Width = 2.0F;
                        this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
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


                for (int i = resizable_X_ArrayDateTime.Count - 1; i > 0; i--)
                {
                    if (resizable_X_ArrayDateTime.InternalArray[i] > lowerBoundTime)
                    {
                        realTimeGraphTimes.Add(resizable_X_ArrayDateTime.InternalArray[i]);
                        realTimeDiameters.Add(resizable_X_ArrayDiameter.InternalArray[i]);
                    }
                    else
                        break;
                }


                filtered_X_Diameter = new FilteredPointList(realTimeGraphTimes.InternalArray, realTimeDiameters.InternalArray);
                filtered_X_Diameter.SetBounds(lowerBoundTime, higherBoundTime, (int)this.zedGraphControl1.GraphPane.Rect.Width);

                this.zedGraphControl1.GraphPane.CurveList.Remove(X_diameterCurve);

                X_diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("X Diameter", filtered_X_Diameter, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);

                

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

        public void AddDataPointY(string diameter)
        {
            //if (!stopwatch.IsRunning)
            //    stopwatch.Start();
            DateTime dateTime = DateTime.Now;

            double dblDateTime = new XDate(dateTime);
            double dblDiameter = Convert.ToDouble(diameter);
            //diameterList.Add(new DataListXY(stopwatch.ElapsedMilliseconds, Convert.ToDouble(diameter)));

            int timeLength = diameterList.Count;

            diameterList.Add(new DataListXY(dblDateTime, dblDiameter));

            resizable_Y_ArrayDateTime.Add(dblDateTime);
            resizable_Y_ArrayDiameter.Add(dblDiameter);

            //double dblLowerLimitDiameter = Convert.ToDouble(lowerLimitDiameter) - 0.025;
            //double dblUpperLimitDiameter = Convert.ToDouble(upperLimitDiameter) + 0.025;

            //if (this.zedGraphControl1.GraphPane == null)
            //    return;

            //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min != dblLowerLimitDiameter)
            //    this.zedGraphControl1.GraphPane.YAxis.Scale.Min = dblLowerLimitDiameter;

            //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max != dblUpperLimitDiameter)
            //    this.zedGraphControl1.GraphPane.YAxis.Scale.Max = dblUpperLimitDiameter;

            if (this.zedGraphControl1.GraphPane == null)
                return;

            if (IsHistorical)
            {
                if (!IsZoomed)
                {
                    filtered_Y_Diameter = new FilteredPointList(resizable_Y_ArrayDateTime.InternalArray, resizable_Y_ArrayDiameter.InternalArray);
                    filtered_Y_Diameter.SetBounds(this.zedGraphControl1.GraphPane.XAxis.Scale.Min, new XDate(DateTime.Now.AddMilliseconds(2)), (int)this.zedGraphControl1.GraphPane.Rect.Width);

                    this.zedGraphControl1.GraphPane.CurveList.Remove(Y_diameterCurve);

                    Y_diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Y Diameter", filtered_Y_Diameter, System.Drawing.Color.FromArgb(0, 255, 153), SymbolType.None);

                    //***********************//
                    //Adding the reference lines here updates the visual faster than adding them in a function
                    //this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                    //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                    //{
                    //    upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(upperLimitDiameter), resizable_Y_ArrayDateTime.InternalArray[resizable_Y_ArrayDateTime.Count - 1], Convert.ToDouble(upperLimitDiameter));
                    //    upperLimitLine.Line.Width = 2.0F;
                    //    this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                    //}

                    //this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                    //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                    //{
                    //    lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(lowerLimitDiameter), resizable_Y_ArrayDateTime.InternalArray[resizable_Y_ArrayDateTime.Count - 1], Convert.ToDouble(lowerLimitDiameter));
                    //    lowerLimitLine.Line.Width = 2.0F;
                    //    this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                    //}

                    //this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                    //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                    //{
                    //    nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), this.zedGraphControl1.GraphPane.XAxis.Scale.Min, Convert.ToDouble(nominalDiameter), resizable_Y_ArrayDateTime.InternalArray[resizable_Y_ArrayDateTime.Count - 1], Convert.ToDouble(nominalDiameter));
                    //    nominalLine.Line.Width = 2.0F;
                    //    this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
                    //}
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


                for (int i = resizable_Y_ArrayDateTime.Count - 1; i > 0; i--)
                {
                    if (resizable_Y_ArrayDateTime.InternalArray[i] > lowerBoundTime)
                    {
                        realTimeGraphTimes.Add(resizable_Y_ArrayDateTime.InternalArray[i]);
                        realTimeDiameters.Add(resizable_Y_ArrayDiameter.InternalArray[i]);
                    }
                    else
                        break;
                }

                filtered_Y_Diameter = new FilteredPointList(realTimeGraphTimes.InternalArray, realTimeDiameters.InternalArray);
                filtered_Y_Diameter.SetBounds(lowerBoundTime, higherBoundTime, (int)this.zedGraphControl1.GraphPane.Rect.Width);

                this.zedGraphControl1.GraphPane.CurveList.Remove(Y_diameterCurve);

                Y_diameterCurve = this.zedGraphControl1.GraphPane.AddCurve("Y Diameter", filtered_Y_Diameter, System.Drawing.Color.FromArgb(0, 255, 153), SymbolType.None);



                zedGraphControl1.GraphPane.XAxis.Scale.Min = lowerBoundTime;
                zedGraphControl1.GraphPane.XAxis.Scale.Max = higherBoundTime;


                //***********************//
                //Adding the reference lines here updates the visual faster than adding them in a function
                //this.zedGraphControl1.GraphPane.GraphObjList.Remove(upperLimitLine);
                //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(upperLimitDiameter))
                //{
                //    upperLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), lowerBoundTime, Convert.ToDouble(upperLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(upperLimitDiameter));
                //    upperLimitLine.Line.Width = 2.0F;
                //    this.zedGraphControl1.GraphPane.GraphObjList.Add(upperLimitLine);
                //}

                //this.zedGraphControl1.GraphPane.GraphObjList.Remove(lowerLimitLine);
                //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(lowerLimitDiameter))
                //{
                //    lowerLimitLine = new LineObj(System.Drawing.Color.FromArgb(255, 0, 0), lowerBoundTime, Convert.ToDouble(lowerLimitDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(lowerLimitDiameter));
                //    lowerLimitLine.Line.Width = 2.0F;
                //    this.zedGraphControl1.GraphPane.GraphObjList.Add(lowerLimitLine);
                //}

                //this.zedGraphControl1.GraphPane.GraphObjList.Remove(nominalLine);
                //if (this.zedGraphControl1.GraphPane.YAxis.Scale.Min <= Convert.ToDouble(nominalDiameter) && this.zedGraphControl1.GraphPane.YAxis.Scale.Max >= Convert.ToDouble(nominalDiameter))
                //{
                //    nominalLine = new LineObj(System.Drawing.Color.FromArgb(64, 191, 67), lowerBoundTime, Convert.ToDouble(nominalDiameter), this.zedGraphControl1.GraphPane.XAxis.Scale.Max, Convert.ToDouble(nominalDiameter));
                //    nominalLine.Line.Width = 2.0F;
                //    this.zedGraphControl1.GraphPane.GraphObjList.Add(nominalLine);
                //}
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


