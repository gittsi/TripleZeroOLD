using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Dto
{
    internal class GuildPlayerCharacterDto
    {
        public string Name { get; set; }
        public int Rarity { get; set; }
        public int Power { get; set; }
        public int Combat_Type { get; set; }
        public int Level { get; set; }
    }
}
