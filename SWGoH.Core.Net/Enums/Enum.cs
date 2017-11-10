using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Enums
{
    public class QueueEnum
    {
        public enum QueueStats
        {
            PendingProcess = 0,
            Processing = 1,
            Failed = 101            
        }

        public enum QueueType
        {
            Player = 0,
            Guild = 1            
        }
    }
}
