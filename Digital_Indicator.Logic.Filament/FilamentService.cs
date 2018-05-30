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

        private IXmlService _xmlService;

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
            set { actualDiameter = value; }
        }

        private string nominalDiameter;
        public string NominalDiameter
        {
            get { return nominalDiameter; }
            set
            {
                nominalDiameter = value;
                OnPropertyChanged();
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
                SaveXmlData();
            }
        }

        private string highestValue;
        public string HighestValue
        {
            get { return highestValue; }
            set { highestValue = value; }
        }

        private string lowestValue;
        public string LowestValue
        {
            get { return lowestValue; }
            set { lowestValue = value; }
        }

        private string spoolNumber;
        public string SpoolNumber
        {
            get { return spoolNumber; }
            set
            {
                spoolNumber = value;
                OnPropertyChanged();
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
                }
            }
        }

        public FilamentService(ISerialService serialService, IXmlService xmlService)
        {
            serialService.DiameterChanged += SerialService_DiameterChanged;

            _xmlService = xmlService;

            BuildXmlData();
        }

        private void SerialService_DiameterChanged(object sender, EventArgs e)
        {
            ActualDiameter = sender.ToString();
            DiameterChanged?.Invoke(sender, e);

            if (captureStarted)
                UpdateHighsAndLows();

        }

        private void UpdateHighsAndLows()
        {
            HighestValue = highestValue == null ? actualDiameter : highestValue.GetDouble() < actualDiameter.GetDouble() ? actualDiameter : highestValue;
            LowestValue = lowestValue == null ? actualDiameter : lowestValue.GetDouble() > actualDiameter.GetDouble() ? actualDiameter : lowestValue;
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
    }
}
