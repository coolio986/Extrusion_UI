﻿using Digital_Indicator.Infrastructure.UI.ControlBase;
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
        public ObservableCollection<ViewModelBase> Settings;

        private DataInputViewModel Description;
        private DataInputViewModel FilamentDiameter;
        private DataInputViewModel UpperLimit;
        private DataInputViewModel LowerLimit;
        private DataInputViewModel SpoolNumber;
        private DataInputViewModel BatchNumber;
        private DataInputViewModel SpoolerRpm;
        private DataInputViewModel TraverseInnerOffset;
        private DataInputViewModel TraverseSpoolWdith;
        private EnumItemsViewModel TraverseRunMode;
        

        public SettingItems()
        {
            Settings = new ObservableCollection<ViewModelBase>();


            Description = new DataInputViewModel();
            FilamentDiameter = new DataInputViewModel();
            UpperLimit = new DataInputViewModel();
            LowerLimit = new DataInputViewModel();
            SpoolNumber = new DataInputViewModel();
            BatchNumber = new DataInputViewModel();
            SpoolerRpm = new DataInputViewModel();
            TraverseInnerOffset = new DataInputViewModel();
            TraverseSpoolWdith = new DataInputViewModel();
            TraverseRunMode = new EnumItemsViewModel();

            Description.ParameterName = "Description";
            FilamentDiameter.ParameterName = "Filament Diameter";
            UpperLimit.ParameterName = "Upper Limit";
            LowerLimit.ParameterName = "Lower Limit";
            SpoolNumber.ParameterName = "Spool Number";
            BatchNumber.ParameterName = "Batch Number";


            SpoolerRpm.HardwareType = "1";
            SpoolerRpm.ParameterName = "Spooler RPM Setpoint";
            SpoolerRpm.SerialCommand = "velocity";
            SpoolerRpm.Value = "0";

            TraverseInnerOffset.HardwareType = "3";
            TraverseInnerOffset.ParameterName = "Traverse Inner Offset";
            TraverseInnerOffset.SerialCommand = "InnerOffset";
            TraverseInnerOffset.Value = "100";

            TraverseSpoolWdith.HardwareType = "3";
            TraverseSpoolWdith.ParameterName = "Spool Width";
            TraverseSpoolWdith.SerialCommand = "SpoolWidth";
            TraverseSpoolWdith.Value = "60000";

            TraverseRunMode.HardwareType = "3";
            TraverseRunMode.ParameterName = "Traverse Run Mode";
            TraverseRunMode.SerialCommand = "RunMode";

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



            Settings.Add(Description);
            Settings.Add(FilamentDiameter);
            Settings.Add(UpperLimit);
            Settings.Add(LowerLimit);
            Settings.Add(SpoolNumber);
            Settings.Add(BatchNumber);
            Settings.Add(SpoolerRpm);
            Settings.Add(TraverseInnerOffset);
            Settings.Add(TraverseSpoolWdith);
            Settings.Add(TraverseRunMode);



        }
    }
}
