using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExtrusionUI.Infrastructure.UI.Controls;
using ExtrusionUI.Logic.Filament;
using ExtrusionUI.Logic.SerialCommunications;
using Prism.Commands;
using Prism.Mvvm;
using System.Reflection;
using System.Windows;
using ExtrusionUI.Logic.FileOperations;
using ExtrusionUI.Core;

namespace ExtrusionUI.Logic.UI_Intelligence
{
    public class UI_IntelligenceService : BindableBase, IUI_IntelligenceService
    {
        public event EventHandler SettingsUpdated;
        private ISerialService _serialService;
        private IFilamentService _filamentService;
        private SettingItems items;

        private ObservableCollection<ViewModelBase> Errors;

        private Collection<ViewModelBase> Settings { get; set; }

        public UI_IntelligenceService(ISerialService serialService, IFilamentService filamentService, IFileService fileService)
        {
            _serialService = serialService;
            _filamentService = filamentService;


            items = new SettingItems(fileService);
            Settings = new Collection<ViewModelBase>();
            Errors = new ObservableCollection<ViewModelBase>();

            //Settings = items.Settings; //new ObservableCollection<ViewModelBase>();

            _filamentService.PropertyChanged += _filamentService_PropertyChanged;

            foreach (KeyValuePair<string, ObservableCollection<ViewModelBase>> item in items.Settings)
            {
                foreach (ViewModelBase vmb in item.Value)
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

            //_serialService.SendSerialData(new SerialCommand() { Command = "GetFullUpdate", DeviceID = "100" });

            foreach (ViewModelBase item in Settings.Where(x => x.IsSerialCommand))
            {

                item.PropertyChanged += ItemChange_Handler;
                item.EnterCommand = new DelegateCommand<ViewModelBase>(UpdateItem);


                _serialService.SendSerialData(new SerialCommand() { Command = "Get" + item.SerialCommand, DeviceID = item.HardwareType });
                //break;

                //if (item.GetType() == typeof(ButtonPressViewModel))
                //{
                //    ((ButtonPressViewModel)item).ButtonCommand = new DelegateCommand<ButtonPressViewModel>(OnButtonPressed);
                //}
            }
            foreach (ViewModelBase item in Settings.Where(x => !x.IsSerialCommand))
            {
                if (item.GetType() == typeof(ButtonPressViewModel))
                {
                    ((ButtonPressViewModel)item).ButtonCommand = new DelegateCommand<ButtonPressViewModel>(OnButtonPressed);
                }
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

                        if (item.Value == null) { item.Value = ""; }

                        if (item.Value.ToString() != command.Value)
                        {
                            if (item.GetType() == typeof(EnumItemsViewModel))
                            {
                                ((EnumItemsViewModel)item).ItemIndex = Int32.Parse(command.Value);
                            }
                            else
                            {
                                if ((string)item.Value != (string)command.Value)
                                {
                                    item.Value = command.Value;

                                }
                            }
                        }
                        item.PropertyChanged += ItemChange_Handler;
                        break;
                    }
                }

                ViewModelBase spoolNumberItem = Settings.First(x => x.XmlParameterName == StaticStrings.SPOOLNUMBER);

                //TODO fix this, doesnt belong here
                spoolNumberItem.PropertyChanged -= ItemChange_Handler;
                spoolNumberItem.Value = _filamentService.FilamentServiceVariables[StaticStrings.SPOOLNUMBER];
                spoolNumberItem.PropertyChanged += ItemChange_Handler;
            }

            if (!Errors.Any(x => (string)x.Value == command.Value) && command.Command == "DiameterError")
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
              Errors.Add(new ErrorItemViewModel() { Value = command.Value })
                ));


                //  Dispatcher.CurrentDispatcher.Invoke((Action)(() =>
                //Errors.Add(new ErrorItemViewModel() { Value = command.Value })
                //  ));
            }


        }

        public Dictionary<string, ObservableCollection<ViewModelBase>> GetSettings()
        {
            return items.Settings;
        }

        public ObservableCollection<ViewModelBase> GetErrors()
        {
            return Errors;
        }

        public void SaveSettings()
        {
            SettingsUpdated?.Invoke(null, new EventArgs());
        }

        private void UpdateItem(ViewModelBase sender)
        {
            //ItemChange_Handler(sender, null);
        }
        private void OnButtonPressed(ViewModelBase sender)
        {
            ButtonPressed_Handler(sender, null);
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



            if (objectItem.HardwareType != string.Empty && objectItem.IsSerialCommand)
            {
                _serialService.SendSerialData(new SerialCommand() { Command = "Set" + objectItem.SerialCommand, Value = itemValue, DeviceID = objectItem.HardwareType });
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

        private void ButtonPressed_Handler(object sender, EventArgs e)
        {
            ButtonPressViewModel objectItem = (ButtonPressViewModel)sender;

            if (objectItem.ButtonType == Infrastructure.UI.ButtonTypes.OpenFolderType)
            {
                System.Diagnostics.Process.Start(objectItem.ButtonPathLocation);
            }



        }


    }
}
