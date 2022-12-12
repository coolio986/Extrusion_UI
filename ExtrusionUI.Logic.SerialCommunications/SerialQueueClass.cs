using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtrusionUI.Logic.SerialCommunications
{
    public class SerialQueueClass
    {
        public readonly static List<SerialQueueClass> SerialQueues = new List<SerialQueueClass>();
        private SerialPort SerialPort { get; set; }
        private ConcurrentQueue<SerialCommand> SerialQueue { get; set; }
        private int CCqueueCount;
        ReaderWriterLockSlim Lock_ { get; set; }
        private int DeviceId { get; set; } = 1;

        public static void AddSerialPortToQueue(SerialPort serialPort, int deviceId, ConcurrentQueue<SerialCommand> serialQueue, int CCqueueCount, ReaderWriterLockSlim lock_)
        {
            int queueCount = 0;
            SerialQueues.Add(new SerialQueueClass()
            {
                SerialPort = serialPort,
                SerialQueue = serialQueue,
                CCqueueCount = queueCount,
                Lock_ = lock_,
                DeviceId = deviceId,
            });
            CCqueueCount = queueCount;
        }

        public void DecrementQueueLock()
        {
            Interlocked.Decrement(ref this.CCqueueCount);
        }

        public void IncrementQueueLock()
        {
            Interlocked.Increment(ref this.CCqueueCount);
        }

        public static void Enqueue(SerialCommand serialCommand)
        {
            SerialQueueClass serialQueueClass = GetMyQueueById(serialCommand);
            if (serialQueueClass != null)
            {

                serialQueueClass.SerialQueue.Enqueue(serialCommand);
                serialQueueClass.IncrementQueueLock();
            }
        }

        public static void DecrementQueueLock(SerialCommand serialCommand)
        {
            SerialQueueClass serialQueueClass = GetMyQueueById(serialCommand);

            serialQueueClass.DecrementQueueLock();
        }

        private static SerialQueueClass GetMyQueueById(SerialCommand serialCommand)
        {
            var queue = SerialQueues.FirstOrDefault(x => x.DeviceId.ToString() == serialCommand.DeviceID);
            if(queue != null)
            {
                return queue;
            }
            return null;
        }
    }
}
