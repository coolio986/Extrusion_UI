using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Digital_Indicator.Logic.Helpers;

namespace Digital_Indicator.Toolkits.Wpf
{
    public class MM_Converter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) throw new NotImplementedException();
            var doubleValue = double.Parse(value.ToString());
            return String.Format("{0:N}", (double)doubleValue / 1000);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = 0;
            double.TryParse(value.ToString(), out doubleValue);
            return String.Format("{0}", (int)(doubleValue * 1000));
        }
    }
}
