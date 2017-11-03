using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SwGoh
{
    public class GuildsConfig
    {
        public List<GuildConfig> Guilds { get; set; }
    }

    public class GuildConfig
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHId { get; set; }

        public static GuildConfig GetGuildFromAllias(string allias)
        {
            GuildConfig ret = new GuildConfig();
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.mlab.com/api/1/databases/triplezero/collections/Config.Guild/?apiKey=JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O&q=%7B%22Aliases%22:%22" + allias + "%22%7D";
                string response = client.GetStringAsync(url).Result;
                if (response != "")
                {
                    List<GuildConfig> result = JsonConvert.DeserializeObject< List<GuildConfig>>(response);
                    if (result.Count == 1) return result[0];
                    else return null;
                }
                return null;
            }
        }
        public static GuildConfig GetGuildFromName(string name)
        {
            GuildConfig ret = new GuildConfig();
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.mlab.com/api/1/databases/triplezero/collections/Config.Guild/?apiKey=JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O&q=%7B%22Name%22:%22" + name + "%22%7D";
                string response = client.GetStringAsync(url).Result;
                if (response != "")
                {
                    List<GuildConfig> result = JsonConvert.DeserializeObject<List<GuildConfig>>(response);
                    if (result.Count == 1) return result[0];
                    else return null;
                }
                return null;
            }
        }
    }

}