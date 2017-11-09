using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwGoh
{
    public class CharactersConfig
    {
        public List<CharacterConfig> Characters { get; set; }

        public static void ExportCharacterFilesToDB()
        {
            using (HttpClient client = new HttpClient())
            {
                var queryData = string.Concat("q={}");
                string apikey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";

                string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/?{0}&apiKey={1}", queryData, apikey);
                string response = client.GetStringAsync(url).Result;
                if (response != "" && response != "[  ]")
                {
                    List<CharactersConfig> result = JsonConvert.DeserializeObject<List<CharactersConfig>>(response);
                    if (result.Count == 1)
                    {
                        CharactersConfig config = result[0];
                        foreach (CharacterConfig item in config.Characters)
                        {
                            string json = JsonConvert.SerializeObject(item, Converter.Settings);
                            
                            string uri1 = "https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character?apiKey=JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";
                            HttpResponseMessage response1 = client.PostAsync(uri1, new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                            if (response1.IsSuccessStatusCode)
                            {
                                SWGoH.Core.Net.Log.ConsoleMessage("Added : " + item.Name);
                            }
                            else
                            {
                                SWGoH.Core.Net.Log.ConsoleMessage("Error : " + item.Name);
                            }
                        }
                    }
                }
            }
        }
    }

    public class CharacterConfig
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHUrl { get; set; }
    }

    
}
