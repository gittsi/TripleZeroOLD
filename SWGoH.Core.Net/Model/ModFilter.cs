using System;
using System.Collections.Generic;
using System.Text;
using static SwGoH.ModEnum;

namespace SwGoH.Model
{
    public class ModFilters
    {
        public List<ModFilter> Filters { get; set; }
        public int Limit { get; set; }
    }

    public class ModFilter
    {   
        public bool isPrimary { get; set; }
        public ModSet ModSet { get; set; }
        public ModSlot ModSlot { get; set; }
        public ModStatType ModStatType { get; set; }
        public ModValueType ModValueType { get; set; }        
    }
}
