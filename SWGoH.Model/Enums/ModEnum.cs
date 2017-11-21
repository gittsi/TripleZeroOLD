using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SWGoH.Model.Enums
{
    public enum ModValueType
    {
        [Description("None")]
        None = -1,
        [Description("Flat")]
        Flat = 0,
        [Description("Percentage")]
        Percentage = 1
    }
    public enum ModStatType
    {
        [Description("None")]
        None = 0,
        [Description("Speed")]
        Speed = 1,
        [Description("CriticalChance")]
        CriticalChance = 2,
        [Description("CriticalDamage")]
        CriticalDamage = 3,
        [Description("Potency")]
        Potency = 4,
        [Description("Tenacity")]
        Tenacity = 5,
        [Description("Accuracy")]
        Accuracy = 6,
        [Description("CriticalAvoidance")]
        CriticalAvoidance = 7,
        [Description("Offense")]
        Offense = 8,
        [Description("Defense")]
        Defense = 9,
        [Description("Health")]
        Health = 10,
        [Description("Protection")]
        Protection = 11
    }
    public enum ModSlot
    {
        [Description("None")]
        None = 0,
        [Description("Rectangle")]
        Transmitter = 1,
        [Description("Arrow")]
        Receiver = 2,
        [Description("Diamond")]
        Processor = 3,
        [Description("Triangle")]
        HoloArray = 4,
        [Description("Circle")]
        DataBus = 5,
        [Description("Cross")]
        Multiplexer = 6

    }
    public enum ModSet
    {
        [Description("None")]
        None = 0,
        [Description("Health")]
        Health = 1,
        [Description("Defense")]
        Defense = 2,
        [Description("CriticalDDamage")]
        CriticalDamage = 3,
        [Description("CriticalChance")]
        CriticalChance = 4,
        [Description("Tenacity")]
        Tenacity = 5,
        [Description("Offense")]
        Offense = 6,
        [Description("Potency")]
        Potency = 7,
        [Description("Speed")]
        Speed = 8
    }
}
