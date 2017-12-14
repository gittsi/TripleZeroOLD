using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Character : Unit
    {
        public int Gear { get; set; }
        public int StatPower { get; set; }
        public List<Mod> Mods { get; set; }
    }    
}

