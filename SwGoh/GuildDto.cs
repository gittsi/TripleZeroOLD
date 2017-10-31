using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwGoh
{
    public class GuildDto
    {
        public GuildDto(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public List<string> Players { get; set; }
        private System.Net.WebClient web = null;

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

        private void FillGuildData(string html)
        {
            string strtosearch = "data-sort-value=\"";
            int index = html.IndexOf(strtosearch);
            int Position = index + strtosearch.Length;
            if (index != -1)
            {
                Players = new List<string>();

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
                        value = rest.Substring(start, length);
                        restposition = restindexEnd;
                        Players.Add(value);
                    }
                    else exit = true;
                }
            }
        }

        public void UpdateAllPlayers()
        {
            int count = 0;
            foreach (string item in Players)
            {
                count++;
                Console.WriteLine(count.ToString () + ") Reading Player : " + item);
                SwGoh.PlayerDto player = new PlayerDto(item);
                bool ret = player.ParseSwGoh();
                if (ret) player.Export();
                Thread.Sleep(5000);
                
            }
        }
    }
}
