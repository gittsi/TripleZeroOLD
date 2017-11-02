using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public GuildDto(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public List<string> PlayerNames { get; set; }

        public List<PlayerDto> Players { get; set; }

        public string GetGuildURLFromAlias(string Alias)
        {
            SWGohGuildSettings guild = GuildSettings.Get(Alias);
            string URL = "https://swgoh.gg/g" + guild.SWGoHId;
            return URL;
        }
        public void ParseSwGoh()
        {
            web = new System.Net.WebClient();
            Uri uri = new Uri(GetGuildURLFromAlias(this.Name));
            string html = "";
            try
            {
                html = web.DownloadString(uri);
            }
            catch { return; }
            FillGuildData(html);
        }

        public void Export()
        {
            try
            {
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

        private void FillGuildData(string html)
        {
            string strtosearch = "data-sort-value=\"";
            int index = html.IndexOf(strtosearch);
            int Position = index + strtosearch.Length;
            if (index != -1)
            {
                PlayerNames = new List<string>();

                string value;
                int restposition = 0;
                string rest = html.Substring(Position);

                bool exit = false;
                while (!exit)
                {
                    string reststrTosearchStart = "href=\"/u/";
                    int restindexStart = rest.IndexOf(reststrTosearchStart, restposition);
                    string reststrTosearchEnd = "/\">";
                    int restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = WebUtility.HtmlDecode(rest.Substring(start, length));
                        restposition = restindexEnd;
                        PlayerNames.Add(value);
                    }
                    else exit = true;
                }
            }
        }

        public void UpdateAllPlayers()
        {
            int count = 0;
            for(int i=0;i<PlayerNames.Count;i++)
            {
                count++;
                SwGoh.PlayerDto player = new PlayerDto(PlayerNames[i]);
                bool ret = player.ParseSwGoh();
                if (ret)
                {
                    player.Export();
                    if (Players == null) Players = new List<PlayerDto>();
                    Players.Add(player);
                }
                else
                {
                    Thread.Sleep(mDelayError);
                    i--;
                }
                Thread.Sleep(mDelayPlayer);
            }
            Export();
        }
    }
}
