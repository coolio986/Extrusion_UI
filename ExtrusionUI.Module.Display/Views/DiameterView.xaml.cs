using ExtrusionUI.Core;
using ExtrusionUI.Logic.Filament;
using ExtrusionUI.Logic.Navigation;
using ExtrusionUI.Logic.SerialCommunications;
using ExtrusionUI.WindowForms.ZedGraphUserControl;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ZedGraph;

namespace ExtrusionUI.Module.Display.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class DiameterView : UserControl
    {
        private readonly IFilamentService _filamentService;
        private readonly INavigationService _navigationService;
        private readonly ISerialService _serialService;
        Stopwatch timer;
        long previousHistoricalMillis;
        long previousRealTimeMillis;
        long previousScanMillis;
        private int timeDelayMultiplier = 33;
        private int minTimeDelayMultiplier = 33;

        Storyboard plotStoryboard;

        ZedGraphUserControl zgraphHistorical;
        ZedGraphUserControl zgraphRealTime;

        bool settingsWindowOpen;
        bool updateInProgress;

        public DiameterView(IFilamentService filamentService, INavigationService navigationService, ISerialService serialService)
        {
            InitializeComponent();

            timer = new Stopwatch();

            _filamentService = filamentService;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            _navigationService = navigationService;
            _navigationService.RegionCleared += _navigationService_RegionCleared;
            _serialService = serialService;

            zgraphHistorical = ZedGraphPlotModel.GetPlot("HistoricalModel");
            zgraphRealTime = ZedGraphPlotModel.GetPlot("RealTimeModel");

            timer.Start();
            previousHistoricalMillis = 0;

            //add to grid on xaml page
            zedGraphHistoricalModel.Children.Add(new WindowsFormsHost() { Child = zgraphHistorical });
            zedGraphRealTimeModel.Children.Add(new WindowsFormsHost() { Child = zgraphRealTime });

            settingsButton.Click += SettingButton_Click;

            this.LayoutUpdated += DiameterView_LayoutUpdated;
            this.SizeChanged += DiameterView_SizeChanged;

        }

        private void DiameterView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (this.ActualWidth - 320 > 0)
            //{
            //    if (settingsWindowOpen && zedGraphHistoricalModel.Width != this.ActualWidth - 320)
            //    {
                    
            //        zedGraphHistoricalModel.Width = this.ActualWidth - 320;
            //        zedGraphRealTimeModel.Width = this.ActualWidth - 320;
            //    }
            //    else if (zedGraphHistoricalModel.Width != this.ActualWidth)
            //    {
            //        zedGraphHistoricalModel.Width = this.ActualWidth -320;
            //        zedGraphRealTimeModel.Width = this.ActualWidth -320;
            //    }
            //}
        }

        private void _navigationService_RegionCleared(object sender, EventArgs e)
        {
            //if (sender.ToString() == "SettingsRegion")
            //{
            //    Storyboard sb = new Storyboard();

            //    DoubleAnimation doubleAnimationHistorical = new DoubleAnimation(zedGraphHistoricalModel.Width, this.ActualWidth, new Duration(TimeSpan.FromMilliseconds(200)));
            //    DoubleAnimation doubleAnimationRealTime = new DoubleAnimation(zedGraphRealTimeModel.Width, this.ActualWidth, new Duration(TimeSpan.FromMilliseconds(200)));
            //    CircleEase ease = new CircleEase() { EasingMode = EasingMode.EaseInOut };
            //    doubleAnimationHistorical.EasingFunction = ease;
            //    doubleAnimationRealTime.EasingFunction = ease;
            //    sb.Children.Add(doubleAnimationHistorical);
            //    sb.Children.Add(doubleAnimationRealTime);
            //    Storyboard.SetTarget(doubleAnimationHistorical, zedGraphHistoricalModel);
            //    Storyboard.SetTarget(doubleAnimationRealTime, zedGraphRealTimeModel);
            //    Storyboard.SetTargetProperty(doubleAnimationHistorical, new PropertyPath("(Width)"));
            //    Storyboard.SetTargetProperty(doubleAnimationRealTime, new PropertyPath("(Width)"));

            //    sb.Completed += Storyboard_Completed;
            //    plotStoryboard = sb;

            //    sb.Begin();

            //    settingsWindowOpen = false;
            //}
        }

        private void DiameterView_LayoutUpdated(object sender, EventArgs e)
        {
            //zedGraphHistoricalModel.Width = this.ActualWidth -320;
            //zedGraphRealTimeModel.Width = this.ActualWidth -320;

            zgraphHistorical.ZedGraph.AxisChange();
            zgraphHistorical.ZedGraph.Refresh();

            zgraphRealTime.ZedGraph.AxisChange();
            zgraphRealTime.ZedGraph.Refresh();

            this.LayoutUpdated -= DiameterView_LayoutUpdated; //fire once
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            //Storyboard sb = new Storyboard();

            //DoubleAnimation doubleAnimationHistorical = new DoubleAnimation(zedGraphHistoricalModel.Width, this.ActualWidth - 320, new Duration(TimeSpan.FromMilliseconds(200)));
            //DoubleAnimation doubleAnimationRealTime = new DoubleAnimation(zedGraphRealTimeModel.Width, this.ActualWidth - 320, new Duration(TimeSpan.FromMilliseconds(200)));
            //CircleEase ease = new CircleEase() { EasingMode = EasingMode.EaseInOut };
            //doubleAnimationHistorical.EasingFunction = ease;
            //doubleAnimationRealTime.EasingFunction = ease;
            //sb.Children.Add(doubleAnimationHistorical);
            //sb.Children.Add(doubleAnimationRealTime);
            //Storyboard.SetTarget(doubleAnimationHistorical, zedGraphHistoricalModel);
            //Storyboard.SetTarget(doubleAnimationRealTime, zedGraphRealTimeModel);
            //Storyboard.SetTargetProperty(doubleAnimationHistorical, new PropertyPath("(Width)"));
            //Storyboard.SetTargetProperty(doubleAnimationRealTime, new PropertyPath("(Width)"));

            //sb.Completed += Storyboard_Completed;
            //plotStoryboard = sb;
            //sb.Begin();

            settingsWindowOpen = true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            //var width = this.zedGraphHistoricalModel.Width;
            //plotStoryboard.Completed -= Storyboard_Completed;
            plotStoryboard.Stop();
            //this.zedGraphHistoricalModel.Width = width;
            //this.zedGraphRealTimeModel.Width = width;
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {

            if (timer.ElapsedMilliseconds >= previousScanMillis + 1 && !updateInProgress)
            {
                updateInProgress = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    double actDiameter = 0;
                    double.TryParse(_filamentService.FilamentServiceVariables[StaticStrings.ACTUALDIAMETER], out actDiameter);

                    double UpperLimit = 0;
                    double.TryParse(_filamentService.FilamentServiceVariables[StaticStrings.FILAMENTUPPERLIMIT], out UpperLimit);

                    double LowerLimit = 0;
                    double.TryParse(_filamentService.FilamentServiceVariables[StaticStrings.FILAMENTLOWERLIMIT], out LowerLimit);

                    double HighestValue = 0;
                    double.TryParse(_filamentService.FilamentServiceVariables[StaticStrings.HIGHESTVALUE], out HighestValue);

                    double LowestValue = 0;
                    double.TryParse(_filamentService.FilamentServiceVariables[StaticStrings.LOWESTVALUE], out LowestValue);

                    if (actDiameter >= UpperLimit || actDiameter <= LowerLimit)
                        textBlockDiameter.Foreground = Brushes.Red;
                    else
                        textBlockDiameter.Foreground = Brushes.Black;


                    textBlockDiameter.Text = _filamentService.FilamentServiceVariables[StaticStrings.ACTUALDIAMETER] + " mm";

                    this.InvalidateVisual();

                }));
                previousScanMillis = timer.ElapsedMilliseconds;
                updateInProgress = false;
            }


            if (_serialService.SerialBufferSize >= 1000)
            {
                timeDelayMultiplier++;
                timeDelayMultiplier = timeDelayMultiplier > 1000 ? 1000 : timeDelayMultiplier;
            }
            else
            {
                if (!_filamentService.CaptureStarted)
                    timeDelayMultiplier = minTimeDelayMultiplier;

                timeDelayMultiplier--;
                timeDelayMultiplier = timeDelayMultiplier < minTimeDelayMultiplier ? minTimeDelayMultiplier : timeDelayMultiplier;
            }
            //Debug.WriteLine("timeDelayMultiplier: " + timeDelayMultiplier);
            if (timer.ElapsedMilliseconds >= previousRealTimeMillis + timeDelayMultiplier  && _filamentService.CaptureStarted && !updateInProgress)
            {
                updateInProgress = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    //zgraphRealTime.ZedGraph.GraphPane.XAxis.Scale.Min = new XDate(DateTime.Now.AddMilliseconds(-5000));
                    //zgraphRealTime.ZedGraph.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddMilliseconds(2));
                    zgraphRealTime.ZedGraph.AxisChange();
                    zgraphRealTime.ZedGraph.Refresh();
                    
                    //zgraphRealTime.ZedGraph.Invalidate();
                    //zgraphRealTime.ZedGraph.Update();

                }));
                previousRealTimeMillis = timer.ElapsedMilliseconds;
                updateInProgress = false;
            }

            if (timer.ElapsedMilliseconds >= previousHistoricalMillis + 5000 && _filamentService.CaptureStarted && !updateInProgress)
            {
                //updateInProgress = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!zgraphHistorical.IsZoomed)
                    {
                        zgraphHistorical.ZedGraph.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddMilliseconds(100));
                        zgraphHistorical.ZedGraph.AxisChange();
                        zgraphHistorical.ZedGraph.Refresh();
                    }

                }));
                previousHistoricalMillis = timer.ElapsedMilliseconds;
                //updateInProgress = false;
            }
        }
    }
}


