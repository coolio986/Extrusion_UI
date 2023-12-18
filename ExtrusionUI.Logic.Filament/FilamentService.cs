using ExtrusionUI.Core;
using ExtrusionUI.Infrastructure;
using ExtrusionUI.Logic.FileOperations;
using ExtrusionUI.Logic.Helpers;
using ExtrusionUI.Logic.SerialCommunications;
using ExtrusionUI.WindowForms.ZedGraphUserControl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.Filament
{
    public class FilamentService : IFilamentService
    {
        public event EventHandler DiameterChanged;

        public event EventHandler PropertyChanged;

        public event EventHandler StopWatchedTimeChanged;

        public ConcurrentDictionary<string, string> FilamentServiceVariables { get; private set; }

        public Stopwatch DurationClock { get; private set; }

        public String RemainingTime { get; private set; }

        private IXmlService _xmlService;
        private ICsvService _csvService;
        private ISerialService _serialService;

        private double previousWeight;

        private bool captureStarted;
        public bool CaptureStarted
        {
            get { return captureStarted; }
            set
            {
                captureStarted = value;

                if (captureStarted)
                {
                    //disconnect events while clearing plots to prevent race condition
                    _serialService.X_DiameterChanged -= SerialService_X_DiameterChanged;
                    _serialService.Y_DiameterChanged -= SerialService_Y_DiameterChanged;

                    FilamentServiceVariables[StaticStrings.HIGHESTVALUE] = FilamentServiceVariables[StaticStrings.X_ACTUALDIAMETER];
                    FilamentServiceVariables[StaticStrings.LOWESTVALUE] = FilamentServiceVariables[StaticStrings.X_ACTUALDIAMETER];
                    if (FilamentServiceVariables[StaticStrings.SPOOLNUMBER] == string.Empty) { FilamentServiceVariables[StaticStrings.SPOOLNUMBER] = "0"; }

                    FilamentServiceVariables[StaticStrings.SPOOLNUMBER] = (FilamentServiceVariables[StaticStrings.SPOOLNUMBER].GetInteger() + 1).ToString();

                    SetupPlots();
                    SetupStopwatch();
                    SaveXmlData();

                    //reconnect events
                    _serialService.X_DiameterChanged += SerialService_X_DiameterChanged;
                    _serialService.Y_DiameterChanged += SerialService_Y_DiameterChanged;
                }
                else
                {
                    SaveHistoricalData(ZedGraphPlotModel.GetPlot("HistoricalModel").GetDataPoints());
                    DurationClock.Stop();
                }
                //SerialCommand command = new SerialCommand() { Command = "FilamentCapture", DeviceID = "100", Value = captureStarted ? "1" : "0" };
                //_serialService.SendSerialData(command);
            }
        }

        public bool IsSimulationModeActive
        {
            get { return _serialService.IsSimulationModeActive; }
            set { _serialService.IsSimulationModeActive = value; }
        }

        public FilamentService(ISerialService serialService, IXmlService xmlService, ICsvService csvService)
        {
            DurationClock = new Stopwatch();

            _serialService = serialService;
            _serialService.X_DiameterChanged += SerialService_X_DiameterChanged;
            _serialService.Y_DiameterChanged += SerialService_Y_DiameterChanged;
            //_serialService.TraverseDataChanged += _serialService_TraverseDataChanged;
            _serialService.GeneralDataChanged += _serialService_GeneralDataChanged;

            _xmlService = xmlService;
            _csvService = csvService;

            FilamentServiceVariables = new ConcurrentDictionary<string, string>();
            //FilamentServiceVariables.Add("Description", "");
            FilamentServiceVariables.TryAdd(StaticStrings.TRAVERSEMOTIONSTATUS, "None");
            FilamentServiceVariables.TryAdd(StaticStrings.SPOOLMOTIONSTATUS, "None");
            FilamentServiceVariables.TryAdd(StaticStrings.X_ACTUALDIAMETER, "");
            FilamentServiceVariables.TryAdd(StaticStrings.HIGHESTVALUE, "");
            FilamentServiceVariables.TryAdd(StaticStrings.LOWESTVALUE, "");
            FilamentServiceVariables.TryAdd(StaticStrings.FILAMENTNOMINALDIAMETER, "");
            FilamentServiceVariables.TryAdd(StaticStrings.FILAMENTUPPERLIMIT, "");
            FilamentServiceVariables.TryAdd(StaticStrings.FILAMENTLOWERLIMIT, "");
            FilamentServiceVariables.TryAdd(StaticStrings.TRAVERSEHOMEOFFSET, "Nan");
            FilamentServiceVariables.TryAdd(StaticStrings.SPOOLWIDTH, "Nan");
            //FilamentServiceVariables.Add("Tolerance", "");
            FilamentServiceVariables.TryAdd(StaticStrings.DURATION, "");
            FilamentServiceVariables.TryAdd(StaticStrings.SPOOLNUMBER, "");
            //FilamentServiceVariables.Add("SpoolRPM", "");
            //FilamentServiceVariables.Add("SpecificGravity", "");
            FilamentServiceVariables.TryAdd(StaticStrings.SPOOLWEIGHTLIMIT, "");
            //FilamentServiceVariables.Add("PullerRPM", "");
            //FilamentServiceVariables.Add("FilamentLength", "");
            //FilamentServiceVariables.Add("Feedrate", "");
            FilamentServiceVariables.TryAdd(StaticStrings.SPOOLWEIGHT, "");
            //FilamentServiceVariables.Add("OutputRate", "");
            //FilamentServiceVariables.Add("RemainingTime", "");
            //FilamentServiceVariables.Add("KeepAlive", "Disconnected");
            //FilamentServiceVariables.Add("DiameterError", "");
            //FilamentServiceVariables.Add("velocity", "0");

            BuildXmlData();
            SetupPlots();
        }

        private void _serialService_GeneralDataChanged(object sender, EventArgs e)
        {
            SerialCommand command = (SerialCommand)sender;

            FilamentServiceVariables[command.Command] = command.Value;

            PropertyChanged?.Invoke(command, new PropertyChangedEventArgs(command.Command));

            if (command.Command == "LoggerMotionState" && command.Value == "Start" && !captureStarted)
            {
                //captureStarted = true;
            }

            ////Expression set (faster than reflection)
            //Type type = this.GetType();
            //PropertyInfo propertyInfo = type.GetProperty(command.Command);
            //ParameterExpression instanceParam = Expression.Parameter(type);
            //ParameterExpression argumentParam = Expression.Parameter(typeof(Object));

            //if (propertyInfo != null)
            //{
            //    Action<FilamentService, Object> expression = Expression.Lambda<Action<FilamentService, Object>>(
            //       Expression.Call(instanceParam, propertyInfo.GetSetMethod(), Expression.Convert(argumentParam, propertyInfo.PropertyType)),
            //       instanceParam, argumentParam
            //     ).Compile();
            //    expression(this, command.Value);
            //}
        }

        private void SetupPlots()
        {
            if(FilamentServiceVariables[StaticStrings.FILAMENTUPPERLIMIT] == string.Empty)
                FilamentServiceVariables[StaticStrings.FILAMENTUPPERLIMIT] = "1.80";
            if (FilamentServiceVariables[StaticStrings.FILAMENTLOWERLIMIT] == string.Empty)
                FilamentServiceVariables[StaticStrings.FILAMENTLOWERLIMIT] = "1.70";
            if (FilamentServiceVariables[StaticStrings.FILAMENTNOMINALDIAMETER] == string.Empty)
                FilamentServiceVariables[StaticStrings.FILAMENTNOMINALDIAMETER] = "1.75";

            ZedGraphPlotModel.CreatePlots(FilamentServiceVariables[StaticStrings.FILAMENTUPPERLIMIT], FilamentServiceVariables[StaticStrings.FILAMENTNOMINALDIAMETER], FilamentServiceVariables[StaticStrings.FILAMENTLOWERLIMIT]);
            ZedGraphPlotModel.GetPlot("HistoricalModel").ZedGraph.GraphPane.XAxis.Scale.Min = new ZedGraph.XDate(DateTime.Now.AddMilliseconds(-100));

            //ZedGraphPlotModel.CreatePlots(UpperLimit, NominalDiameter, LowerLimit);
            //ZedGraphPlotModel.GetPlot("HistoricalModel").ZedGraph.GraphPane.XAxis.Scale.Min = new ZedGraph.XDate(DateTime.Now.AddMilliseconds(-100));

        }

        private void SerialService_X_DiameterChanged(object sender, EventArgs e)
        {
            //ActualDiameter = sender.ToString();

            double newDiameter = (double)Convert.ChangeType(sender, typeof(double));

            FilamentServiceVariables[StaticStrings.X_ACTUALDIAMETER] = sender.ToString();

            if (captureStarted)
            {
                ZedGraphPlotModel.GetPlots().ForEach(x => x.AddDataPointX(FilamentServiceVariables[StaticStrings.X_ACTUALDIAMETER]));
                //Parallel.ForEach(ZedGraphPlotModel.GetPlots(), zedGraphUserControl =>
                //{
                //    zedGraphUserControl.AddDataPointX(FilamentServiceVariables[StaticStrings.X_ACTUALDIAMETER]);
                //});
            }
                //ZedGraphPlotModel.GetPlots().Select(x => { x.AddDataPoint(FilamentServiceVariables[StaticStrings.ACTUALDIAMETER]); return x; }).ToList();

            DiameterChanged?.Invoke(sender, e);

            if (captureStarted)
                UpdateHighsAndLows();
        }

        private void SerialService_Y_DiameterChanged(object sender, EventArgs e)
        {
            //ActualDiameter = sender.ToString();

            double newDiameter = (double)Convert.ChangeType(sender, typeof(double));

            FilamentServiceVariables[StaticStrings.Y_ACTUALDIAMETER] = sender.ToString();

            if (captureStarted)
            {
                ZedGraphPlotModel.GetPlots().ForEach(x => x.AddDataPointY(FilamentServiceVariables[StaticStrings.Y_ACTUALDIAMETER]));
                //Parallel.ForEach(ZedGraphPlotModel.GetPlots(), zedGraphUserControl =>
                //{
                //    zedGraphUserControl.AddDataPointY(FilamentServiceVariables[StaticStrings.Y_ACTUALDIAMETER]);
                //});
            }
            //ZedGraphPlotModel.GetPlots().Select(x => { x.AddDataPoint(FilamentServiceVariables[StaticStrings.ACTUALDIAMETER]); return x; }).ToList();

            DiameterChanged?.Invoke(sender, e);

            if (captureStarted)
                UpdateHighsAndLows();
        }

        private void UpdateHighsAndLows()
        {
            string X_actualDiameter = FilamentServiceVariables[StaticStrings.X_ACTUALDIAMETER];
            string highestValue = FilamentServiceVariables[StaticStrings.HIGHESTVALUE];
            string lowestValue = FilamentServiceVariables[StaticStrings.LOWESTVALUE];
            FilamentServiceVariables[StaticStrings.HIGHESTVALUE] = highestValue == null ? X_actualDiameter : highestValue.GetDouble() < X_actualDiameter.GetDouble() ? X_actualDiameter : highestValue;
            FilamentServiceVariables[StaticStrings.LOWESTVALUE] = lowestValue == null ? X_actualDiameter : lowestValue.GetDouble() > X_actualDiameter.GetDouble() ? X_actualDiameter : lowestValue;
            FilamentServiceVariables["Tolerance"] = ((highestValue.GetDouble() - lowestValue.GetDouble()) / 2).ToString("0.000");
        }

        private void UpdatePlots()
        {
            var zedGraphPlotModel = ZedGraphPlotModel.GetPlots();

            if (zedGraphPlotModel != null)
                zedGraphPlotModel.Select(x =>
                {
                    x.UpperLimitDiameter = FilamentServiceVariables[StaticStrings.FILAMENTUPPERLIMIT];
                    x.NominalDiameter = FilamentServiceVariables[StaticStrings.FILAMENTNOMINALDIAMETER];
                    x.LowerLimitDiameter = FilamentServiceVariables[StaticStrings.FILAMENTLOWERLIMIT];
                    return x;
                }).ToList();
        }

        private void BuildXmlData()
        {
            foreach (KeyValuePair<string, string> kvp in FilamentServiceVariables.ToList())
            {
                if (!_xmlService.XmlSettings.ContainsKey("filamentData." + kvp.Key))
                {
                    _xmlService.XmlSettings.Add("filamentData." + kvp.Key.ToString(), kvp.Value.ToString());
                }

                FilamentServiceVariables[kvp.Key] = _xmlService.XmlSettings["filamentData." + kvp.Key];
                if (kvp.Key == "DiameterError")
                    FilamentServiceVariables[kvp.Key] = "";
            }
            SaveXmlData();
        }

        public void SaveXmlData()
        {
            foreach (KeyValuePair<string, string> kvp in FilamentServiceVariables.ToList())
            {
                _xmlService.XmlSettings["filamentData." + kvp.Key] = kvp.Value;
            }
            UpdatePlots();

            _xmlService.SaveSettings();
        }

        private void SetupStopwatch()
        {
            DurationClock.Reset();
            DurationClock.Start();


            Task.Factory.StartNew(() =>
            {
                while (DurationClock.IsRunning)
                {



                    FilamentServiceVariables[StaticStrings.DURATION] = DurationClock.Elapsed.Hours.ToString("0") + ":" +
                                                          DurationClock.Elapsed.Minutes.ToString("0#") + ":" +
                                                          DurationClock.Elapsed.Seconds.ToString("0#");

                    double actSpoolWeight = 0.0;
                    double.TryParse(FilamentServiceVariables[StaticStrings.SPOOLWEIGHT], out actSpoolWeight);
                    double spoolWeightLimit = 0;
                    double.TryParse(FilamentServiceVariables[StaticStrings.SPOOLWEIGHTLIMIT], out spoolWeightLimit);


                    double timePerWeight = (DurationClock.ElapsedMilliseconds / actSpoolWeight) * spoolWeightLimit;


                    if (previousWeight != actSpoolWeight)
                    {
                        if (actSpoolWeight > 0 && timePerWeight != double.NaN && timePerWeight > 0 && DurationClock.ElapsedMilliseconds >= 15000)
                        {
                            TimeSpan remainingTime = TimeSpan.Zero;
                            remainingTime = TimeSpan.FromMilliseconds(timePerWeight);
                            remainingTime = TimeSpan.FromMilliseconds(remainingTime.TotalMilliseconds - DurationClock.ElapsedMilliseconds);

                            if (remainingTime.TotalMilliseconds > 0)
                            {
                                FilamentServiceVariables["RemainingTime"] = remainingTime.Hours.ToString("0") + ":" +
                                                                      remainingTime.Minutes.ToString("0#") + ":" +
                                                                     remainingTime.Seconds.ToString("0#");
                            }
                            else
                            {
                                FilamentServiceVariables["RemainingTime"] = "0:00:00";
                            }
                            previousWeight = actSpoolWeight;
                        }
                    }

                    if (DurationClock.ElapsedMilliseconds < 15000)
                    {
                        FilamentServiceVariables["RemainingTime"] = "Stabilizing";
                    }

                    StopWatchedTimeChanged?.Invoke(DurationClock, new EventArgs());

                    System.Threading.Thread.Sleep(500);
                }

            });
        }


        public void SaveHistoricalData(HashSet<DataListXY> dataPoints)
        {

            //_csvService.SaveSettings(dataPoints, FilamentServiceVariables[StaticStrings.SPOOLNUMBER], FilamentServiceVariables["Description"]);
            _csvService.SaveSettings(dataPoints, FilamentServiceVariables[StaticStrings.SPOOLNUMBER], "");
        }
    }

}
