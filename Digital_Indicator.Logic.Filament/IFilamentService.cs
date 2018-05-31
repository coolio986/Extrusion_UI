using Digital_Indicator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Filament
{
    public interface IFilamentService
    {
        string Description { get; set; }
        string ActualDiameter { get; set; }
        string NominalDiameter { get; set; }
        string UpperLimit { get; set; }
        string LowerLimit { get; set; }
        string HighestValue { get; set; }
        string LowestValue { get; set; }
        string SpoolNumber { get; set; }
        string BatchNumber { get; set; }

        event EventHandler DiameterChanged;
        event EventHandler PropertyChanged;

        bool CaptureStarted { get; set; }
        bool IsSimulationModeActive { get; set; }

        void SaveHistoricalData(List<DataListXY> dataPoints);
        
    }
}
