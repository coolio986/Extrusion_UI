using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Toolkits.PrismExtensions
{
    public class DependentView
    {
        /// <summary>
        /// Gets or sets the view, which was created.
        /// </summary>
        public object View { get; set; }

        /// <summary>
        /// Gets or sets the region name, which the view was loaded to.
        /// </summary>
        public string RegionName { get; set; }
    }
}
