using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Model
{
    public class GuildCharacter
    {
        public Character Character { get; set; }        
        
        public List<PlayerCharacter> Players { get; set; }
    }
}
