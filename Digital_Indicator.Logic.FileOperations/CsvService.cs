﻿using Digital_Indicator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.FileOperations
{
    public class CsvService : ICsvService
    {
        IFileService _fileService;
        List<DataListXY> DataList { get; }


        public CsvService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void SaveSettings(List<DataListXY> dataList, string spoolNumber, string description)
        {
            string csvString = string.Empty;
            csvString += "Timestamp, Diameter,\r\n";

            foreach (DataListXY list in dataList)
            {
                csvString += list.X.ToString() + "," + list.Y.ToString() + ",\r\n";
            }
            csvString = csvString.TrimEnd(','); //remove trailing comma

            string fileName = DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Year.ToString("0000") + 
                "_" + description + "_" + "Spool" + spoolNumber + ".csv";

            _fileService.WriteFile(_fileService.EnvironmentDirectory + @"\" + fileName, csvString);
        }
    }
}
