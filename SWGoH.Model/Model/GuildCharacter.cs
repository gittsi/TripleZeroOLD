using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class GuildCharacter
    {
        public string CharacterName { get; set; }
        public List<GuildPlayerCharacter> Players { get; set; }
    }
}
