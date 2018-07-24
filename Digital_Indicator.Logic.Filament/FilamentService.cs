using Digital_Indicator.Infrastructure;
using Digital_Indicator.Logic.FileOperations;
using Digital_Indicator.Logic.Helpers;
using Digital_Indicator.Logic.SerialCommunications;
using Digital_Indicator.WindowForms.ZedGraphUserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Filament
{
    public class FilamentService : IFilamentService
    {
        public event EventHandler DiameterChanged;

        public event EventHandler PropertyChanged;

        public event EventHandler StopWatchedTimeChanged;

        public Dictionary<string, string> FilamentServiceVariables { get; private set; }

        public Stopwatch stopWatch { get; private set; }

        private IXmlService _xmlService;
        private ICsvService _csvService;
        private ISerialService _serialService;

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        private string nominalDiameter;
        public string NominalDiameter
        {
            get { return nominalDiameter; }
            set
            {
                nominalDiameter = value;

                OnPropertyChanged();
                UpdatePlots();
                SaveXmlData();
                SetFilamentVariables();
            }
        }

        private string upperLimit;
        public string UpperLimit
        {
            get { return upperLimit; }
            set
            {
                upperLimit = value;
                OnPropertyChanged();
                UpdatePlots();
                SaveXmlData();
                SetFilamentVariables();
            }
        }

        private string lowerLimit;
        public string LowerLimit
        {
            get { return lowerLimit; }
            set
            {
                lowerLimit = value;

                OnPropertyChanged();
                UpdatePlots();
                SaveXmlData();
                SetFilamentVariables();
            }
        }

        private string spoolNumber;
        public string SpoolNumber
        {
            get { return spoolNumber; }
            set
            {
                spoolNumber = value;
                OnPropertyChanged();
                UpdatePlots();
                SaveXmlData();
            }
        }

        private string batchNumber;
        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                batchNumber = value;
                OnPropertyChanged();
                UpdatePlots();
                SaveXmlData();
            }
        }

        private bool captureStarted;
        public bool CaptureStarted
        {
            get { return captureStarted; }
            set
            {
                captureStarted = value;
                if (captureStarted)
                {
                    FilamentServiceVariables["HighestValue"] = nominalDiameter;
                    FilamentServiceVariables["LowestValue"] = nominalDiameter;
                    SpoolNumber = (spoolNumber.GetInteger() + 1).ToString();
                    SetupPlots();
                    SetupStopwatch();
                }
                else
                {
                    SaveHistoricalData(ZedGraphPlotModel.GetPlot("HistoricalModel").GetDataPoints());
                    stopWatch.Stop();
                }
            }
        }

        public bool IsSimulationModeActive
        {
            get { return _serialService.IsSimulationModeActive; }
            set { _serialService.IsSimulationModeActive = value; }
        }

        public FilamentService(ISerialService serialService, IXmlService xmlService, ICsvService csvService)
        {
            stopWatch = new Stopwatch();

            _serialService = serialService;
            _serialService.DiameterChanged += SerialService_DiameterChanged;

            _xmlService = xmlService;
            _csvService = csvService;

            FilamentServiceVariables = new Dictionary<string, string>();
            FilamentServiceVariables.Add("ActualDiameter", "");
            FilamentServiceVariables.Add("HighestValue", "");
            FilamentServiceVariables.Add("LowestValue", "");
            FilamentServiceVariables.Add("NominalValue", "");
            FilamentServiceVariables.Add("UpperLimit", "");
            FilamentServiceVariables.Add("LowerLimit", "");

            BuildXmlData();
            SetupPlots();
        }

        private void SetupPlots()
        {
            ZedGraphPlotModel.CreatePlots(UpperLimit, NominalDiameter, LowerLimit);
            ZedGraphPlotModel.GetPlot("HistoricalModel").ZedGraph.GraphPane.XAxis.Scale.Min = new ZedGraph.XDate(DateTime.Now.AddMilliseconds(-100));

        }

        private void SerialService_DiameterChanged(object sender, EventArgs e)
        {
            //ActualDiameter = sender.ToString();
            FilamentServiceVariables["ActualDiameter"] = sender.ToString();

            if (captureStarted)
                ZedGraphPlotModel.GetPlots().Select(x => { x.AddDataPoint(FilamentServiceVariables["ActualDiameter"]); return x; }).ToList();

            DiameterChanged?.Invoke(sender, e);

            if (captureStarted)
                UpdateHighsAndLows();
        }

        private void UpdateHighsAndLows()
        {
            string actualDiameter = FilamentServiceVariables["ActualDiameter"];
            string highestValue = FilamentServiceVariables["HighestValue"];
            string lowestValue = FilamentServiceVariables["LowestValue"];
            FilamentServiceVariables["HighestValue"] = highestValue == null ? actualDiameter : highestValue.GetDouble() < actualDiameter.GetDouble() ? actualDiameter : highestValue;
            FilamentServiceVariables["LowestValue"] = lowestValue == null ? actualDiameter : lowestValue.GetDouble() > actualDiameter.GetDouble() ? actualDiameter : lowestValue;
        }

        private void UpdatePlots()
        {
            var zedGraphPlotModel = ZedGraphPlotModel.GetPlots();

            if (zedGraphPlotModel != null)
                zedGraphPlotModel.Select(x =>
                {
                    x.UpperLimitDiameter = upperLimit;
                    x.NominalDiameter = nominalDiameter;
                    x.LowerLimitDiameter = lowerLimit;
                    return x;
                }).ToList();
        }

        private void BuildXmlData()
        {
            nominalDiameter = _xmlService.XmlSettings["filamentData.nominalDiameter"];
            upperLimit = _xmlService.XmlSettings["filamentData.upperLimit"];
            lowerLimit = _xmlService.XmlSettings["filamentData.lowerLimit"];
            spoolNumber = _xmlService.XmlSettings["filamentData.spoolNumber"];
            description = _xmlService.XmlSettings["filamentData.materialDescription"];
            batchNumber = _xmlService.XmlSettings["filamentData.batchNumber"];

            SetFilamentVariables();
        }

        private void SaveXmlData()
        {
            _xmlService.XmlSettings["filamentData.nominalDiameter"] = nominalDiameter;
            _xmlService.XmlSettings["filamentData.upperLimit"] = upperLimit;
            _xmlService.XmlSettings["filamentData.lowerLimit"] = lowerLimit;
            _xmlService.XmlSettings["filamentData.spoolNumber"] = spoolNumber;
            _xmlService.XmlSettings["filamentData.materialDescription"] = description;
            _xmlService.XmlSettings["filamentData.batchNumber"] = batchNumber;

            _xmlService.SaveSettings();
        }

        private void SetFilamentVariables()
        {
            FilamentServiceVariables["UpperLimit"] = upperLimit.ToString();
            FilamentServiceVariables["LowerLimit"] = lowerLimit.ToString();
            FilamentServiceVariables["NominalDiameter"] = nominalDiameter.ToString();

        }

        private void SetupStopwatch()
        {
            stopWatch.Reset();
            stopWatch.Start();


            Task.Factory.StartNew(() =>
            {
                while (stopWatch.IsRunning)
                {
                    StopWatchedTimeChanged?.Invoke(stopWatch, new EventArgs());
                    System.Threading.Thread.Sleep(500);
                }

            });
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveHistoricalData(HashSet<DataListXY> dataPoints)
        {
            _csvService.SaveSettings(dataPoints, spoolNumber, description);
        }
    }
}
