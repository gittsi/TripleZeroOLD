using SWGoH.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class GuildPlayerCharacter
    {
        public string PlayerName { get; set; }
        public int Rarity { get; set; }
        public int Power { get; set; }
        public UnitCombatType CombatType { get; set; }
        public int Level { get; set; }
    }
}
