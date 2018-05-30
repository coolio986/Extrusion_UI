using Digital_Indicator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.FileOperations
{
    public interface ICsvService
    {
        void SaveSettings(List<DataListXY> dataList, string spoolNumber, string description);
    }
}
