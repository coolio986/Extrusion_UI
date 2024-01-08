using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.ModbusTCP
{
    public interface IModbusTCPService
    {
        void Start();

        event EventHandler RunStatusChanged;
    }
}
