﻿using ExtrusionUI.Infrastructure.UI.ControlBase;
using ExtrusionUI.Infrastructure.UI.Controls;
using ExtrusionUI.Logic.FileOperations;
using ExtrusionUI.Core;
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
        private DataInputViewModel TraverseHomeOffset;
        private DataInputViewModel TraverseSpoolWidth;
        private DataInputViewModel TraverseLeadWidth;
        private DataInputViewModel TraverseSpeed;
        private DataInputViewModel TraverseLeadSpeed;
        private EnumItemsViewModel TraverseRunMode;
        private DataInputViewModel SpecificGravity;
        private DataInputViewModel SpoolWeightLimit;
        private EnumItemsViewModel TraverseStartPosition;
        private LargeDataInputViewModel MotherboardRestartReason;
        private ButtonPressViewModel OpenSpoolerFolder;
        private DataInputViewModel TraverseMotionStatus;

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
            TraverseHomeOffset = new DataInputViewModel();
            TraverseSpoolWidth = new DataInputViewModel();
            TraverseLeadWidth = new DataInputViewModel();
            TraverseSpeed = new DataInputViewModel();
            TraverseLeadSpeed = new DataInputViewModel();
            TraverseRunMode = new EnumItemsViewModel();
            SpecificGravity = new DataInputViewModel();
            SpoolWeightLimit = new DataInputViewModel();
            TraverseStartPosition = new EnumItemsViewModel();
            MotherboardRestartReason = new LargeDataInputViewModel();
            OpenSpoolerFolder = new ButtonPressViewModel();
            TraverseMotionStatus = new DataInputViewModel();

            //Description.ParameterName = "Description";
            //Description.IsXmLParameter = true;
            //Description.XmlParameterName = "Description";
            //Description.ParameterType = "Production";

            FilamentDiameter.HardwareType = "3";
            FilamentDiameter.ParameterName = "Filament Diameter";
            FilamentDiameter.IsXmLParameter = true;
            FilamentDiameter.XmlParameterName = StaticStrings.FILAMENTNOMINALDIAMETER;
            FilamentDiameter.Unit = "mm";
            FilamentDiameter.ParameterType = "Production";
            FilamentDiameter.Value = "0.00";
            FilamentDiameter.IsSerialCommand = true;
            FilamentDiameter.SerialCommand = StaticStrings.FILAMENTNOMINALDIAMETER;

            UpperLimit.HardwareType = "3";
            UpperLimit.ParameterName = "Upper Limit";
            UpperLimit.IsXmLParameter = true;
            UpperLimit.XmlParameterName = StaticStrings.FILAMENTUPPERLIMIT;
            UpperLimit.Unit = "mm";
            UpperLimit.ParameterType = "Production";
            UpperLimit.Value = "0.00";
            UpperLimit.IsSerialCommand = true;
            UpperLimit.SerialCommand = StaticStrings.FILAMENTUPPERLIMIT;

            LowerLimit.HardwareType = "3";
            LowerLimit.ParameterName = "Lower Limit";
            LowerLimit.IsXmLParameter = true;
            LowerLimit.XmlParameterName = StaticStrings.FILAMENTLOWERLIMIT;
            LowerLimit.Unit = "mm";
            LowerLimit.ParameterType = "Production";
            LowerLimit.Value = "0.00";
            LowerLimit.IsSerialCommand = true;
            LowerLimit.SerialCommand = StaticStrings.FILAMENTLOWERLIMIT;

            SpoolNumber.ParameterName = "Spool Number";
            SpoolNumber.IsXmLParameter = true;
            SpoolNumber.XmlParameterName = StaticStrings.SPOOLNUMBER;
            SpoolNumber.ParameterType = "Production";

            SpoolerRpm.HardwareType = "3";
            SpoolerRpm.ParameterName = "Pull Rpm";
            SpoolerRpm.IsSerialCommand = false;
            SpoolerRpm.SerialCommand = "velocity";
            SpoolerRpm.Value = "0";
            SpoolerRpm.Unit = "rpm";
            SpoolerRpm.ParameterType = "Production";

            SpecificGravity.HardwareType = "3";
            SpecificGravity.ParameterName = "Specific Gravity";
            SpecificGravity.IsSerialCommand = false;
            SpecificGravity.SerialCommand = "SpecificGravity";
            SpecificGravity.Value = "0";
            SpecificGravity.ParameterType = "Production";
            //SpecificGravity.IsXmLParameter = true;
            SpecificGravity.Unit = "g/cc";
            //SpecificGravity.XmlParameterName = "SpecificGravity";

            SpoolWeightLimit.HardwareType = "3";
            SpoolWeightLimit.ParameterName = "Spool Weight Limit";
            SpoolWeightLimit.Value = "0";
            SpoolWeightLimit.ParameterType = "Production";
            SpoolWeightLimit.IsXmLParameter = true;
            SpoolWeightLimit.XmlParameterName = StaticStrings.SPOOLWEIGHTLIMIT;
            SpoolWeightLimit.Unit = "g";
            SpoolWeightLimit.IsSerialCommand = false;
            SpoolWeightLimit.SerialCommand = StaticStrings.SPOOLWEIGHTLIMIT;

            TraverseHomeOffset.HardwareType = "3";
            TraverseHomeOffset.ParameterName = "Traverse Home Offset";
            TraverseHomeOffset.IsSerialCommand = true;
            TraverseHomeOffset.SerialCommand = StaticStrings.TRAVERSEHOMEOFFSET;
            TraverseHomeOffset.Value = "0.00";
            TraverseHomeOffset.Unit = "mm";
            TraverseHomeOffset.ParameterType = StaticStrings.TRAVERSE;

            TraverseSpoolWidth.HardwareType = "3";
            TraverseSpoolWidth.ParameterName = "Spool Width";
            TraverseSpoolWidth.IsSerialCommand = true;
            TraverseSpoolWidth.SerialCommand = StaticStrings.SPOOLWIDTH;
            TraverseSpoolWidth.Value = "0.00";
            TraverseSpoolWidth.Unit = "mm";
            TraverseSpoolWidth.ParameterType = StaticStrings.TRAVERSE;

            TraverseLeadWidth.HardwareType = "3";
            TraverseLeadWidth.ParameterName = "Traverse Lead Width";
            TraverseLeadWidth.IsSerialCommand = true;
            TraverseLeadWidth.SerialCommand = "TraverseLeadWidth";
            TraverseLeadWidth.Value = "0.00";
            TraverseLeadWidth.Unit = "mm";
            TraverseLeadWidth.ParameterType = StaticStrings.TRAVERSE;

            TraverseSpeed.HardwareType = "3";
            TraverseSpeed.ParameterName = "Traverse Speed";
            TraverseSpeed.IsSerialCommand = true;
            TraverseSpeed.SerialCommand = "TraverseRPM";
            TraverseSpeed.Value = "0.00";
            TraverseSpeed.Unit = "rpm";
            TraverseSpeed.ParameterType = StaticStrings.TRAVERSE;

            TraverseLeadSpeed.HardwareType = "3";
            TraverseLeadSpeed.ParameterName = "Traverse Lead Speed";
            TraverseLeadSpeed.IsSerialCommand = true;
            TraverseLeadSpeed.SerialCommand = "TraverseLeadRPM";
            TraverseLeadSpeed.Value = "0.00";
            TraverseLeadSpeed.Unit = "rpm";
            TraverseLeadSpeed.ParameterType = StaticStrings.TRAVERSE;

            //TraverseRunMode.HardwareType = "1";
            //TraverseRunMode.ParameterName = "Traverse Run Mode";
            //TraverseRunMode.IsSerialCommand = false;
            //TraverseRunMode.SerialCommand = "RunMode";
            //TraverseRunMode.ItemIndex = 0;
            //TraverseRunMode.Value = 0;
            //TraverseRunMode.ParameterType = "Traverse";



            //EnumItem home = new EnumItem() { ItemValue = "Home", ItemValueID = "0" };
            //EnumItem stop = new EnumItem() { ItemValue = "Stop", ItemValueID = "1" };
            //EnumItem manual = new EnumItem() { ItemValue = "Manual", ItemValueID = "2" };
            //EnumItem semiManual = new EnumItem() { ItemValue = "Semi Manual", ItemValueID = "3" };
            //EnumItem fullAuto = new EnumItem() { ItemValue = "Full Auto", ItemValueID = "4" };

            //TraverseRunMode.EnumList = new ObservableCollection<EnumItem>();
            //TraverseRunMode.EnumList.Add(home);
            //TraverseRunMode.EnumList.Add(stop);
            //TraverseRunMode.EnumList.Add(manual);
            //TraverseRunMode.EnumList.Add(semiManual);
            //TraverseRunMode.EnumList.Add(fullAuto);




            TraverseStartPosition.HardwareType = "3";
            TraverseStartPosition.ParameterName = "Traverse Start";
            TraverseStartPosition.IsSerialCommand = true;
            TraverseStartPosition.SerialCommand = "TraverseStartPosition";
            TraverseStartPosition.ItemIndex = 0;
            TraverseStartPosition.Value = 0;
            TraverseStartPosition.ParameterType = StaticStrings.TRAVERSE;


            EnumItem back = new EnumItem() { ItemValue = "Back", ItemValueID = "0" };
            EnumItem middle = new EnumItem() { ItemValue = "Middle", ItemValueID = "1" };
            EnumItem front = new EnumItem() { ItemValue = "Front", ItemValueID = "2" };
            
            TraverseStartPosition.EnumList = new ObservableCollection<EnumItem>();
            TraverseStartPosition.EnumList.Add(back);
            TraverseStartPosition.EnumList.Add(middle);
            TraverseStartPosition.EnumList.Add(front);

            MotherboardRestartReason.ParameterName = "Motherboard Restart Reason";
            MotherboardRestartReason.ParameterType = "Debug";
            MotherboardRestartReason.IsSerialCommand = false;
            MotherboardRestartReason.SerialCommand = "MotherboardRestartReason";

            OpenSpoolerFolder.ParameterName = "Open Spool Folder";
            OpenSpoolerFolder.ParameterType = "Production";
            OpenSpoolerFolder.IsXmLParameter = false;
            OpenSpoolerFolder.ButtonType = Infrastructure.UI.ButtonTypes.OpenFolderType;
            OpenSpoolerFolder.ButtonPathLocation = _fileService.EnvironmentDirectory;

            TraverseMotionStatus.HardwareType = "3";
            TraverseMotionStatus.ParameterName = "Traverse Motion Status";
            TraverseMotionStatus.IsSerialCommand = true;
            TraverseMotionStatus.SerialCommand = "TraverseMotionStatus";
            TraverseMotionStatus.Value = "";
            TraverseMotionStatus.Unit = "";
            TraverseMotionStatus.ParameterType = StaticStrings.TRAVERSE;


            //settings.Add(Description);
            settings.Add(FilamentDiameter);
            settings.Add(UpperLimit);
            settings.Add(LowerLimit);
            settings.Add(SpoolNumber);
            settings.Add(TraverseHomeOffset);
            settings.Add(TraverseSpoolWidth);
            settings.Add(TraverseLeadWidth);
            //settings.Add(SpoolerRpm);
            settings.Add(TraverseSpeed);
            settings.Add(TraverseLeadSpeed);
            //settings.Add(TraverseRunMode);
            //settings.Add(SpecificGravity);
            settings.Add(SpoolWeightLimit);
            settings.Add(TraverseStartPosition);
            //settings.Add(MotherboardRestartReason);
            settings.Add(OpenSpoolerFolder);
            settings.Add(TraverseMotionStatus);


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
