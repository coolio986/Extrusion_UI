using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Module.Display.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private DelegateCommand closeSettingsView;
        public DelegateCommand CloseSettingsView
        {
            get { return closeSettingsView; }
            set { SetProperty(ref closeSettingsView, value); }
        }


        public SettingsViewModel()
        {
            

        }
    }
}
