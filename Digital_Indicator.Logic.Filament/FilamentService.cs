using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Filament
{
    public class FilamentService : IFilamentService
    {
        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string nominalFilamentDiameter = "1.75";
        public string NominalDiameter
        {
            get { return nominalFilamentDiameter; }
            set { nominalFilamentDiameter = value; }
        }

        private string upperLimit;
        public string UpperLimit
        {
            get { return upperLimit; }
            set { upperLimit = value; }
        }

        private string lowerLimit;
        public string LowerLimit
        {
            get { return lowerLimit; }
            set { lowerLimit = value; }
        }
    }
}
