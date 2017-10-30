using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TripleZero.Configuration
{
    public class GuildSettings
    {      

        public SWGohGuildSettings Get(string alias)
        {            

            using (StreamReader r = new StreamReader("Configuration/guilds.json"))
            {
                string json = r.ReadToEnd();

                try
                {
                    GuildSettingsModel results = JsonConvert.DeserializeObject<GuildSettingsModel>(json);
                    //var matched = results.Guilds.Where(p => p.Aliases.Contains(alias)).FirstOrDefault();

                    var matched = results.Guilds.Where(p => p.Aliases.IndexOf(alias) !=1).FirstOrDefault();

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
