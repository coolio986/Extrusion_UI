using ExtrusionUI.Infrastructure.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.UI_Intelligence
{
    public interface IUI_IntelligenceService
    {

        void SaveSettings();

        //IReadOnlyCollection<ViewModelBase> GetSettings();
        Dictionary<string, ObservableCollection<ViewModelBase>> GetSettings();
        ObservableCollection<ViewModelBase> GetErrors();


        event EventHandler SettingsUpdated;

    }
}
