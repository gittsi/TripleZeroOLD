using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SwGoh
{
    public class ModEnum
    {
        public enum ModValueType
        {
            [Description("none")]
            None = -1,
            [Description("flat")]
            Flat = 0,
            [Description("percentage")]
            Percentage = 1
        }        

        public enum ModStatType
        {
            [Description("none")]
            None = 0,
            [Description("speed")]
            Speed = 1,
            [Description("criticalchance")]
            CriticalChance = 2,
            [Description("criticaldamage")]
            CriticalDamage = 3,
            [Description("potency")]
            Potency = 4,
            [Description("tenacity")]
            Tenacity = 5,
            [Description("accuracy")]
            Accuracy = 6,
            [Description("criticalavoidance")]
            CriticalAvoidance = 7,
            [Description("offense")]
            Offense = 8,
            [Description("defense")]
            Defense = 9,
            [Description("health")]
            Health = 10,
            [Description("rotection")]
            Protection = 11
        }

        public enum ModSlot
        {
            [Description("none")]
            None = 0,
            [Description("rectangle")]
            Transmitter = 1,
            [Description("arrow")]
            Receiver = 2,
            [Description("diamond")]
            Processor = 3,
            [Description("triangle")]
            HoloArray = 4,
            [Description("circle")]
            DataBus = 5,
            [Description("cross")]
            Multiplexer = 6

        }

        public enum ModSet
        {
            [Description("none")]
            None = 0,
            [Description("health")]
            Health = 1,
            [Description("defense")]
            Defense = 2,
            [Description("criticaldamage")]
            CriticalDamage = 3,
            [Description("criticalchange")]
            CriticalChance = 4,
            [Description("tenacity")]
            Tenacity = 5,
            [Description("offense")]
            Offense = 6,
            [Description("potency")]
            Potency = 7,
            [Description("speed")]
            Speed = 8
        }
    }
}
