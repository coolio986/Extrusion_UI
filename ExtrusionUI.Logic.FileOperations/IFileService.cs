using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.FileOperations
{
    public interface IFileService
    {
        string EnvironmentDirectory { get; }
        string ReadFile(string filename);
        void WriteFile(string filename, string filedata, int retryCounts = 0);
        void AppendFile(string filename, string filedata);
        void AppendLog(string filedata);
    }
}
