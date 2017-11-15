using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Guild
    {
        public string Name { get; set; }
        public DateTime SWGoHUpdateDate { get; set; }
        public DateTime EntryUpdateDate { get; set; }
        public int GalacticPower { get; set; }
        public int GalacticPowerAverage { get; set; }        
        public List<Player> Players { get; set; }
    }
}
