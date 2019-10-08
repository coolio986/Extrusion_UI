using Digital_Indicator.Infrastructure.UI.ControlBase;
using Digital_Indicator.Infrastructure.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.UI_Intelligence
{
    public class SettingItems
    {
        //public ObservableCollection<ViewModelBase> Settings;
        public Dictionary<string, ObservableCollection<ViewModelBase>> Settings;
        private Collection<ViewModelBase> settings;

        private LargeDataInputViewModel Description;
        private DataInputViewModel FilamentDiameter;
        private DataInputViewModel UpperLimit;
        private DataInputViewModel LowerLimit;
        private DataInputViewModel SpoolNumber;
        private DataInputViewModel SpoolerRpm;
        private DoubleInputViewModel TraverseInnerOffset;
        private DoubleInputViewModel TraverseSpoolWidth;
        private DoubleInputViewModel TraverseSpeed;
        private EnumItemsViewModel TraverseRunMode;
        

        public SettingItems()
        {
            //Settings = new ObservableCollection<ViewModelBase>();
            Settings = new Dictionary<string, ObservableCollection<ViewModelBase>>();
            settings = new Collection<ViewModelBase>();

            Description = new LargeDataInputViewModel();
            FilamentDiameter = new DataInputViewModel();
            UpperLimit = new DataInputViewModel();
            LowerLimit = new DataInputViewModel();
            SpoolNumber = new DataInputViewModel();
            SpoolerRpm = new DataInputViewModel();
            TraverseInnerOffset = new DoubleInputViewModel();
            TraverseSpoolWidth = new DoubleInputViewModel();
            TraverseSpeed = new DoubleInputViewModel();
            TraverseRunMode = new EnumItemsViewModel();

            Description.ParameterName = "Description";
            Description.IsXmLParameter = true;
            Description.XmlParameterName = "Description";
            Description.ParameterType = "Info";

            FilamentDiameter.ParameterName = "Filament Diameter";
            FilamentDiameter.IsXmLParameter = true;
            FilamentDiameter.XmlParameterName = "NominalDiameter";
            FilamentDiameter.Unit = "mm";
            FilamentDiameter.ParameterType = "Diameter";

            UpperLimit.ParameterName = "Upper Limit";
            UpperLimit.IsXmLParameter = true;
            UpperLimit.XmlParameterName = "UpperLimit";
            UpperLimit.Unit = "mm";
            UpperLimit.ParameterType = "Diameter";

            LowerLimit.ParameterName = "Lower Limit";
            LowerLimit.IsXmLParameter = true;
            LowerLimit.XmlParameterName = "LowerLimit";
            LowerLimit.Unit = "mm";
            LowerLimit.ParameterType = "Diameter";

            SpoolNumber.ParameterName = "Spool Number";
            SpoolNumber.IsXmLParameter = true;
            SpoolNumber.XmlParameterName = "SpoolNumber";
            SpoolNumber.ParameterType = "Spool";

            SpoolerRpm.HardwareType = "1";
            SpoolerRpm.ParameterName = "Pull Speed";
            SpoolerRpm.IsSerialCommand = true;
            SpoolerRpm.SerialCommand = "velocity";
            SpoolerRpm.Value = "0";
            SpoolerRpm.Unit = "rpm";
            SpoolerRpm.ParameterType = "Pulling";

            TraverseInnerOffset.HardwareType = "3";
            TraverseInnerOffset.ParameterName = "Traverse Inner Offset";
            TraverseInnerOffset.IsSerialCommand = true;
            TraverseInnerOffset.SerialCommand = "InnerOffset";
            TraverseInnerOffset.Value = "0";
            TraverseInnerOffset.Unit = "mm";
            TraverseInnerOffset.ParameterType = "Traverse";

            TraverseSpoolWidth.HardwareType = "3";
            TraverseSpoolWidth.ParameterName = "Spool Width";
            TraverseSpoolWidth.IsSerialCommand = true;
            TraverseSpoolWidth.SerialCommand = "SpoolWidth";
            TraverseSpoolWidth.Value = "0";
            TraverseSpoolWidth.Unit = "mm";
            TraverseSpoolWidth.ParameterType = "Spool";

            TraverseSpeed.HardwareType = "3";
            TraverseSpeed.ParameterName = "Traverse Speed";
            TraverseSpeed.IsSerialCommand = true;
            TraverseSpeed.SerialCommand = "TraverseRPM";
            TraverseSpeed.Value = "0";
            TraverseSpeed.Unit = "rpm";
            TraverseSpeed.ParameterType = "Traverse";

            TraverseRunMode.HardwareType = "3";
            TraverseRunMode.ParameterName = "Traverse Run Mode";
            TraverseRunMode.IsSerialCommand = true;
            TraverseRunMode.SerialCommand = "RunMode";
            TraverseRunMode.ItemIndex = 0;
            TraverseRunMode.Value = 0;
            TraverseRunMode.ParameterType = "Traverse";

            EnumItem home = new EnumItem() { ItemValue = "Home", ItemValueID = "0" };
            EnumItem stop = new EnumItem() { ItemValue = "Stop", ItemValueID = "1" };
            EnumItem manual = new EnumItem() { ItemValue = "Manual", ItemValueID = "2" };
            EnumItem semiManual = new EnumItem() { ItemValue = "Semi Manual", ItemValueID = "3" };
            EnumItem fullAuto = new EnumItem() { ItemValue = "Full Auto", ItemValueID = "4" };

            TraverseRunMode.EnumList = new ObservableCollection<EnumItem>();
            TraverseRunMode.EnumList.Add(home);
            TraverseRunMode.EnumList.Add(stop);
            TraverseRunMode.EnumList.Add(manual);
            TraverseRunMode.EnumList.Add(semiManual);
            TraverseRunMode.EnumList.Add(fullAuto);





            //Settings.Add(Description.ParameterType, Description);
            //Settings.Add(FilamentDiameter.ParameterType, FilamentDiameter);
            //Settings.Add(UpperLimit.ParameterType, UpperLimit);
            //Settings.Add(LowerLimit.ParameterType, LowerLimit);
            //Settings.Add(SpoolNumber.ParameterType, SpoolNumber);
            //Settings.Add(TraverseInnerOffset.ParameterType, TraverseInnerOffset);
            //Settings.Add(TraverseSpoolWidth.ParameterType, TraverseSpoolWidth);
            //Settings.Add(SpoolerRpm.ParameterType, SpoolerRpm);
            //Settings.Add(TraverseRunMode.ParameterType, TraverseRunMode);


            settings.Add(Description);
            settings.Add(FilamentDiameter);
            settings.Add(UpperLimit);
            settings.Add(LowerLimit);
            settings.Add(SpoolNumber);
            settings.Add(TraverseInnerOffset);
            settings.Add(TraverseSpoolWidth);
            settings.Add(SpoolerRpm);
            settings.Add(TraverseSpeed);
            settings.Add(TraverseRunMode);
            

            foreach (ViewModelBase item in settings)
            {
                if (Settings.ContainsKey(item.ParameterType))
                {
                    Settings[item.ParameterType].Add(item);
                }
                else
                {
                    Settings.Add(item.ParameterType, new ObservableCollection<ViewModelBase>() { item });
                }

            }
        }
    }
}
