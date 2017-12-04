using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    //public class CharactersConfig : Cache
    //{
    //    public List<CharacterConfig> Characters { get; set; }
    //    public override bool LoadedFromCache { get => base.LoadedFromCache; set => base.LoadedFromCache = value; }
    //}
    public class CharacterConfig : Cache
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHUrl { get; set; }
        public List<AbilityConfig> Abilities { get; set; }
        public override bool LoadedFromCache { get => base.LoadedFromCache; set => base.LoadedFromCache = value; }
    }
}
