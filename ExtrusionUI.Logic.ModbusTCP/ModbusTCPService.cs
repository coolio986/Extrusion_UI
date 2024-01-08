using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.ModbusTCP
{
    public class ModbusTCPService : IModbusTCPService
    {

        private int runstatus;
        public ModbusTCPService()
        {
            runstatus = -1;
        }

        public event EventHandler RunStatusChanged;

        public void Start()
        {

            Task.Factory.StartNew(() =>
            {

                using (TcpClient client = new TcpClient("192.168.3.5", 502))
                {

                    while (true)
                    {
                        var factory = new ModbusFactory();
                        IModbusMaster master = factory.CreateMaster(client);

                        byte slaveId = 1;
                        ushort startAddress = 10;
                        ushort numInputs = 1;

                        ushort isRunning = master.ReadHoldingRegisters(slaveId, startAddress, numInputs).First();
                        //Console.WriteLine(isRunning);
                        Thread.Sleep(100);
                        if(runstatus != isRunning)
                        {
                            runstatus = isRunning;
                            RunStatusChanged?.Invoke(isRunning, new EventArgs());
                        }
                    }
                }
            });
        }
    }
}
