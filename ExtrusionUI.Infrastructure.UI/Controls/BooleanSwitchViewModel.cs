﻿using ExtrusionUI.Toolkits;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Infrastructure.UI.Controls
{
    public class BooleanSwitchViewModel : ViewModelBase
    {
        public bool GetValue()
        {
            return (bool)Convert.ChangeType(base.Value, typeof(bool));
        }

    }
}
