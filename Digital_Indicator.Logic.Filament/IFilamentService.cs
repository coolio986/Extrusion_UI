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
        string NominalDiameter { get; set; }
        string UpperLimit { get; set; }
        string LowerLimit { get; set; }
        
    }
}
