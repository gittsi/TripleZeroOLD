using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class GuildConfig : Cache
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public int SWGoHId { get; set; }
        public string SWGoHUrl { get; set; }
        public override bool LoadedFromCache { get => base.LoadedFromCache; set => base.LoadedFromCache = value; }
    }
}