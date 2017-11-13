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
            None = -1,
            Flat = 0,
            Percentage = 1
        }

        public enum ModStatType
        {
            None = 0,
            Speed = 1,
            CriticalChance = 2,
            CriticalDamage = 3,
            Potency = 4,
            Tenacity = 5,
            Accuracy = 6,
            CriticalAvoidance = 7,
            Offense = 8,
            Defense = 9,
            Health = 10,
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
            None = 0,
            Health = 1,
            Defense = 2,
            CriticalDamage = 3,
            CriticalChance = 4,
            Tenacity = 5,
            Offense = 6,
            Potency = 7,
            Speed = 8
        }
    }
}
