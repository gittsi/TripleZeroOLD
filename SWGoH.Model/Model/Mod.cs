using SWGoH.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Mod
    {
        public string Name { get; set; }        
        public int Level { get; set; }        
        public int Star { get; set; }
        public ModSlot Type { get; set; }
        public string Rarity { get; set; }        
        public ModStat PrimaryStat { get; set; }        
        public List<ModStat> SecondaryStat { get; set; }
    }
    public class ModStat
    {
        public ModValueType ValueType { get; set; }
        public ModStatType StatType { get; set; }
        public double Value { get; set; }
    }
}
