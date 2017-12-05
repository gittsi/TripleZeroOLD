using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SWGoH
{
    public class GuildsConfigDto
    {
        public List<GuildConfigDto> Guilds { get; set; }
    }

    public class GuildConfigDto
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public int SWGoHId { get; set; }
        public string SWGoHUrl { get; set; }

        public static GuildConfigDto GetGuildFromAllias(string allias)
        {
            GuildConfigDto ret = new GuildConfigDto();
            using (HttpClient client = new HttpClient())
            {
                string url = SWGoH.MongoDBRepo.BuildApiUrl("Config.Guild", "&q=%7B%22Aliases%22:%22" + allias + "%22%7D", "", "", "");
                string response = client.GetStringAsync(url).Result;
                if (response != "" && response != "[  ]")
                {
                    List<GuildConfigDto> result = JsonConvert.DeserializeObject< List<GuildConfigDto>>(response);
                    if (result.Count == 1) return result[0];
                    else return null;
                }
                return null;
            }
        }
        public static GuildConfigDto GetGuildFromName(string name)
        {
            GuildConfigDto ret = new GuildConfigDto();
            using (HttpClient client = new HttpClient())
            {
                string url = SWGoH.MongoDBRepo.BuildApiUrl("Config.Guild", "&q=%7B%22Name%22:%22" + name + "%22%7D", "", "", "");
                string response = client.GetStringAsync(url).Result;
                if (response != "" && response != "[  ]")
                {
                    List<GuildConfigDto> result = JsonConvert.DeserializeObject<List<GuildConfigDto>>(response);
                    if (result.Count == 1) return result[0];
                    else return null;
                }
                return null;
            }
        }

        public static bool AddGuildToConfig(string pname,int SWGoHId , string SWGoHUrl)
        {
            try
            {
                if (GetGuildFromName(pname) != null) return true;
                using (HttpClient client = new HttpClient())
                {
                    JObject data = new JObject(
                            new JProperty("Name", pname),
                            new JProperty("Aliases", new List<string> { }),
                            new JProperty("SWGoHId", SWGoHId),
                            new JProperty("SWGoHUrl", SWGoHUrl));

                    var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    var requestUri = string.Format(SWGoH.MongoDBRepo.BuildApiUrl("Config.Guild", "", "", "", ""));
                    HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        SWGoH.Log.ConsoleMessage("Added Guild To Config.Guild:" + pname);
                        return true;
                    }
                    return false;
                    
                }
            }
            catch(Exception e)
            {
                SWGoH.Log.ConsoleMessage("ERROR Adding guild to Config.Guild :" + pname);
                return false;
            }
        }
    }

}