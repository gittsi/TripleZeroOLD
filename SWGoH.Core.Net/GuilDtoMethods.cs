using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace SWGoH
{
    public partial class GuildDto
    {
        private System.Net.WebClient web = null;

        public GuildDto()
        {
        }
        public GuildDto(string name)
        {
            Name = name;
        }

        public string GetGuildURLFromName(string name)
        {
            GuildConfigDto guild = GuildConfigDto.GetGuildFromName(name);
            if (guild == null) return "";
            string URL = "https://swgoh.gg/g" + guild.SWGoHUrl;
            return URL;
        }
        public string GetGuildURLFromAlias(string Alias)
        {
            GuildConfigDto guild = GuildConfigDto.GetGuildFromAllias(Alias);
            if (guild == null) return "";
            string URL = "https://swgoh.gg/g" + guild.SWGoHUrl;
            return URL;
        }
        public static string GetGuildNameFromAlias(string Alias)
        {
            GuildConfigDto guild = GuildConfigDto.GetGuildFromAllias(Alias);
            if (guild == null) return Alias;
            return guild.Name;
        }
        public void ParseSwGoh()
        {
            SWGoH.Log.ConsoleMessage("Reading info for guild : " + this.Name);
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

        public void Export(ExportMethodEnum ExportMethod, bool FullUpdateClass)
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
                    SWGoH.Log.ConsoleMessage("Exporting To Database guild : " + this.Name);
                    LastClassUpdated = DateTime.UtcNow;

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;

                    if (!FullUpdateClass)
                    {
                        foreach (PlayerDto item in Players)
                        {
                            item.Characters = null;
                        }
                    }
                    string json = JsonConvert.SerializeObject(this, settings);

                    if (!FullUpdateClass)
                    {   
                        client.BaseAddress = new Uri(SWGoH.MongoDBRepo.BuildApiUrl("Guild", "", "", "", ""));
                        HttpResponseMessage response = client.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            SWGoH.Log.ConsoleMessage("Exported To Database guild : " + this.Name);
                        }
                        else
                        {
                            SWGoH.Log.ConsoleMessage("Error Exporting to Database guild : " + this.Name);
                        }
                    }
                    else
                    {
                        // NOT IMPLMEMENTED
                    }

                    
                }
            }
        }

        private void FillGuildData(string html)
        {
            int Position = -1;
            bool ret1 = false;
            int valueint = 0;

            string reststrTosearchStart = "data-datetime=\"";
            int restindexStart = html.IndexOf(reststrTosearchStart);
            string reststrTosearchEnd = ".";
            int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
            if (restindexStart != -1 && restindexEnd != -1)
            {
                int start = restindexStart + reststrTosearchStart.Length;
                int length = restindexEnd - start;
                string value = html.Substring(start, length);
                Position = restindexEnd;

                value = value.Replace('T', ',');
                value = value.Replace('Z', ' ');
                value = value.Trim();
                LastSwGohUpdated = DateTime.ParseExact(value, "yyyy-MM-dd,HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
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

                        value = value.Replace("%20", " ");

                        PlayerNames.Add(value);
                    }
                    else exit = true;
                }
            }
        }
        public void CheckForNewPlayers()
        {
            //Add new players
            for (int i = 0; i < PlayerNames.Count; i++)
            {
                SWGoH.PlayerDto player = new PlayerDto(PlayerNames[i]);
                if (!QueueMethods.FindPlayer(PlayerNames[i]))
                {
                    SWGoH.Log.ConsoleMessage((i+1).ToString() +   ") NOT Found Player in Queue : " + PlayerNames[i]);
                    SWGoH.QueueMethods.AddPlayer(PlayerNames[i], Enums.QueueEnum.Command.UpdatePlayer, PriorityEnum.DailyUpdate, Enums.QueueEnum.QueueType.Player, DateTime.UtcNow);
                }
                else
                {
                    SWGoH.Log.ConsoleMessage((i + 1).ToString() + ") Found Player in Queue : " + PlayerNames[i] );
                }
            }
        }
        public void UpdateOnlyGuildWithNoChars(ExportMethodEnum ExportMethod)
        {
            int count = 0;
            if (CheckLastUpdateWithCurrent(ExportMethodEnum.Database))
            {
                for (int i = 0; i < PlayerNames.Count; i++)
                {
                    count++;
                    SWGoH.PlayerDto player = new PlayerDto(PlayerNames[i]);
                    int ret = player.ParseSwGoh(ExportMethod, false , false);
                    if (ret == 1)
                    {
                        if (Players == null) Players = new List<PlayerDto>();
                        player.LastClassUpdated = null;
                        Players.Add(player);
                        SWGoH.Log.ConsoleMessage("Added Player : " + player.PlayerName + " aka " + player.PlayerNameInGame);
                    }
                    else if (ret == 0)
                    {
                        Thread.Sleep(Settings.appSettings.DelayErrorPerPlayerAtGuildSearch);
                        i--;
                    }
                    else
                    {
                        if (Players == null) Players = new List<PlayerDto>();
                        Players.Add(player);
                    }
                }
                Export(ExportMethod, false);
            }
        }
        public void UpdateAllPlayers(ExportMethodEnum ExportMethod, bool AddCharacters)
        {
            int count = 0;
            for (int i = 0; i < PlayerNames.Count; i++)
            {
                count++;
                SWGoH.PlayerDto player = new PlayerDto(PlayerNames[i]);
                int ret = player.ParseSwGoh(ExportMethod, AddCharacters, false);
                if (ret == 1)
                {
                    player.LastClassUpdated = DateTime.UtcNow;
                    player.Export(ExportMethod);
                    if (Players == null) Players = new List<PlayerDto>();
                    Players.Add(player);
                    Thread.Sleep(Settings.appSettings.DelayPerPlayerAtGuildSearch);
                }
                else if (ret == 0)
                {
                    Thread.Sleep(Settings.appSettings.DelayErrorPerPlayerAtGuildSearch);
                    i--;
                }
                else
                {
                    if (Players == null) Players = new List<PlayerDto>();
                    Players.Add(player);
                }
            }
            Export(ExportMethod, false);
        }

        private bool CheckLastUpdateWithCurrent(ExportMethodEnum ExportMethod)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = SWGoH.MongoDBRepo.BuildApiUrl("Guild", "&q={\"Name\":\"" + Name + "\"}", "&s={\"LastSwGohUpdated\":-1}", "&l=1", "&f={\"LastSwGohUpdated\": 1}");
                string response = client.GetStringAsync(url).Result;
                if (response != "" && response != "[  ]")
                {
                    List<GuildDto> result = JsonConvert.DeserializeObject<List<GuildDto>>(response);
                    if (result.Count == 1)
                    {
                        GuildDto Found = result[0];
                        if (LastSwGohUpdated.CompareTo(Found.LastSwGohUpdated) == 0)
                        {
                            SWGoH.Log.ConsoleMessage("No need to update!!!!");
                            return false;
                        }
                        return true;
                    }
                    else return true;
                }
            }
            return true;
        }
    }
}
