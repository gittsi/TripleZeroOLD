using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Core.Net.Enums
{
    public class Enum
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
