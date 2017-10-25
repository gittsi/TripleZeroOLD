using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Model;

namespace TripleZero.Configuration
{
    public class GuildSettingsModel
    {
        public List<SWGohGuildSettings> GuildsSettings { get; set; }        
    }

    public class SWGohGuildSettings
    {
        public string Name { get; set; }
        public List<string> Keys { get; set; }
        public string SWGoHId { get; set; }
    }


}
