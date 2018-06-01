using Digital_Indicator.Infrastructure;
using Digital_Indicator.Infrastructure.UI;
using Digital_Indicator.Logic.FileOperations;
using Digital_Indicator.Logic.Helpers;
using Digital_Indicator.Logic.SerialCommunications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Dictionary<string, string> FilamentServiceVariables { get; private set; }

        private IXmlService _xmlService;
        private ICsvService _csvService;
        private ISerialService _serialService;

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string actualDiameter;
        public string ActualDiameter
        {
            get { return actualDiameter; }
            set
            {
                actualDiameter = value;
                FilamentServiceVariables["ActualDiameter"] = actualDiameter;
            }
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
            }
        }

        private string highestValue;
        public string HighestValue
        {
            get { return highestValue; }
            set
            {
                highestValue = value;
                FilamentServiceVariables["HighestValue"] = highestValue;
            }
        }

        private string lowestValue;
        public string LowestValue
        {
            get { return lowestValue; }
            set
            {
                lowestValue = value;
                FilamentServiceVariables["LowestValue"] = lowestValue;
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
                    highestValue = nominalDiameter;
                    lowestValue = nominalDiameter;
                    SpoolNumber = (spoolNumber.GetInteger() + 1).ToString();
                    SetupPlots();
                }
                else
                    SaveHistoricalData(LinearSeriesPlotModel.GetPlot("HistoricalModel").GetDataPoints());
            }
        }

        public bool IsSimulationModeActive
        {
            get { return _serialService.IsSimulationModeActive; }
            set { _serialService.IsSimulationModeActive = value; }
        }

        public FilamentService(ISerialService serialService, IXmlService xmlService, ICsvService csvService)
        {
            _serialService = serialService;
            _serialService.DiameterChanged += SerialService_DiameterChanged;

            _xmlService = xmlService;
            _csvService = csvService;

            BuildXmlData();
            SetupPlots();

            FilamentServiceVariables = new Dictionary<string, string>();
            FilamentServiceVariables.Add("ActualDiameter", actualDiameter);
            FilamentServiceVariables.Add("HighestValue", highestValue);
            FilamentServiceVariables.Add("LowestValue", lowestValue);
        }

        private void SetupPlots()
        {
            LinearSeriesPlotModel.CreatePlots(UpperLimit, NominalDiameter, LowerLimit);
        }

        private void SerialService_DiameterChanged(object sender, EventArgs e)
        {
            ActualDiameter = sender.ToString();

            if (captureStarted)
                LinearSeriesPlotModel.GetPlots().Select(x => { x.AddDataPoint(actualDiameter); return x; }).ToList();

            DiameterChanged?.Invoke(sender, e);

            if (captureStarted)
                UpdateHighsAndLows();
        }

        private void UpdateHighsAndLows()
        {
            HighestValue = highestValue == null ? actualDiameter : highestValue.GetDouble() < actualDiameter.GetDouble() ? actualDiameter : highestValue;
            LowestValue = lowestValue == null ? actualDiameter : lowestValue.GetDouble() > actualDiameter.GetDouble() ? actualDiameter : lowestValue;
        }

        private void UpdatePlots()
        {
            LinearSeriesPlotModel.GetPlots().Select(x =>
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
