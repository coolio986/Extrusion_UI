using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Toolkits.PrismExtensions
{
    public class DependentViewAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type of the dependent view.
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Gets or sets the region name, to which dependent view should be loaded.
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating, if the dependee should
        /// get the same view model, as the main view.
        /// </summary>
        public bool ShareViewModel { get; set; }

        public DependentViewAttribute(Type viewType, string regionName, bool shareViewModel = false)
        {
            this.ViewType = viewType;
            this.RegionName = regionName;
            this.ShareViewModel = shareViewModel;
        }
    }
}
