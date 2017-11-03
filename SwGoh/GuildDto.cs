using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwGoh
{
    public class GuildDto
    {
        private System.Net.WebClient web = null;
        private int mDelayPlayer = 10000;
        private int mDelayError = 600000;
        public GuildDto()
        {
        }
        public GuildDto(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public DateTime LastClassUpdated { get; set; }
        public int GP { get; set; }
        public int GPaverage { get; set; }
        public List<string> PlayerNames { get; set; }

        public List<PlayerDto> Players { get; set; }
        public string GetGuildURLFromName(string name)
        {
            GuildConfig guild = GuildConfig.GetGuildFromName(name);
            if (guild == null) return "";
            string URL = "https://swgoh.gg/g/" + guild.SWGoHId + "/" + guild.Name + "/";
            return URL;
        }
        public string GetGuildURLFromAlias(string Alias)
        {
            GuildConfig guild = GuildConfig.GetGuildFromAllias(Alias);
            if (guild == null) return "";
            string URL = "https://swgoh.gg/g/" + guild.SWGoHId + "/" + guild.Name + "/";
            return URL;
        }
        public string GetGuildNameFromAlias(string Alias)
        {
            GuildConfig guild = GuildConfig.GetGuildFromAllias(Alias);
            if (guild == null) return "";
            return guild.Name;
        }
        public void ParseSwGoh()
        {
            web = new System.Net.WebClient();
            string htm = GetGuildURLFromName(this.Name);
            if (htm == "") return;
            Uri uri = new Uri(htm);
            string html = "";
            try
            {
                html = web.DownloadString(uri);
            }
            catch { return; }
            FillGuildData(html);
        }

        public void Export(ExportMethodEnum ExportMethod , bool CharactersAdded)
        {
            if (ExportMethod == ExportMethodEnum.File)
            {
                try
                {
                    LastClassUpdated = DateTime.UtcNow;

                    string directory = AppDomain.CurrentDomain.BaseDirectory + "PlayerJsons";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;

                    string fname = directory + "\\" + Name + @".json";
                    using (StreamWriter sw = new StreamWriter(fname))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, this);
                    }
                }
                catch
                {
                    //Error Occured , Contact Developer
                }
            }
            else if (ExportMethod == ExportMethodEnum.Database)
            {
                using (HttpClient client = new HttpClient())
                {
                    LastClassUpdated = DateTime.UtcNow;

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                   
                    string json = JsonConvert.SerializeObject(this, settings);

                    if (!CharactersAdded) client.BaseAddress = new Uri("https://api.mlab.com/api/1/databases/triplezero/collections/Guild?apiKey=JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O");
                    else client.BaseAddress = new Uri("https://api.mlab.com/api/1/databases/triplezero/collections/GuildWithCharacters?apiKey=JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O");

                    HttpResponseMessage response = client.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        ConsoleMessage("Exported To Database guild : " + this.Name);
                    }
                    else
                    {
                        ConsoleMessage("Error Exporting to Database guild : " + this.Name);
                    }
                }
            }
           

            
        }

        private void FillGuildData(string html)
        {
            int Position = -1;
            bool ret1 = false;
            int valueint = 0;

            string reststrTosearchStart = "stat-item-value\">";
            int restindexStart = html.IndexOf(reststrTosearchStart);
            string reststrTosearchEnd = "</div>";
            int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
            if (restindexStart != -1 && restindexEnd != -1)
            {
                int start = restindexStart + reststrTosearchStart.Length;
                int length = restindexEnd - start;
                string value = html.Substring(start, length);
                Position = restindexEnd;

                value = value.Replace(",", "");

                ret1 = int.TryParse(value, out valueint);
                if (ret1) GP = valueint;
            }

            reststrTosearchStart = "stat-item-value\">";
            restindexStart = html.IndexOf(reststrTosearchStart, Position);
            reststrTosearchEnd = "</div>";
            restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
            if (restindexStart != -1 && restindexEnd != -1)
            {
                int start = restindexStart + reststrTosearchStart.Length;
                int length = restindexEnd - start;
                string value = html.Substring(start, length);
                Position = restindexEnd;

                value = value.Replace(",", "");

                ret1 = int.TryParse(value, out valueint);
                if (ret1) GPaverage = valueint;
            }


            string strtosearch = "data-sort-value=\"";
            int index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                PlayerNames = new List<string>();

                    string value;
                int restposition = 0;
                string rest = html.Substring(Position);

                bool exit = false;
                while (!exit)
                {
                    reststrTosearchStart = "href=\"/u/";
                    restindexStart = rest.IndexOf(reststrTosearchStart, restposition);
                    reststrTosearchEnd = "/\">";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = WebUtility.HtmlDecode(rest.Substring(start, length));
                        restposition = restindexEnd;
                        PlayerNames.Add(WebUtility.HtmlDecode(value));
                    }
                    else exit = true;
                }
            }
        }
        public void UpdateOnlyGuildWithNoChars(ExportMethodEnum ExportMethod)
        {
            int count = 0;
            for (int i = 0; i < PlayerNames.Count; i++)
            {
                count++;
                SwGoh.PlayerDto player = new PlayerDto(PlayerNames[i]);
                int ret = player.ParseSwGoh(ExportMethod, false);
                if (ret == 1)
                {
                    if (Players == null) Players = new List<PlayerDto>();
                    player.LastClassUpdated = null;
                    Players.Add(player);
                }
                else if (ret == 0)
                {
                    Thread.Sleep(mDelayError);
                    i--;
                }
                else
                {
                    if (Players == null) Players = new List<PlayerDto>();
                    Players.Add(player);
                }
            }
            this.PlayerNames = null;
            Export(ExportMethod, false);
        }
        public void UpdateAllPlayers(ExportMethodEnum ExportMethod, bool AddCharacters)
        {
            int count = 0;
            for(int i=0;i<PlayerNames.Count;i++)
            {
                count++;
                SwGoh.PlayerDto player = new PlayerDto(PlayerNames[i]);
                int ret = player.ParseSwGoh(ExportMethod, AddCharacters);
                if (ret == 1)
                {
                    player.Export(ExportMethod);
                    if (Players == null) Players = new List<PlayerDto>();
                    Players.Add(player);
                    Thread.Sleep(mDelayPlayer);
                }
                else if (ret == 0)
                {
                    Thread.Sleep(mDelayError);
                    i--;
                }
                else
                {
                    if (Players == null) Players = new List<PlayerDto>();
                    Players.Add(player);
                }
            }
            Export(ExportMethod,true);
        }

        private void ConsoleMessage(string message)
        {
            Console.WriteLine(message + "  Time:" + DateTime.Now.TimeOfDay.ToString("h':'m':'s''"));
        }
    }
}
