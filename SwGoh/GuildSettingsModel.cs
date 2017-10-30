using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SwGoh
{
    public class GuildSettingsModel
    {
        public List<SWGohGuildSettings> Guilds { get; set; }        
    }

    public class SWGohGuildSettings
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHId { get; set; }
    }

    public class GuildSettings
    {

        public static SWGohGuildSettings Get(string alias)
        {

            using (StreamReader r = new StreamReader("Settings/guilds.json"))
            {
                string json = r.ReadToEnd();

                try
                {
                    GuildSettingsModel results = JsonConvert.DeserializeObject<GuildSettingsModel>(json);
                    //var matched = results.Guilds.Where(p => p.Aliases.Contains(alias)).FirstOrDefault();

                    var matched = results.Guilds.Where(p => p.Aliases.IndexOf(alias) != 1).FirstOrDefault();

                    //var matched2 = results.Guilds.Select(p=>p.Aliases).Where(t => t.IndexOf(alias) != -1);

                    return matched;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
