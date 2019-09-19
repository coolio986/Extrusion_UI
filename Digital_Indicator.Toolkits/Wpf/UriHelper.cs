using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Toolkits.Wpf
{
    public static class UriHelper
    {
        /// <summary>
        /// Gets resource URI for a resource in calling assembly.
        /// </summary>
        public static string GetResourceUri(string relativePath)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            string assemblyName = assembly.GetName().Name;

            string uri = string.Format("pack://application:,,,/{0};component/{1}", assemblyName, relativePath);
            return uri;
        }
    }
}
