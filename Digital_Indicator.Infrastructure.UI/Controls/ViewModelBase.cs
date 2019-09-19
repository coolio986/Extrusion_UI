﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Digital_Indicator.Infrastructure.UI.Controls
{
    public class ViewModelBase : BindableBase
    {
        public string ParameterName { get; set; }
        public string ParameterID { get; set; }
        public string HardwareType { get; set; }
        public string SerialCommand { get; set; }
        public DelegateCommand <ViewModelBase>EnterCommand { get; set; }

        private object value;
        public virtual object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                RaisePropertyChanged();
            }
        }




    }
}
