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

        public CsvService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void SaveSettings()
        {

        }
    }
}
