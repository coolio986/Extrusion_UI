using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Digital_Indicator.Infrastructure.UI.Controls;
using Digital_Indicator.Logic.Filament;
using Digital_Indicator.Logic.SerialCommunications;
using Prism.Commands;
using Prism.Mvvm;
using System.Reflection;
using System.Threading;

namespace Digital_Indicator.Logic.UI_Intelligence
{
    public class UI_IntelligenceService : BindableBase, IUI_IntelligenceService
    {
        public event EventHandler SettingsUpdated;
        private ISerialService _serialService;
        private IFilamentService _filamentService;
        private SettingItems items;

        private Collection<ViewModelBase> Settings { get; set; }

        public UI_IntelligenceService(ISerialService serialService, IFilamentService filamentService)
        {
            _serialService = serialService;
            _filamentService = filamentService;


            items = new SettingItems();
            Settings = new Collection<ViewModelBase>();
            //Settings = items.Settings; //new ObservableCollection<ViewModelBase>();

            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            foreach (KeyValuePair<string, ObservableCollection<ViewModelBase>> item in items.Settings)
            {
                foreach(ViewModelBase vmb in item.Value)
                {
                    if (vmb.IsXmLParameter)
                    {
                        PropertyInfo prop = _filamentService.GetType().GetProperty(vmb.XmlParameterName, BindingFlags.Public | BindingFlags.Instance);
                        vmb.Value = _filamentService.FilamentServiceVariables[vmb.XmlParameterName];

                        if (prop != null)
                        {
                            vmb.Value = prop.GetValue(_filamentService, null);
                        }
                    }
                    Settings.Add(vmb);
                }

            }

            _serialService.SendSerialData(new SerialCommand() { Command = "GetFullUpdate", DeviceID = "100" });

            foreach (ViewModelBase item in Settings)
            {

                item.PropertyChanged += ItemChange_Handler;
                item.EnterCommand = new DelegateCommand<ViewModelBase>(UpdateItem);
            }
        }


        private void _filamentService_PropertyChanged(object sender, EventArgs e)
        {
            SerialCommand command = (SerialCommand)sender;

            if (items != null)
            {
                foreach (ViewModelBase item in Settings)
                {
                    if (item.SerialCommand == command.Command && item.IsSerialCommand)
                    {
                        item.PropertyChanged -= ItemChange_Handler;
                        if (item.Value.ToString() != command.Value)
                        {
                            if (item.GetType() == typeof(EnumItemsViewModel))
                            {
                                ((EnumItemsViewModel)item).ItemIndex = Int32.Parse(command.Value);
                            }
                            else
                            {
                                if ((string)item.Value != command.Value)
                                {
                                    item.Value = command.Value;
                                    
                                }
                            }
                        }
                        item.PropertyChanged += ItemChange_Handler;
                        break;
                    }
                }
            }


        }

        //public IReadOnlyCollection<ViewModelBase> GetSettings()
        //{
        //    return Settings;
        //}

        public Dictionary<string, ObservableCollection<ViewModelBase>> GetSettings()
        {
            return items.Settings;
        }

        public void SaveSettings()
        {
            SettingsUpdated?.Invoke(null, new EventArgs());
        }

        private void UpdateItem(ViewModelBase sender)
        {
            ItemChange_Handler(sender, null);
        }

        private void ItemChange_Handler(object sender, EventArgs e)
        {
            ViewModelBase objectItem = (ViewModelBase)sender;

            //TO DO needs refactor, not the right place for this
            string itemValue = objectItem.Value.ToString();

            //if (objectItem.SerialCommand == "velocity")
            //{
            //    itemValue = (-Math.Abs((float)Convert.ChangeType(objectItem.Value.ToString(), typeof(float)))).ToString();
            //}



            if (objectItem.HardwareType != string.Empty)
            {
                _serialService.SendSerialData(new SerialCommand() { Command = objectItem.SerialCommand, Value = itemValue, DeviceID = objectItem.HardwareType });
            }

            if (objectItem.IsXmLParameter)
            {

                _filamentService.FilamentServiceVariables[objectItem.XmlParameterName] = objectItem.Value.ToString();
                _filamentService.SaveXmlData();
                
                //PropertyInfo prop = _filamentService.GetType().GetProperty(objectItem.XmlParameterName, BindingFlags.Public | BindingFlags.Instance);

                //if (prop != null)
                //{
                //    prop.SetValue(_filamentService, objectItem.Value, null);
                //}

            }

        }


    }
}
