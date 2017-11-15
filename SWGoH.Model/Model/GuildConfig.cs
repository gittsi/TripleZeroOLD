using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class GuildConfig
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public int SWGoHId { get; set; }
        public string SWGoHUrl { get; set; }
    }
}