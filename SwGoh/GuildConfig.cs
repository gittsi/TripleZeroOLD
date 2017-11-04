using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwGoh
{
    public class GuildConfig
    {
        [JsonProperty(PropertyName = "_id")]
        [JsonIgnore]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public int SWGoHId { get; set; }
    }
}
