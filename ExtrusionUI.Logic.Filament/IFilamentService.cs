using ExtrusionUI.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.Filament
{
    public interface IFilamentService
    {
        //string Description { get; set; }
        //string ActualDiameter { get; set; }
        //string NominalDiameter { get; set; }
        //string UpperLimit { get; set; }
        //string LowerLimit { get; set; }
        //string HighestValue { get; set; }
        //string LowestValue { get; set; }
        //string SpoolNumber { get; set; }
        //string SpoolRPM { get; set; }
        Dictionary<string, string> FilamentServiceVariables { get; }

        event EventHandler DiameterChanged;
        event EventHandler PropertyChanged;
        event EventHandler StopWatchedTimeChanged;

        bool CaptureStarted { get; set; }
        bool IsSimulationModeActive { get; set; }

        //void SaveHistoricalData(HashSet<DataListXY> dataPoints);
        void SaveHistoricalData(ArrayList dataPoints);
        void SaveXmlData();

        Stopwatch DurationClock { get; }

        string RemainingTime { get; }

    }
}
