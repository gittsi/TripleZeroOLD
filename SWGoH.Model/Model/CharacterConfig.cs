using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class CharacterConfig : UnitConfig
    {       
        public List<AbilityConfig> Abilities { get; set; }
    }
}
