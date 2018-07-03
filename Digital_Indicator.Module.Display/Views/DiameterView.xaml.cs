using Digital_Indicator.Logic.Filament;
using Digital_Indicator.Logic.Navigation;
using Digital_Indicator.WindowForms.ZedGraphUserControl;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media.Animation;
using ZedGraph;

namespace Digital_Indicator.Module.Display.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class DiameterView : UserControl
    {
        IFilamentService _filamentService;
        INavigationService _navigationService;
        Stopwatch timer;
        long previousHistoricalMillis;
        long previousRealTimeMillis;
        long previousScanMillis;

        Storyboard plotStoryboard;

        ZedGraphUserControl zgraphHistorical;
        ZedGraphUserControl zgraphRealTime;

        bool settingsWindowOpen;
        bool updateInProgress;

        public DiameterView(IFilamentService filamentService, INavigationService navigationService)
        {
            InitializeComponent();

            timer = new Stopwatch();

            _filamentService = filamentService;
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;
            _navigationService = navigationService;
            _navigationService.RegionCleared += _navigationService_RegionCleared;

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
            if (this.ActualWidth - 320 > 0)
            {
                if (settingsWindowOpen && zedGraphHistoricalModel.Width != this.ActualWidth - 320)
                {
                    
                    zedGraphHistoricalModel.Width = this.ActualWidth - 320;
                    zedGraphRealTimeModel.Width = this.ActualWidth - 320;
                }
                else if (zedGraphHistoricalModel.Width != this.ActualWidth)
                {
                    zedGraphHistoricalModel.Width = this.ActualWidth;
                    zedGraphRealTimeModel.Width = this.ActualWidth;
                }
            }
        }

        private void _navigationService_RegionCleared(object sender, EventArgs e)
        {
            if (sender.ToString() == "SettingsRegion")
            {
                Storyboard sb = new Storyboard();

                DoubleAnimation doubleAnimationHistorical = new DoubleAnimation(zedGraphHistoricalModel.Width, this.ActualWidth, new Duration(TimeSpan.FromMilliseconds(200)));
                DoubleAnimation doubleAnimationRealTime = new DoubleAnimation(zedGraphRealTimeModel.Width, this.ActualWidth, new Duration(TimeSpan.FromMilliseconds(200)));
                CircleEase ease = new CircleEase() { EasingMode = EasingMode.EaseInOut };
                doubleAnimationHistorical.EasingFunction = ease;
                doubleAnimationRealTime.EasingFunction = ease;
                sb.Children.Add(doubleAnimationHistorical);
                sb.Children.Add(doubleAnimationRealTime);
                Storyboard.SetTarget(doubleAnimationHistorical, zedGraphHistoricalModel);
                Storyboard.SetTarget(doubleAnimationRealTime, zedGraphRealTimeModel);
                Storyboard.SetTargetProperty(doubleAnimationHistorical, new PropertyPath("(Width)"));
                Storyboard.SetTargetProperty(doubleAnimationRealTime, new PropertyPath("(Width)"));

                sb.Completed += Storyboard_Completed;
                plotStoryboard = sb;

                sb.Begin();

                settingsWindowOpen = false;
            }
        }

        private void DiameterView_LayoutUpdated(object sender, EventArgs e)
        {
            zedGraphHistoricalModel.Width = this.ActualWidth;
            zedGraphRealTimeModel.Width = this.ActualWidth;

            zgraphHistorical.ZedGraph.AxisChange();
            zgraphHistorical.ZedGraph.Refresh();

            zgraphRealTime.ZedGraph.AxisChange();
            zgraphRealTime.ZedGraph.Refresh();

            this.LayoutUpdated -= DiameterView_LayoutUpdated; //fire once
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimation doubleAnimationHistorical = new DoubleAnimation(zedGraphHistoricalModel.Width, this.ActualWidth - 320, new Duration(TimeSpan.FromMilliseconds(200)));
            DoubleAnimation doubleAnimationRealTime = new DoubleAnimation(zedGraphRealTimeModel.Width, this.ActualWidth - 320, new Duration(TimeSpan.FromMilliseconds(200)));
            CircleEase ease = new CircleEase() { EasingMode = EasingMode.EaseInOut };
            doubleAnimationHistorical.EasingFunction = ease;
            doubleAnimationRealTime.EasingFunction = ease;
            sb.Children.Add(doubleAnimationHistorical);
            sb.Children.Add(doubleAnimationRealTime);
            Storyboard.SetTarget(doubleAnimationHistorical, zedGraphHistoricalModel);
            Storyboard.SetTarget(doubleAnimationRealTime, zedGraphRealTimeModel);
            Storyboard.SetTargetProperty(doubleAnimationHistorical, new PropertyPath("(Width)"));
            Storyboard.SetTargetProperty(doubleAnimationRealTime, new PropertyPath("(Width)"));

            sb.Completed += Storyboard_Completed;
            plotStoryboard = sb;
            sb.Begin();

            settingsWindowOpen = true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            var width = this.zedGraphHistoricalModel.Width;
            plotStoryboard.Stop();
            this.zedGraphHistoricalModel.Width = width;
            this.zedGraphRealTimeModel.Width = width;
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {

            if (timer.ElapsedMilliseconds >= previousScanMillis + 1 && !updateInProgress)
            {
                updateInProgress = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    textBlockDiameter.Text = _filamentService.FilamentServiceVariables["ActualDiameter"];
                    this.InvalidateVisual();

                }));
                previousScanMillis = timer.ElapsedMilliseconds;
                updateInProgress = false;
            }

            if (timer.ElapsedMilliseconds >= previousRealTimeMillis + 10 && _filamentService.CaptureStarted && !updateInProgress)
            {
                updateInProgress = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    zgraphRealTime.ZedGraph.GraphPane.XAxis.Scale.Min = new XDate(DateTime.Now.AddMilliseconds(-5000));
                    zgraphRealTime.ZedGraph.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddMilliseconds(2));
                    zgraphRealTime.ZedGraph.AxisChange();
                    zgraphRealTime.ZedGraph.Refresh();

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


