using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Toolkits.Wpf
{
    public static class Converter
    {
        public static FalseToCollapsedConverter FalseToCollapsedConverter = new FalseToCollapsedConverter();
        public static TrueToCollapsedConverter TrueToCollapsedConverter = new TrueToCollapsedConverter();
    }
}
