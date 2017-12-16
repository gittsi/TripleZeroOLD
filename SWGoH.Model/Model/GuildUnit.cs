using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class GuildUnit : Cache
    {
        public string Name { get; set; }
        public List<GuildPlayerUnit> Players { get; set; }
        public override bool LoadedFromCache { get => base.LoadedFromCache; set => base.LoadedFromCache = value; }
    }
}
