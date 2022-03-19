using Digital_Indicator.Infrastructure.UI.ControlBase;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Infrastructure.UI.Controls
{
    public class ButtonPressViewModel : ViewModelBase
    {
        public DelegateCommand<ButtonPressViewModel> ButtonCommand { get; set; }
        public ButtonTypes ButtonType { get; set; }
        public String ButtonPathLocation { get; set; }
    }
}
