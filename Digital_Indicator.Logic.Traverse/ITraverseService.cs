using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Traverse
{
    public interface ITraverseService
    {
        string SpoolRPM { get; set; }
        event EventHandler SpoolRPMChanged;
    }
}
