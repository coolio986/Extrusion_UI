using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Digital_Indicator.Toolkits.Wpf
{
    /// <summary>
    /// Helper methods for font handling.
    /// </summary>
    public static class FontHelper
    {
        /// <summary>
        /// Finds first available font family from a given list.
        /// </summary>
        /// <param name="names">A list of font family names to search.</param>
        /// <returns>Font family, or null if neither was found.</returns>
        public static FontFamily FindFontFamily(params string[] names)
        {
            foreach (string name in names)
            {
                var fontFamily = Fonts.SystemFontFamilies.SingleOrDefault(f => f.Source == name);

                if (fontFamily != null)
                    return fontFamily;
            }

            return null;
        }
    }
}
