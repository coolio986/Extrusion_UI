using Digital_Indicator.Logic.Filament;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZedGraph;

namespace Digital_Indicator.Module.Display.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class DiameterView : UserControl
    {
        IFilamentService _filamentService;
        PointPairList listHistorical;
        PointPairList listRealTime;
        PointPairList listNominalDiameter;
        PointPairList listUpperLimit;
        PointPairList listLowerLimit;
        GraphPane realTimePane;
        GraphPane historicalPane;
        Stopwatch historicalTimer;
        long previousMillis;

        bool historicalUpdateInProgress;
        bool realTimeUpdateInProgress;


        public DiameterView(IFilamentService filamentService)
        {
            InitializeComponent();

            historicalTimer = new Stopwatch();

            _filamentService = filamentService;


            listHistorical = new PointPairList();
            listRealTime = new PointPairList();
            listNominalDiameter = new PointPairList();
            listUpperLimit = new PointPairList();
            listLowerLimit = new PointPairList();
            historicalPane = zgraphHistorical.GraphPane;
            realTimePane = zgraphRealTime.GraphPane;

            CreateGraphHistorical(zgraphHistorical);
            CreateGraphRealTime(zgraphRealTime);

            SetSizeHistorical();
            SetSizeRealTime();

            historicalTimer.Start();
            previousMillis = 5000;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            ResetGraph.Click += ResetGraph_Click;
        }

        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
            zgraphHistorical.ZoomOutAll(historicalPane);
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {
            double x;
            double y1;

            x = new XDate(DateTime.Now);
            y1 = Convert.ToDouble(_filamentService.ActualDiameter);
            listHistorical.Add(x, y1);
            listRealTime.Add(x, y1);
            listNominalDiameter.Add(x, 1.75);
            listUpperLimit.Add(x, 1.80);
            listLowerLimit.Add(x, 1.70);


            if (historicalTimer.ElapsedMilliseconds >= previousMillis + 5000 && !historicalUpdateInProgress)
            {
                historicalUpdateInProgress = true;
                Task.Factory.StartNew(() =>
                {
                    zgraphHistorical.AxisChange();
                    zgraphHistorical.Invalidate();
                    previousMillis = historicalTimer.ElapsedMilliseconds;
                    historicalUpdateInProgress = false;
                });
            }

            if (!realTimeUpdateInProgress)
            {
                realTimeUpdateInProgress = true;
                Task.Factory.StartNew(() =>
                {
                    zgraphRealTime.GraphPane.XAxis.Scale.Min = new XDate(DateTime.Now.AddMilliseconds(-5000));
                    zgraphRealTime.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddMilliseconds(2));
                    zgraphRealTime.AxisChange();
                    zgraphRealTime.Invalidate();
                    Thread.Sleep(50);
                    realTimeUpdateInProgress = false;
                });
            }


            Dispatcher.BeginInvoke(new Action(() =>
            {
                textBlockDiameter.Text = _filamentService.ActualDiameter;
                this.InvalidateVisual();
            }));
        }

        private void CreateGraphHistorical(ZedGraphControl zgc)
        {
            // get a reference to the GraphPane
            //GraphPane myPane = zgc.GraphPane;

            // Set the Titles
            // myPane.Title.Text = "My Test Graph\n(For CodeProject Sample)";
            historicalPane.XAxis.Title.Text = "Time";
            historicalPane.YAxis.Title.Text = "Diameter";
            //myPane.XAxis.Scale.MajorStep = 5;

            // Make up some data arrays based on the Sine function

            //PointPairList list1 = new PointPairList();
            //PointPairList list2 = new PointPairList();
            //for (int i = 0; i < 36; i++)
            //{
            // x = (double)i + 5;
            //y1 = 1.5 + Math.Sin((double)i * 0.2);
            //y2 = 3.0 * (1.5 + Math.Sin((double)i * 0.2));
            //list1.Add(x, y1);
            //list2.Add(x, y2);
            //}

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            historicalPane.AddCurve("Diameter", listHistorical, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);
            historicalPane.AddCurve("NominalDiameter", listNominalDiameter, System.Drawing.Color.FromArgb(64, 191, 67), SymbolType.None).Line.Width = 2.0F;
            historicalPane.AddCurve("UpperLimit", listUpperLimit, System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;
            historicalPane.AddCurve("LowerLimit", listLowerLimit, System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;

            // Generate a blue curve with circle
            // symbols, and "Piper" in the legend
            //LineItem myCurve2 = myPane.AddCurve("Piper",
            //      list2, System.Drawing.Color.Blue, SymbolType.Circle);

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
        }

        private void CreateGraphRealTime(ZedGraphControl zgc)
        {
            // get a reference to the GraphPane
            //GraphPane myPane = zgc.GraphPane;

            // Set the Titles
            // myPane.Title.Text = "My Test Graph\n(For CodeProject Sample)";
            realTimePane.XAxis.Title.Text = "Time";
            realTimePane.YAxis.Title.Text = "Diameter";
            //myPane.XAxis.Scale.MajorStep = 5;

            // Make up some data arrays based on the Sine function

            //PointPairList list1 = new PointPairList();
            //PointPairList list2 = new PointPairList();
            //for (int i = 0; i < 36; i++)
            //{
            // x = (double)i + 5;
            //y1 = 1.5 + Math.Sin((double)i * 0.2);
            //y2 = 3.0 * (1.5 + Math.Sin((double)i * 0.2));
            //list1.Add(x, y1);
            //list2.Add(x, y2);
            //}

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            realTimePane.AddCurve("Diameter", listRealTime, System.Drawing.Color.FromArgb(0, 153, 255), SymbolType.None);
            realTimePane.AddCurve("NominalDiameter", listNominalDiameter, System.Drawing.Color.FromArgb(64, 191, 67), SymbolType.None).Line.Width = 2.0F;
            realTimePane.AddCurve("UpperLimit", listUpperLimit, System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;
            realTimePane.AddCurve("LowerLimit", listLowerLimit, System.Drawing.Color.FromArgb(255, 0, 0), SymbolType.None).Line.Width = 2.0F;



            // Generate a blue curve with circle
            // symbols, and "Piper" in the legend
            //LineItem myCurve2 = myPane.AddCurve("Piper",
            //      list2, System.Drawing.Color.Blue, SymbolType.Circle);

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zgc.AxisChange();
        }

        private void SetSizeHistorical()
        {
            zgraphHistorical.Location = new System.Drawing.Point(10, 10);

            // Leave a small margin around the outside of the control
            zgraphHistorical.Size = new System.Drawing.Size((int)this.Width - 20,
                                    (int)this.Height - 20);
        }

        private void SetSizeRealTime()
        {
            zgraphRealTime.Location = new System.Drawing.Point(10, 10);

            // Leave a small margin around the outside of the control
            zgraphRealTime.Size = new System.Drawing.Size((int)this.Width - 20,
                                    (int)this.Height - 20);
        }
    }
}
