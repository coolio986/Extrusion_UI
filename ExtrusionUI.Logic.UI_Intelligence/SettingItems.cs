﻿using ExtrusionUI.Infrastructure.UI.ControlBase;
using ExtrusionUI.Infrastructure.UI.Controls;
using ExtrusionUI.Logic.FileOperations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.UI_Intelligence
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
        private DataInputViewModel SpecificGravity;
        private DataInputViewModel SpoolWeightLimit;
        private EnumItemsViewModel TraverseStartPosition;
        private LargeDataInputViewModel MotherboardRestartReason;
        private ButtonPressViewModel OpenSpoolerFolder;

        IFileService _fileService;



        public SettingItems(IFileService fileService)
        {
            _fileService = fileService;
            //Settings = new ObservableCollection<ViewModelBase>();
            Settings = new Dictionary<string, ObservableCollection<ViewModelBase>>();
            settings = new Collection<ViewModelBase>();

            //Description = new LargeDataInputViewModel();
            FilamentDiameter = new DataInputViewModel();
            UpperLimit = new DataInputViewModel();
            LowerLimit = new DataInputViewModel();
            SpoolNumber = new DataInputViewModel();
            SpoolerRpm = new DataInputViewModel();
            TraverseInnerOffset = new DoubleInputViewModel();
            TraverseSpoolWidth = new DoubleInputViewModel();
            TraverseSpeed = new DoubleInputViewModel();
            TraverseRunMode = new EnumItemsViewModel();
            SpecificGravity = new DataInputViewModel();
            SpoolWeightLimit = new DataInputViewModel();
            TraverseStartPosition = new EnumItemsViewModel();
            MotherboardRestartReason = new LargeDataInputViewModel();
            OpenSpoolerFolder = new ButtonPressViewModel();

            //Description.ParameterName = "Description";
            //Description.IsXmLParameter = true;
            //Description.XmlParameterName = "Description";
            //Description.ParameterType = "Production";

            FilamentDiameter.HardwareType = "100";
            FilamentDiameter.ParameterName = "Filament Diameter";
            FilamentDiameter.IsXmLParameter = true;
            FilamentDiameter.XmlParameterName = "NominalDiameter";
            FilamentDiameter.Unit = "mm";
            FilamentDiameter.ParameterType = "Production";
            FilamentDiameter.Value = "0.00";
            FilamentDiameter.IsSerialCommand = true;
            FilamentDiameter.SerialCommand = "NominalDiameter";

            UpperLimit.HardwareType = "100";
            UpperLimit.ParameterName = "Upper Limit";
            UpperLimit.IsXmLParameter = true;
            UpperLimit.XmlParameterName = "UpperLimit";
            UpperLimit.Unit = "mm";
            UpperLimit.ParameterType = "Production";
            UpperLimit.Value = "0.00";
            UpperLimit.IsSerialCommand = true;
            UpperLimit.SerialCommand = "UpperLimit";

            LowerLimit.HardwareType = "100";
            LowerLimit.ParameterName = "Lower Limit";
            LowerLimit.IsXmLParameter = true;
            LowerLimit.XmlParameterName = "LowerLimit";
            LowerLimit.Unit = "mm";
            LowerLimit.ParameterType = "Production";
            LowerLimit.Value = "0.00";
            LowerLimit.IsSerialCommand = true;
            LowerLimit.SerialCommand = "LowerLimit";

            SpoolNumber.ParameterName = "Spool Number";
            SpoolNumber.IsXmLParameter = true;
            SpoolNumber.XmlParameterName = "SpoolNumber";
            SpoolNumber.ParameterType = "Production";

            SpoolerRpm.HardwareType = "100";
            SpoolerRpm.ParameterName = "Pull Rpm";
            SpoolerRpm.IsSerialCommand = true;
            SpoolerRpm.SerialCommand = "velocity";
            SpoolerRpm.Value = "0";
            SpoolerRpm.Unit = "rpm";
            SpoolerRpm.ParameterType = "Production";

            SpecificGravity.HardwareType = "100";
            SpecificGravity.ParameterName = "Specific Gravity";
            SpecificGravity.IsSerialCommand = true;
            SpecificGravity.SerialCommand = "SpecificGravity";
            SpecificGravity.Value = "0";
            SpecificGravity.ParameterType = "Production";
            //SpecificGravity.IsXmLParameter = true;
            SpecificGravity.Unit = "g/cc";
            //SpecificGravity.XmlParameterName = "SpecificGravity";

            SpoolWeightLimit.HardwareType = "100";
            SpoolWeightLimit.ParameterName = "Spool Weight Limit";
            SpoolWeightLimit.Value = "0";
            SpoolWeightLimit.ParameterType = "Production";
            SpoolWeightLimit.IsXmLParameter = true;
            SpoolWeightLimit.XmlParameterName = "SpoolWeightLimit";
            SpoolWeightLimit.Unit = "g";
            SpoolWeightLimit.IsSerialCommand = true;
            SpoolWeightLimit.SerialCommand = "SpoolWeightLimit";

            TraverseInnerOffset.HardwareType = "3";
            TraverseInnerOffset.ParameterName = "Traverse Inner Offset";
            TraverseInnerOffset.IsSerialCommand = true;
            TraverseInnerOffset.SerialCommand = "InnerOffset";
            TraverseInnerOffset.Value = "0";
            TraverseInnerOffset.Unit = "mm";
            TraverseInnerOffset.ParameterType = "Machine";

            TraverseSpoolWidth.HardwareType = "3";
            TraverseSpoolWidth.ParameterName = "Spool Width";
            TraverseSpoolWidth.IsSerialCommand = true;
            TraverseSpoolWidth.SerialCommand = "SpoolWidth";
            TraverseSpoolWidth.Value = "0";
            TraverseSpoolWidth.Unit = "mm";
            TraverseSpoolWidth.ParameterType = "Machine";

            TraverseSpeed.HardwareType = "3";
            TraverseSpeed.ParameterName = "Traverse Speed";
            TraverseSpeed.IsSerialCommand = true;
            TraverseSpeed.SerialCommand = "TraverseRPM";
            TraverseSpeed.Value = "0";
            TraverseSpeed.Unit = "rpm";
            TraverseSpeed.ParameterType = "Machine";

            TraverseRunMode.HardwareType = "3";
            TraverseRunMode.ParameterName = "Traverse Run Mode";
            TraverseRunMode.IsSerialCommand = true;
            TraverseRunMode.SerialCommand = "RunMode";
            TraverseRunMode.ItemIndex = 0;
            TraverseRunMode.Value = 0;
            TraverseRunMode.ParameterType = "Machine";

            

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




            TraverseStartPosition.HardwareType = "3";
            TraverseStartPosition.ParameterName = "Traverse Start";
            TraverseStartPosition.IsSerialCommand = true;
            TraverseStartPosition.SerialCommand = "StartPosition";
            TraverseStartPosition.ItemIndex = 0;
            TraverseStartPosition.Value = 0;
            TraverseStartPosition.ParameterType = "Machine";


            EnumItem back = new EnumItem() { ItemValue = "Back", ItemValueID = "0" };
            EnumItem middle = new EnumItem() { ItemValue = "Middle", ItemValueID = "1" };
            EnumItem front = new EnumItem() { ItemValue = "Front", ItemValueID = "2" };
            
            TraverseStartPosition.EnumList = new ObservableCollection<EnumItem>();
            TraverseStartPosition.EnumList.Add(back);
            TraverseStartPosition.EnumList.Add(middle);
            TraverseStartPosition.EnumList.Add(front);

            MotherboardRestartReason.ParameterName = "Motherboard Restart Reason";
            MotherboardRestartReason.ParameterType = "Debug";
            MotherboardRestartReason.IsSerialCommand = true;
            MotherboardRestartReason.SerialCommand = "MotherboardRestartReason";

            OpenSpoolerFolder.ParameterName = "Open Spool Folder";
            OpenSpoolerFolder.ParameterType = "Production";
            OpenSpoolerFolder.IsXmLParameter = false;
            OpenSpoolerFolder.ButtonType = Infrastructure.UI.ButtonTypes.OpenFolderType;
            OpenSpoolerFolder.ButtonPathLocation = _fileService.EnvironmentDirectory;


            //settings.Add(Description);
            settings.Add(FilamentDiameter);
            settings.Add(UpperLimit);
            settings.Add(LowerLimit);
            settings.Add(SpoolNumber);
            settings.Add(TraverseInnerOffset);
            settings.Add(TraverseSpoolWidth);
            settings.Add(SpoolerRpm);
            settings.Add(TraverseSpeed);
            settings.Add(TraverseRunMode);
            settings.Add(SpecificGravity);
            settings.Add(SpoolWeightLimit);
            settings.Add(TraverseStartPosition);
            settings.Add(MotherboardRestartReason);
            settings.Add(OpenSpoolerFolder);


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
