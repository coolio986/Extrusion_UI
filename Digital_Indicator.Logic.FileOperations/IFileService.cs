using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.FileOperations
{
    public interface IFileService
    {
        string EnvironmentDirectory { get; }
        string ReadFile(string filename);
        void WriteFile(string filename, string filedata);
    }
}
