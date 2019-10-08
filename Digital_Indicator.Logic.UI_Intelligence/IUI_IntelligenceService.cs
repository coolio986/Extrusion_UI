using Digital_Indicator.Infrastructure.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.UI_Intelligence
{
    public interface IUI_IntelligenceService
    {

        void SaveSettings();

        //IReadOnlyCollection<ViewModelBase> GetSettings();
        Dictionary<string, ObservableCollection<ViewModelBase>> GetSettings();


        event EventHandler SettingsUpdated;

    }
}
