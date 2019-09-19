using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Digital_Indicator.Infrastructure.UI.Controls;
using Digital_Indicator.Logic.SerialCommunications;
using Prism.Commands;
using Prism.Mvvm;


namespace Digital_Indicator.Logic.UI_Intelligence
{
    public class UI_IntelligenceService : BindableBase, IUI_IntelligenceService
    {
        public event EventHandler SettingsUpdated;
        private ISerialService _serialService;

        private Collection<ViewModelBase> Settings { get; set; }

        public UI_IntelligenceService(ISerialService serialService)
        {
            _serialService = serialService;

            SettingItems items = new SettingItems();

            Settings = items.Settings; //new ObservableCollection<ViewModelBase>();

            foreach (ViewModelBase item in items.Settings)
            {
                item.PropertyChanged += ItemChange_Handler;
                item.EnterCommand = new DelegateCommand<ViewModelBase>(UpdateItem);
                
            }
        }


        public IReadOnlyCollection<ViewModelBase> GetSettings()
        {
            return Settings;
        }

        public void SaveSettings()
        {
            SettingsUpdated?.Invoke(null, new EventArgs());
        }

        private void UpdateItem(ViewModelBase sender)
        {
            ItemChange_Handler(sender, null);
        }

        private void ItemChange_Handler (object sender, EventArgs e)
        {
            ViewModelBase objectItem = (ViewModelBase)sender;


            //TO DO needs refactor, not the right place for this
            string itemValue = objectItem.Value.ToString();

            if (objectItem.SerialCommand == "velocity")
            {
                itemValue = (-Math.Abs((float)Convert.ChangeType(objectItem.Value.ToString(), typeof(float)))).ToString();
            }
            


            if (objectItem.HardwareType != string.Empty)
            {
                _serialService.SendSerialData(new SerialCommand() { Command = objectItem.SerialCommand, Value = itemValue, DeviceID = objectItem.HardwareType});
            }

        }

        
    }
}
