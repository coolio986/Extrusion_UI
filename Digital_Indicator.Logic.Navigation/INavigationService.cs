using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.Navigation
{
    public interface INavigationService
    {

        void NavigateTo(string screenName);
    }
}
