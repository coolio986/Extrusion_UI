using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.WindowForms.ZedGraphUserControl
{
    public class ZedGraphPlotModel
    {
        static Dictionary<string, ZedGraphUserControl> plotModelDict;

        public static ZedGraphUserControl GetPlot(string plotName)
        {
            return plotModelDict[plotName];
        }

        public static List<ZedGraphUserControl> GetPlots()
        {
            return plotModelDict.Select(x => { return x.Value; }).ToList();
        }

        public static void CreatePlots(string upperLimit, string nominalDiameter, string lowerLimit)
        {
            if (plotModelDict != null)
            {
                plotModelDict.Select(x =>
                {
                    x.Value.ClearPlots();
                    return x;
                }).ToList();
            }
            else
            {
                plotModelDict = new Dictionary<string, ZedGraphUserControl>();

                plotModelDict.Add("HistoricalModel",
                 new ZedGraphUserControl()
                 {
                     Title = "Historical Diameter",
                     UpperLimitDiameter = upperLimit,
                     NominalDiameter = nominalDiameter,
                     LowerLimitDiameter = lowerLimit,
                     IsHistorical = true,
                 });

                plotModelDict.Add("RealTimeModel",
                 new ZedGraphUserControl()
                 {
                     Title = "RealTime Diameter",
                     UpperLimitDiameter = upperLimit,
                     NominalDiameter = nominalDiameter,
                     LowerLimitDiameter = lowerLimit,
                 });
            }
        }
    }
}
