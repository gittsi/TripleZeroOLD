using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class GuildCharacter : Cache
    {
        public string CharacterName { get; set; }
        public List<GuildPlayerCharacter> Players { get; set; }
        public override bool LoadedFromCache { get => base.LoadedFromCache; set => base.LoadedFromCache = value; }
    }
}
