using ExtrusionUI.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.FileOperations
{
    public interface ICsvService
    {
        //void SaveSettings(HashSet<DataListXY> dataList, string spoolNumber, string description);
        void SaveSettings(ArrayList dataList, string spoolNumber, string description);
    }
}
