using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Helpers
{
    public static class ExtensionHelpers
    {
        public static int GetInteger(this object value)
        {
            return (int)Convert.ChangeType(value, typeof(int));
        }

        public static double GetDouble(this object value)
        {
            return (double)Convert.ChangeType(value, typeof(double));
        }

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
