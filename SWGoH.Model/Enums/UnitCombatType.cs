using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SWGoH.Model.Enums
{
    public enum UnitCombatType
    {
        [Description("none")]
        None = 0,
        [Description("character")]
        Character = 1,
        [Description("ship")]
        Ship = 2
    }   
}
