﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWGoH;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using SWGoH.Enums.ModEnum;

namespace SWGoH
{
    public partial class PlayerDto
    {
        public static bool isOnExit = false;
        public void Import(ExportMethodEnum ExportMethod)
        {
            if (ExportMethod == ExportMethodEnum.File)
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory + "PlayerJsons";
                string fname = directory + "\\" + PlayerName + @".json";
                if (File.Exists(fname))
                {
                    var lines = File.ReadAllText(fname);
                    //PlayerDto ret = JsonConvert.DeserializeObject<PlayerDto>(lines, Converter.Settings);
                    JsonConvert.PopulateObject(lines, this);
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = SWGoH.MongoDBRepo.BuildApiUrl("Player", "&q={\"PlayerName\":\"" + PlayerName + "\"}", "&s={\"LastSwGohUpdated\":-1}", "&l=1", "");
                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<PlayerDto> result = JsonConvert.DeserializeObject<List<PlayerDto>>(response);
                        if (result.Count == 1)
                        {
                            JsonConvert.PopulateObject(JsonConvert.SerializeObject(result[0]), this);
                        }
                    }
                }
            }
        }
        public void DeletePlayerFromDBAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = SWGoH.MongoDBRepo.BuildApiUrl("Player", "&q={\"PlayerName\":\"" + PlayerName + "\"}", "&s={\"LastClassUpdated\":1}", "", "&f={\"PlayerName\": 1}");
                    var response = client.GetStringAsync(url).Result;

                    List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    if (document.Count == 1) return;
                    PlayerDto result1 = BsonSerializer.Deserialize<PlayerDto>(document.FirstOrDefault());

                    if (result1 != null)
                    {

                        var deleteurl = SWGoH.MongoDBRepo.BuildApiUrlFromId("Player", result1.Id.ToString());
                        WebRequest request = WebRequest.Create(deleteurl);
                        request.Method = "DELETE";

                        HttpWebResponse response1 = (HttpWebResponse)request.GetResponse();
                        if (response1.StatusCode == HttpStatusCode.OK)
                        {
                            SWGoH.Log.ConsoleMessage("Removed Previous from Players!");
                        }
                        else
                        {
                            SWGoH.Log.ConsoleMessage("Error : Could not remove previous from Pleayers!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error deleting player " + PlayerName + " : " + e.Message);
            }
        }
        public void Export(ExportMethodEnum ExportMethod)
        {
            if (ExportMethod == ExportMethodEnum.File)
            {
                try
                {
                    string directory = AppDomain.CurrentDomain.BaseDirectory + "PlayerJsons";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };

                    string fname = directory + "\\" + PlayerName + @".json";
                    using (StreamWriter sw = new StreamWriter(fname))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, this);
                    }
                    SWGoH.Log.ConsoleMessage("Created : " + PlayerName + "'s json File");
                }
                catch (Exception e)
                {
                    SWGoH.Log.ConsoleMessage("Error : " + e.Message);
                    //Error Occured , Contact Developer
                }
            }
            else if (ExportMethod == ExportMethodEnum.Database)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {

                        string json = JsonConvert.SerializeObject(this, Converter.Settings);
                        client.BaseAddress = new Uri(SWGoH.MongoDBRepo.BuildApiUrl("Player", "", "", "", ""));
                        HttpResponseMessage response = client.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            SWGoH.Log.ConsoleMessage("Added To Database : " + PlayerNameInGame);
                        }
                        else
                        {
                            SWGoH.Log.ConsoleMessage("Error Adding To Database : " + PlayerName);
                        }
                    }
                }
                catch (Exception e)
                {
                    SWGoH.Log.ConsoleMessage("Exception Adding To Database : " + PlayerName + " : " + e.Message);
                }
            }
        }
        public int ParseSwGoh(ExportMethodEnum ExportMethod, bool AddCharacters, bool checkForCharAllias)
        {
            if (PlayerName == null || PlayerName == "") return 0;

            string pname = TryGetRealURLFromAlliasPlayerName(PlayerName);

            PlayerName = pname.Replace("%20", " ");

            int retbool = -1;

            FillArenaInfo(pname);

            web = new System.Net.WebClient();
            Uri uri = new Uri("https://swgoh.gg/u/" + pname + "/collection/");

            string html = "";
            try
            {
                html = web.DownloadString(uri);
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Exception on Player : " + PlayerName + " : " + e.Message);
                web = null;
                return 0;
            }

            bool retPlayer = FillPlayerData(html, out int Position);
            if (!retPlayer)
            {
                SWGoH.Log.ConsoleMessage("Player NOT FOUND : " + this.PlayerName + " aka " + PlayerNameInGame);
                return 0;
            }
            SWGoH.Log.ConsoleMessage("Reading Player " + this.PlayerName + " aka " + PlayerNameInGame);
            if (!AddCharacters) return 1;
            bool ret = CheckLastUpdateWithCurrent(ExportMethod);
            //ret = true;
            if (ret || checkForCharAllias)
            {
                FillPlayerCharacters(html, Position, checkForCharAllias);
                FillPlayerShips(pname, checkForCharAllias);
                retbool = 1;
            }
            else
            {
                Import(ExportMethod);
                retbool = 2;
            }
            web = null;
            return retbool;
        }

        private void FillArenaInfo(string pname)
        {
            web = new System.Net.WebClient();
            Uri uri = new Uri("https://swgoh.gg/u/" + pname + "/");

            string html = "";
            try
            {
                html = web.DownloadString(uri);
            
                Arena = new ArenaDto();

                int valueint = 0;
                bool ret1 = true;
                string value = "";

                string strtosearch = "Arena</div>";
                int index = html.IndexOf(strtosearch);
                int Position = index + strtosearch.Length;
                if (index != -1)
                {
                    string reststrTosearchStart = "current-rank-value\">";
                    int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    string reststrTosearchEnd = "</div>";
                    int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = html.Substring(start, length);
                        Position = restindexEnd;

                        ret1 = int.TryParse(value, out valueint);
                        if (ret1) Arena.CurrentRank = valueint;
                    }

                    strtosearch = "Highest Rank";
                    int EndArenaTeamindex = html.IndexOf(strtosearch, Position);

                    bool exit = false;
                    while (!exit)
                    {
                        reststrTosearchStart = "alt=\"";
                        restindexStart = html.IndexOf(reststrTosearchStart, Position);
                        reststrTosearchEnd = "\"";
                        restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                        if (restindexStart != -1 && restindexEnd != -1)
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;

                            value = WebUtility.HtmlDecode(value);
                            value = FixCharacterName(value);

                            if (Arena.ArenaTeam == null) Arena.ArenaTeam = new List<string>();
                            Arena.ArenaTeam.Add(value);
                        }
                        if (restindexStart == -1 || restindexStart > EndArenaTeamindex || Arena.ArenaTeam.Count == 5) exit = true;
                    }

                    strtosearch = "Average Rank";
                    index = html.IndexOf(strtosearch);
                    Position = index;
                    if (index != -1)
                    {
                        reststrTosearchStart = "stat-item-value\">";
                        restindexStart = html.IndexOf(reststrTosearchStart, Position-100);
                        reststrTosearchEnd = "</div>";
                        restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                        if (restindexStart != -1 && restindexEnd != -1)
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;

                            double valuedouble = 0.0;
                            ret1 = double.TryParse(value, out valuedouble);
                            if (ret1) Arena.AverageRank = valuedouble;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Exception on reading Arena Info for Player : " + PlayerName + " : " + e.Message);
                web = null;
            }
        }

        private void FillPlayerShips(string pname, bool checkForCharAllias)
        {
            string html = "";
            using (WebClient web = new System.Net.WebClient())
            {
                Uri uri = new Uri("https://swgoh.gg/u/" + pname + "/ships/");
                try
                {
                    html = web.DownloadString(uri);
                }
                catch (Exception e)
                {
                    SWGoH.Log.ConsoleMessage("Exception on Player Ships : " + PlayerName + " : " + e.Message);
                }
            }

            Ships = new List<ShipDto>();

            bool exit = false;
            int count = 0;
            int previousPosition = 0;
            List<BsonDocument> Base_ID_Document = null;
            if (checkForCharAllias)
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = string.Format("https://swgoh.gg/api/ships/?format=json");
                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        Base_ID_Document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    }
                }
            }
            bool ret = true;
            int Position = 0;
            while (!exit)
            {
                if (isOnExit) return;
                previousPosition = Position;
                ShipDto newchar = GetShip(html, out Position);
                ret = FillShipData(newchar);
                if (ret && Position > 0) html = html.Substring(Position);
                if (Position < 0) exit = true;
                if (ret)
                {
                    if (newchar.Name != null)
                    {
                        count++;
                        newchar.Name = FixCharacterName(newchar.Name);
                        Ships.Add(newchar);
                        SWGoH.Log.ConsoleMessage("          " + count.ToString() + ") Added ship : " + newchar.Name);

                        if (checkForCharAllias) { AddShipToAlliasConfig(newchar, Base_ID_Document); }

                        Thread.Sleep(Settings.appSettings.DelayPerCharacter);
                    }
                }
                else
                {
                    Thread.Sleep(Settings.appSettings.DelayErrorAtCharacter);
                    Position = previousPosition;
                }
            }
        }

        private string TryGetRealURLFromAlliasPlayerName(string pname)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = SWGoH.MongoDBRepo.BuildApiUrl("Config.Players", "&q={\"Aliases\":\"" + pname + "\"}", "", "&l=1", "&f={\"PlayerName\": 1}");
                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<PlayerDto> result = JsonConvert.DeserializeObject<List<PlayerDto>>(response);
                        if (result.Count == 1)
                        {
                            PlayerDto Found = result[0];
                            return Found.PlayerName;
                        }
                        else return pname;
                    }
                }
                return pname;
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error Retrieving Real Player Name From Allias : " + this.PlayerName + "  " + e.Message);
                return pname;
            }
        }

        /// <summary>
        /// Returns true if the file should be updated , false not to update
        /// </summary>
        /// <param name="ExportMethod"></param>
        /// <returns></returns>
        private bool CheckLastUpdateWithCurrent(ExportMethodEnum ExportMethod)
        {
            //return true;
            if (ExportMethod == ExportMethodEnum.File)
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory + "PlayerJsons";
                string fname = directory + "\\" + PlayerName + @".json";

                if (File.Exists(fname))
                {
                    using (StreamReader reader = new StreamReader(fname))
                    {
                        string firstLine = reader.ReadLine() ?? "";
                        string secondLine = reader.ReadLine() ?? "";
                        string ThirdLine = reader.ReadLine() ?? "";

                        ThirdLine = ThirdLine.Trim();
                        ThirdLine = ThirdLine.Remove(0, 16);
                        ThirdLine = ThirdLine.TrimEnd(',');
                        ThirdLine = ThirdLine.TrimEnd('\"');

                        ThirdLine = ThirdLine.Replace('\"', ' ');
                        ThirdLine = ThirdLine.Replace('T', ',');
                        ThirdLine = ThirdLine.Trim();

                        DateTime filelastupdated = DateTime.ParseExact(ThirdLine, "yyyy-MM-dd,HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        if (filelastupdated.CompareTo(this.LastSwGohUpdated) == 0)
                        {
                            SWGoH.Log.ConsoleMessage("No need to update!!!!");
                            return false;
                        }
                        else return true;
                    }
                }
                else return true;
            }
            else
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = SWGoH.MongoDBRepo.BuildApiUrl("Player", "&q={\"PlayerName\":\"" + PlayerName + "\"}", "&s={\"LastSwGohUpdated\":1}", "&l=1", "&f={\"LastSwGohUpdated\": 1 , \"id\" : 1}");
                        string response = client.GetStringAsync(url).Result;
                        if (response != "" && response != "[  ]")
                        {
                            List<PlayerDto> result = BsonSerializer.Deserialize<List<PlayerDto>>(response);
                            if (result.Count == 1)
                            {
                                PlayerDto Found = result[0];
                                if (LastSwGohUpdated.CompareTo(Found.LastSwGohUpdated) == 0)
                                {
                                    SWGoH.Log.ConsoleMessage("No need to update!!!!");

                                    string date = JsonConvert.SerializeObject(DateTime.UtcNow, Converter.Settings).ToString();

                                    var httpContent = new StringContent("{\"$set\" : { \"LastClassUpdated\" :" + date + "}}", Encoding.UTF8, "application/json");
                                    var requestUri = SWGoH.MongoDBRepo.BuildApiUrlFromId("Player", Found.Id.ToString());
                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                                    }


                                    return false;
                                }
                                else
                                    return true;
                            }
                            else return true;
                        }
                    }
                    return true;
                }
                catch(Exception e)
                {
                    SWGoH.Log.ConsoleMessage("Error in CkeckLastUpdated :" + this.PlayerName + "  " + e.Message);
                    return true;
                }
            }
        }
        //public static void GetNewCharactersAndAbilities()
        //{
        //    try
        //    {
        //        string html = "";
        //        using (WebClient web = new System.Net.WebClient())
        //        {
        //            Uri uri = new Uri("https://swgoh.gg/");
        //            try
        //            {
        //                html = web.DownloadString(uri);
        //            }
        //            catch (Exception e)
        //            {
        //                SWGoH.Log.ConsoleMessage("Exception on Getting main page swgoh : " + e.Message);
        //            }
        //        }
        //        if (html != "")
        //        {
        //            List<BsonDocument> Base_ID_Document = GetBaseIDList();

        //            string strtosearch = "Star Wars Galaxy of Heroes Characters";
        //            int index = html.IndexOf(strtosearch);
        //            int Position = index + strtosearch.Length;
        //            if (index != -1)
        //            {
        //                string reststrTosearchStart = "";
        //                int restindexStart = 0;
        //                string reststrTosearchEnd = "";
        //                int restindexEnd = 0;
        //                bool exit = false;
        //                while (!exit)
        //                {
        //                    strtosearch = "alt=\\";
        //                    index = html.IndexOf(strtosearch, Position);
        //                    Position = index + strtosearch.Length;
        //                    if (index == -1) exit = true;
        //                    else
        //                    {

        //                    }

        //                    reststrTosearchStart = "alt=\\";
        //                    restindexStart = html.IndexOf(reststrTosearchStart, Position);
        //                    reststrTosearchEnd = "</a>";
        //                    restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
        //                    if (restindexStart != -1 && restindexEnd != -1)
        //                    {
        //                        int start = restindexStart + reststrTosearchStart.Length;
        //                        int length = restindexEnd - start;
        //                        string value = html.Substring(start, length);
        //                        Position = restindexEnd;


        //                    }


        //                }

        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        SWGoH.Log.ConsoleMessage("Exception on GetNewCharactersAndAbilities : " + e.Message);
        //    }
        //}
        private static List<BsonDocument> GetBaseIDList()
        {
            List<BsonDocument> Base_ID_Document = null;
            using (HttpClient client = new HttpClient())
            {
                string url = string.Format("https://swgoh.gg/api/characters/?format=json");
                string response = client.GetStringAsync(url).Result;
                if (response != "" && response != "[  ]")
                {
                    Base_ID_Document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                }
            }
            return Base_ID_Document;
        }
        /// <summary>
        /// Fills Players characters
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Position"></param>
        private void FillPlayerCharacters(string html, int Position, bool CheckForAllias)
        {
            if (Position == -1) return;

            Characters = new List<CharacterDto>();

            html = html.Substring(Position);

            bool exit = false;
            int count = 0;
            int previousPosition = 0;
            List<BsonDocument> Base_ID_Document = null;
            if (CheckForAllias)
            {
                Base_ID_Document = GetBaseIDList();
            }
            while (!exit)
            {
                if (isOnExit) return;
                previousPosition = Position;
                CharacterDto newchar = GetChar(html, out Position);
                bool ret = FillCharData(newchar);
                if (ret && Position > 0) html = html.Substring(Position);
                if (Position < 0) exit = true;
                if (ret)
                {
                    if (newchar.Name != null)
                    {
                        count++;
                        newchar.Name = FixCharacterName(newchar.Name);
                        Characters.Add(newchar);
                        SWGoH.Log.ConsoleMessage("          " + count.ToString() + ") Added character : " + newchar.Name);

                        if (CheckForAllias)
                        {
                            List<ConfigAbility> abilities = GetAbilities(newchar);
                            AddCharacterToAlliasConfig(newchar, Base_ID_Document , abilities);
                        }

                        Thread.Sleep(Settings.appSettings.DelayPerCharacter);
                    }
                }
                else
                {
                    Thread.Sleep(Settings.appSettings.DelayErrorAtCharacter);
                    Position = previousPosition;
                }
            }
        }

        private List<ConfigAbility> GetAbilities(CharacterDto newchar)
        {
            List<ConfigAbility> ret = null;
            try
            {
                string html = "";
                using (WebClient web = new System.Net.WebClient())
                {
                    string replace = "/u/" + PlayerName + "/collection/";
                    string charurl = newchar.SWGoHUrl.Replace(replace, "");
                    int index = charurl.IndexOf("/");
                    charurl = charurl.Substring(index, charurl.Length - index);
                    Uri uri = new Uri("https://swgoh.gg/characters" + charurl);
                    try
                    {
                        html = web.DownloadString(uri);
                    }
                    catch (Exception e)
                    {
                        SWGoH.Log.ConsoleMessage("Exception on getting the html : " + e.Message);
                    }
                }
                if (html != "")
                {
                    ret = new List<ConfigAbility>();

                    string strtosearch = "Abilities</h3>";
                    int index = html.IndexOf(strtosearch);
                    int Position = index + strtosearch.Length;
                    if (index != -1)
                    {
                        bool exit = false;
                        while (!exit)
                        {
                            ConfigAbility ability = new ConfigAbility();

                            string reststrTosearchStart = "alt=\"";
                            int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                            string reststrTosearchEnd = "\"";
                            int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                            if (restindexStart != -1 && restindexEnd != -1)
                            {
                                int start = restindexStart + reststrTosearchStart.Length;
                                int length = restindexEnd - start;
                                string value = html.Substring(start, length);
                                Position = restindexEnd;

                                ability.Name = value;

                                reststrTosearchStart = "\"tooltip\"";
                                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                                if (restindexStart != -1)
                                {
                                    Position = restindexStart;
                                    reststrTosearchStart = "title=\"";
                                    restindexStart = html.IndexOf(reststrTosearchStart, Position);
                                    reststrTosearchEnd = "\"";
                                    restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                                    if (restindexStart != -1 && restindexEnd != -1)
                                    {
                                        start = restindexStart + reststrTosearchStart.Length;
                                        length = restindexEnd - start;
                                        value = html.Substring(start, length);
                                        Position = restindexEnd;

                                        if (value.ToLower().Contains("zeta")) ability.AbilityType = ConfigAbilityType.Zeta;
                                        else ability.AbilityType = ConfigAbilityType.Omega;
                                    }

                                    ret.Add(ability);
                                }
                            }
                            else
                            {
                                exit = true;
                            }

                        }
                    }
                }
                return ret;
            }
            catch(Exception e)
            {
                SWGoH.Log.ConsoleMessage("Exception on GetAbilities : " + e.Message);
                return null;
            }
        }

        private string FixCharacterName(string newchar)
        {
            newchar = newchar.Replace("\"", "");
            newchar = newchar.Replace("'", "");
            newchar = newchar.Replace("Î", "");
            
            return newchar;
        }

        private void AddShipToAlliasConfig(ShipDto newchar, List<BsonDocument> base_ID_Document)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = SWGoH.MongoDBRepo.BuildApiUrl("Config.Ship", "&q={\"Name\" : \"" + newchar.Name + "\" }", "", "", "");
                string response = client.GetStringAsync(url).Result;

                string replace = "/u/" + PlayerName + "/ships/";

                string Base_ID = "";
                string lowername = newchar.Name.ToLower();
                foreach (BsonDocument item in base_ID_Document)
                {
                    string name = (string)item[0];
                    name = name.ToLower();
                    if (name.Equals(lowername))
                    {
                        Base_ID = ((string)item[1]).ToLower(); break;
                    }
                }

                if (Base_ID == "") SWGoH.Log.ConsoleMessage("Did not find BaseID for character : " + newchar.Name + "!!!!!!!");

                if (response != "" && response != "[  ]")
                {
                    SWGoH.Log.ConsoleMessage("Found Allias Char " + newchar.Name);
                    List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    CharacterConfigDto result1 = BsonSerializer.Deserialize<CharacterConfigDto>(document.FirstOrDefault());

                    if (newchar.SWGoHUrl != null)
                    {
                        if (result1.Command != "") Base_ID = result1.Command;
                        JObject data = new JObject(
                            new JProperty("Name", result1.Name),
                            new JProperty("Command", Base_ID),
                            new JProperty("SWGoHUrl", newchar.SWGoHUrl.Replace(replace, "")),
                            new JProperty("Aliases", result1.Aliases));

                        var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                        var requestUri = SWGoH.MongoDBRepo.BuildApiUrlFromId("Config.Ship", result1.Id.ToString());
                        using (HttpClient client1 = new HttpClient())
                        {
                            HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                        }
                    }
                }
                else
                {
                    if (newchar.SWGoHUrl == null) newchar.SWGoHUrl = "";
                    JObject data = new JObject(
                        new JProperty("Name", newchar.Name),
                        new JProperty("Command", Base_ID),
                        new JProperty("SWGoHUrl", newchar.SWGoHUrl.Replace(replace, "")),
                        new JProperty("Aliases", new List<string> { }));
                    string json = JsonConvert.SerializeObject(data, Converter.Settings);
                    using (HttpClient client1 = new HttpClient())
                    {
                        client1.BaseAddress = new Uri(SWGoH.MongoDBRepo.BuildApiUrl("Config.Ship", "", "", "", ""));
                        HttpResponseMessage response1 = client1.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                        SWGoH.Log.ConsoleMessageNotInFile("Added new Allias Char " + newchar.Name + "!!!!!!!");
                    }

                    //var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", apikey);
                    //using (HttpClient client1 = new HttpClient())
                    //{
                    //    HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                    //    SWGoH.Log.ConsoleMessage("Added new Allias Char" + newchar.Name + "!!!!!!!");
                    //}
                }
            }
        }
        private void AddCharacterToAlliasConfig(CharacterDto newchar, List<BsonDocument> Base_ID_Document, List<ConfigAbility> abilities)
        {
            //try
            //{
            using (HttpClient client = new HttpClient())
            {
                string url = SWGoH.MongoDBRepo.BuildApiUrl("Config.Character", "&q={\"Name\" : \"" + newchar.Name + "\" }", "", "", "");
                string response = client.GetStringAsync(url).Result;

                string replace = "/u/"+PlayerName+ "/collection/";

                string Base_ID = "";
                string lowername = newchar.Name.ToLower();
                foreach (BsonDocument item in Base_ID_Document)
                {
                    string name = (string)item[0];
                    name = name.ToLower();
                    if (name.Equals(lowername))
                    {
                        Base_ID = ((string)item[1]).ToLower (); break;
                    }
                }

                if (Base_ID == "") SWGoH.Log.ConsoleMessage("Did not find BaseID for character : " + newchar.Name + "!!!!!!!");

                if (response != "" && response != "[  ]")
                {
                    SWGoH.Log.ConsoleMessage("Found Allias Char " + newchar.Name);
                    List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    CharacterConfigDto result1 = BsonSerializer.Deserialize<CharacterConfigDto>(document.FirstOrDefault());

                    if (newchar.SWGoHUrl != null)
                    {
                        if (result1.Command != "") Base_ID = result1.Command;

                        CharacterConfigDto data = new CharacterConfigDto();
                        data.Name = result1.Name;
                        data.Command = Base_ID;
                        data.SWGoHUrl = newchar.SWGoHUrl.Replace(replace, "");
                        data.Aliases = result1.Aliases;
                        data.Abilities = abilities;

                        //JObject data = new JObject(
                        //    new JProperty("Name", result1.Name),
                        //    new JProperty("Command", Base_ID),
                        //    new JProperty("SWGoHUrl", newchar.SWGoHUrl.Replace(replace, "")),
                        //    new JProperty("Aliases", result1.Aliases),
                        //    new JProperty("Abilities", abilities));

                        string json = JsonConvert.SerializeObject(data, Converter.Settings);
                        var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                        var requestUri = SWGoH.MongoDBRepo.BuildApiUrlFromId("Config.Character", result1.Id.ToString());
                        using (HttpClient client1 = new HttpClient())
                        {
                            HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                        }
                    }
                }
                else
                {
                    if (newchar.SWGoHUrl == null) newchar.SWGoHUrl = "";

                    CharacterConfigDto data = new CharacterConfigDto();
                    data.Name = newchar.Name;
                    data.Command = Base_ID;
                    data.SWGoHUrl = newchar.SWGoHUrl.Replace(replace, "");
                    data.Aliases = new List<string> { };
                    data.Abilities = abilities;

                    //JObject data = new JObject(
                    //    new JProperty("Name", newchar.Name),
                    //    new JProperty("Command", Base_ID),
                    //    new JProperty("SWGoHUrl", newchar.SWGoHUrl.Replace(replace, "")),
                    //    new JProperty("Aliases", new List<string> { }),
                    //    new JProperty("Abilities", abilities));

                    string json = JsonConvert.SerializeObject(data, Converter.Settings);
                    using (HttpClient client1 = new HttpClient())
                    {
                        client1.BaseAddress = new Uri(SWGoH.MongoDBRepo.BuildApiUrl("Config.Character", "", "", "", ""));
                        HttpResponseMessage response1 = client1.PostAsync("", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                        SWGoH.Log.ConsoleMessageNotInFile("Added new Allias Char " + newchar.Name + "!!!!!!!");
                    }

                    //var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", apikey);
                    //using (HttpClient client1 = new HttpClient())
                    //{
                    //    HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                    //    SWGoH.Log.ConsoleMessage("Added new Allias Char" + newchar.Name + "!!!!!!!");
                    //}
                }
            }
            //}
            //catch (Exception e) { SWGoH.Log.ConsoleMessage("Added new Allias Char" + newchar.Name + ":" + e.Message); }
        }
        /// <summary>
        /// Fills player properties like LastUpdated
        /// </summary>
        /// <param name="html"></param>
        private bool FillPlayerData(string html, out int Position)
        {
            bool ret1 = false;
            int valueint = 0;

            string strtosearch = "Guild <strong";
            int index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                strtosearch = "<a href=\"";
                index = html.IndexOf(strtosearch,Position);
                Position = index + strtosearch.Length;
                if (index != -1)
                {
                    string reststrTosearchStart = "\">";
                    int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    string reststrTosearchEnd = "</a>";
                    int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        string value = html.Substring(start, length);
                        Position = restindexEnd;

                        GuildName = value;
                    }
                }
            }
            strtosearch = "Last updated:";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "data-datetime=\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "\" data";
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
                    if (value != "") LastSwGohUpdated = DateTime.ParseExact(value, "yyyy-MM-dd,HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    else return false;
                }
            }

            strtosearch = "<h5>Player Info</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "href=\"\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</a>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    string value = WebUtility.HtmlDecode(html.Substring(start, length));
                    Position = restindexEnd;

                    PlayerNameInGame = value;
                }

                strtosearch = "Galactic Power (Characters)";
                index = html.IndexOf(strtosearch, Position);
                Position = index + strtosearch.Length;
                if (index != -1)
                {
                    reststrTosearchStart = "pull-right\">";
                    restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</strong>";
                    restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        string value = html.Substring(start, length);
                        Position = restindexEnd;
                        value = value.Replace(",", "");

                        ret1 = int.TryParse(value, out valueint);
                        if (ret1) GPcharacters = valueint;
                    }
                }

                strtosearch = "Galactic Power (Ships)";
                index = html.IndexOf(strtosearch, Position);
                Position = index + strtosearch.Length;
                if (index != -1)
                {
                    reststrTosearchStart = "pull-right\">";
                    restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</strong>";
                    restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        string value = html.Substring(start, length);
                        Position = restindexEnd;
                        value = value.Replace(",", "");

                        ret1 = int.TryParse(value, out valueint);
                        if (ret1) GPships = valueint;
                    }
                }
            }
            return true;
        }

        private ShipDto GetShip(string html , out int Position)
        {
            ShipDto ret = new ShipDto();
            
            string strTosearch = "class=\"collection-ship-name-link\"";
            int index = html.IndexOf(strTosearch);
            Position = index;
            if (index != -1)
            {
                string reststrTosearchStart = "href=\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "\"";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    string link = html.Substring(start, length);
                    ret.SWGoHUrl = link;
                    Position = restindexEnd;
                }


                reststrTosearchStart = "nofollow\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</a>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    string name = html.Substring(start, length);
                    ret.Name = WebUtility.HtmlDecode(name);
                }
                Position = restindexEnd;
            }
            

            

            return ret;
        }
        private bool FillShipData(ShipDto newship)
        {
            string html = "";
            bool ret1 = false;
            int valueint = 0;
            double valuedecimal = 0;

            using (WebClient web = new System.Net.WebClient())
            {
                Uri uri = new Uri("https://swgoh.gg" + newship.SWGoHUrl);
                try
                {
                    html = web.DownloadString(uri);
                }
                catch (Exception e)
                {
                    SWGoH.Log.ConsoleMessage("Exception : " + e.Message);
                    return false;
                }
            }

            string strtosearch = "";
            int index = 0;
            int Position = 0;
            string value = "";

            #region Labels
            
            strtosearch = "char-tag char-alignment";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position + 40);
                string reststrTosearchEnd = "</a>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (newship.ShipTags == null) newship.ShipTags = new List<string>();
                    newship.ShipTags.Add(value);
                }
            }

            strtosearch = "char-tag char-role\">";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</a>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (newship.ShipTags == null) newship.ShipTags = new List<string>();
                    newship.ShipTags.Add(value);
                }
            }

            bool exit = false;
            while (!exit)
            {
                strtosearch = "char-category\"";
                index = html.IndexOf(strtosearch, Position);
                Position = index + strtosearch.Length;
                if (index != -1)
                {
                    string reststrTosearchStart = "\">";
                    int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    string reststrTosearchEnd = "</a>";
                    int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = html.Substring(start, length);
                        Position = restindexEnd;

                        if (newship.ShipTags == null) newship.ShipTags = new List<string>();
                        newship.ShipTags.Add(value);
                    }
                }
                else
                    exit = true;
            }
            #endregion


            
            #region Stars
            int star = 0;
            exit = false;
            while (!exit)
            {
                strtosearch = "ship-portrait-full-star-inactive";
                index = html.IndexOf(strtosearch, Position);
                if (index == -1) exit = true;
                else
                {
                    star++;
                    Position = index + strtosearch.Length;
                }
            }
            newship.Stars = 7 - star;
            #endregion

            #region lvl
            strtosearch = "ship-portrait-full-frame-level\">";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchEnd = "</div>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, Position);
                if (restindexEnd != -1)
                {
                    int start = Position;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.Level = valueint;
                }
            }
            else return true;  ////not opened ship
            #endregion

            #region Skills
            strtosearch = "<h4>Skills</h4>";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                exit = false;
                strtosearch = "<h5>Primary Attributes</h5>";
                int EndIndex = html.IndexOf(strtosearch, Position);
                if (EndIndex != -1)
                {
                    while (!exit)
                    {
                        Ability abil = new Ability();

                        string reststrTosearchStart = "data-title=\"";
                        int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                        string reststrTosearchEnd = "\">";
                        int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                        if (restindexStart != -1 && restindexEnd != -1)
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;
                            SetAbilityPropertiesFromString(abil, value);
                        }
                        else exit = true;

                        reststrTosearchStart = "pc-skill-name\">";
                        restindexStart = html.IndexOf(reststrTosearchStart, Position);
                        reststrTosearchEnd = "</div>";
                        restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                        if (restindexStart != -1 && restindexEnd != -1)
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;
                            abil.Name = value;
                            if (newship.Abilities == null) newship.Abilities = new List<Ability>();
                            newship.Abilities.Add(abil);
                        }
                        else exit = true;

                        if (Position == -1 || Position > EndIndex) exit = true;
                    }
                }
            }
            #endregion

            #region Crew
            strtosearch = "<h4>Crew</h4>";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;

            strtosearch = "<h5>Power</h5>";
            int indexPower = html.IndexOf(strtosearch, Position);

            if (index != -1)
            {
                exit = false;
                while (!exit)
                {
                    string reststrTosearchStart = "title=\"";
                    int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    string reststrTosearchEnd = "\">";
                    int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        if (restindexStart == -1 || restindexEnd == -1 || restindexStart > indexPower) exit = true;
                        else
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;

                            value = FixCharacterName(WebUtility.HtmlDecode(value));

                            if (newship.Crew == null) newship.Crew = new List<string>();
                            newship.Crew.Add(value);
                        }
                    }
                }
            }
            #endregion

            #region Power
            if (indexPower != -1)
            {
                string reststrTosearchStart = "stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position - 50);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;


                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.Power = valueint;
                }
            }
            #endregion

            #region General
            strtosearch = "<h5>General</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "group-stat-value\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "<";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.Health = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.Protection = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.Speed = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.CriticalDamage = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.Potency = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.Tenacity = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.HealthSteal = valuedecimal;
                }

            }
            #endregion

            #region Physical Offense
            strtosearch = "<h5>Physical Offense</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "group-stat-value\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "<";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.PhysicalDamage = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.PhysicalCriticalChance = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newship.ArmorPenetration = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.PhysicalAccuracy = valuedecimal;
                }
            }
            #endregion

            #region Physical Survivability
            strtosearch = "<h5>Physical Survivability</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "group-stat-value\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "<";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.Armor = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.DodgeChance = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.PhysicalCriticalAvoidance = valuedecimal;
                }
            }
            #endregion

            #region Special Offense
            strtosearch = "<h5>Special Offense</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "group-stat-value\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "<";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newship.SpecialDamage = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.SpecialCriticalChance = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newship.ResistancePenetration = valueint;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.SpecialAccuracy = valuedecimal;
                }
            }
            #endregion

            #region Special Survivability
            strtosearch = "<h5>Special Survivability</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "group-stat-value\"";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "<";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.Resistance = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.DeflectionChance = valuedecimal;
                }

                reststrTosearchStart = "group-stat-value\"";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "<";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (value.StartsWith('>')) value = value.Remove(0, 1);
                    else
                    {
                        //remove tooltip
                        reststrTosearchStart = ">";
                        restindexStart = value.IndexOf(reststrTosearchStart);
                        if (restindexStart != -1)
                        {
                            start = restindexStart + reststrTosearchStart.Length;
                            value = value.Substring(start);
                        }
                    }

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newship.SpecialCriticalAvoidance = valuedecimal;
                }
            }
            #endregion

            return true;
        }
        private CharacterDto GetChar(string html, out int Position)
        {
            CharacterDto ret = new CharacterDto();

            string strTosearch = "<div class=\"player-char-portrait char-portrait-full char-portrait";
            int index = html.IndexOf(strTosearch);
            if (index != -1)
            {
                string rest = html.Substring(index + strTosearch.Length);
                string reststrTosearchStart = "href=\"";
                int restindexStart = rest.IndexOf(reststrTosearchStart);
                string reststrTosearchEnd = "\"";
                int restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    string link = rest.Substring(start, length);
                    ret.SWGoHUrl = link;
                }
            }

            strTosearch = "<div class=\"char-portrait-full-gear-level";
            index = html.IndexOf(strTosearch);
            if (index != -1)
            {
                string gear = html.Substring(index + strTosearch.Length, 5);
                gear = gear.Replace('\\', ' ');
                gear = gear.Replace('/', ' ');
                gear = gear.Replace('"', ' ');
                gear = gear.Replace('<', ' ');
                gear = gear.Replace('>', ' ');
                gear = gear.Trim();

                ret.Gear = ConvertGearStr(gear);
            }


            strTosearch = "<div class=\"char-portrait-full-level";
            index = html.IndexOf(strTosearch);
            if (index != -1)
            {
                string lvl = html.Substring(index + strTosearch.Length, 5);
                lvl = lvl.Replace('\\', ' ');
                lvl = lvl.Replace('/', ' ');
                lvl = lvl.Replace('"', ' ');
                lvl = lvl.Replace('<', ' ');
                lvl = lvl.Replace('>', ' ');
                lvl = lvl.Trim();

                int lvlint = -1;
                bool ret1 = int.TryParse(lvl, out lvlint);
                if (ret1) ret.Level = lvlint;
            }

            strTosearch = "<div class=\"collection-char-name";
            index = html.IndexOf(strTosearch);
            int lastindex = 0;
            if (index != -1)
            {
                string rest = html.Substring(index + strTosearch.Length);

                string reststrTosearchStart = "nofollow\">";
                int restindexStart = rest.IndexOf(reststrTosearchStart);
                string reststrTosearchEnd = "</a>";
                int restindexEnd = rest.IndexOf(reststrTosearchEnd);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    string name = WebUtility.HtmlDecode(rest.Substring(start, length));
                    ret.Name = name;
                }

                lastindex = restindexEnd;
            }
            Position = index + lastindex;

            return ret;
        }

        private bool FillCharData(CharacterDto newchar)
        {
            string html = "";
            bool ret1 = false;
            int valueint = 0;
            double valuedecimal = 0;

            Uri uri = new Uri("https://swgoh.gg" + newchar.SWGoHUrl);
            try
            {
                html = web.DownloadString(uri);
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Exception : " + e.Message);
                return false;
            }

            string value;

            #region Labels
            bool exit = false;
            int Position = 0;
            string strtosearch = "";
            int index = 0;

            strtosearch = "char-tag char-alignment";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position+40);
                string reststrTosearchEnd = "</a>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (newchar.CharacterTags == null) newchar.CharacterTags = new List<string>();
                    newchar.CharacterTags.Add(value);
                }
            }

            strtosearch = "char-tag char-role\">";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</a>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    if (newchar.CharacterTags == null) newchar.CharacterTags = new List<string>();
                    newchar.CharacterTags.Add(value);
                }
            }

            while (!exit)
            { 
                strtosearch = "char-category\"";
                index = html.IndexOf(strtosearch, Position);
                Position = index + strtosearch.Length;
                if (index != -1)
                {
                    string reststrTosearchStart = "\">";
                    int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    string reststrTosearchEnd = "</a>";
                    int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = html.Substring(start, length);
                        Position = restindexEnd;

                        if (newchar.CharacterTags == null) newchar.CharacterTags = new List<string>();
                        newchar.CharacterTags.Add(value);
                    }
                }
                else
                    exit = true;
            }
            #endregion

            #region GP BreakDown // Stars
            strtosearch = "<h4>Galactic Power Breakdown</h4>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                strtosearch = "Stars";
                index = html.IndexOf(strtosearch, Position);
                Position = index + strtosearch.Length;
                if (index != -1)
                {
                    string reststrTosearchStart = "gp-stat-amount-current\">";
                    int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                    string reststrTosearchEnd = "</span>";
                    int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = html.Substring(start, length);
                        Position = restindexEnd;

                        newchar.Stars = GetStarsFromValue(value);
                    }
                }
            }
            #endregion

            strtosearch = "<h4>Skills</h4>";
            index = html.IndexOf(strtosearch, Position);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                exit = false;
                strtosearch = "<h5>Primary Attributes</h5>";
                int EndIndex = html.IndexOf(strtosearch, Position);
                if (EndIndex != -1)
                {
                    while (!exit)
                    {
                        Ability abil = new Ability();

                        string reststrTosearchStart = "data-title=\"";
                        int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                        string reststrTosearchEnd = "\">";
                        int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                        if (restindexStart != -1 && restindexEnd != -1)
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;
                            SetAbilityPropertiesFromString(abil, value);
                        }
                        else exit = true;

                        reststrTosearchStart = "pc-skill-name\">";
                        restindexStart = html.IndexOf(reststrTosearchStart, Position);
                        reststrTosearchEnd = "</div>";
                        restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                        if (restindexStart != -1 && restindexEnd != -1)
                        {
                            int start = restindexStart + reststrTosearchStart.Length;
                            int length = restindexEnd - start;
                            value = html.Substring(start, length);
                            Position = restindexEnd;
                            abil.Name = value;
                            if (newchar.Abilities == null) newchar.Abilities = new List<Ability>();
                            newchar.Abilities.Add(abil);
                        }
                        else exit = true;

                        if (Position == -1 || Position > EndIndex) exit = true;
                    }
                }
            }

            strtosearch = "<h5>Power</h5>";
            index = html.IndexOf(strtosearch, Position);
            if (index != -1)
            {
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position - 50);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Power = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.StatPower = valueint;
                }
            }



            #region General
            strtosearch = "<h5>General</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Health = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Protection = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Speed = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.CriticalDamage = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.Potency = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.Tenacity = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.HealthSteal = valuedecimal;
                }

            }
            #endregion

            #region Physical Offense
            strtosearch = "<h5>Physical Offense</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.PhysicalDamage = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.PhysicalCriticalChance = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.ArmorPenetration = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.PhysicalAccuracy = valuedecimal;
                }
            }
            #endregion

            #region Physical Survivability
            strtosearch = "<h5>Physical Survivability</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.Armor = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.DodgeChance = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.PhysicalCriticalAvoidance = valuedecimal;
                }
            }
            #endregion

            #region Special Offense
            strtosearch = "<h5>Special Offense</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newchar.SpecialDamage = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.SpecialCriticalChance = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newchar.ResistancePenetration = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.SpecialAccuracy = valuedecimal;
                }
            }
            #endregion

            #region Special Survivability
            strtosearch = "<h5>Special Survivability</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = html.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.Resistance = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.DeflectionChance = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = html.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = html.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = html.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = double.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.SpecialCriticalAvoidance = valuedecimal;
                }
            }
            #endregion

            LoadMods(html, newchar);

            return true;
        }

        private void LoadMods(string html, CharacterDto newchar)
        {
            int count = 0;

            string strtosearch = "<h4>Stat Mods</h4>";
            int index = html.IndexOf(strtosearch);
            if (index != -1)
            {
                int Position = index + strtosearch.Length;

                Mod mod1 = FetchMod(html, newchar, ref Position, "1");
                if (mod1 != null)
                {
                    if (newchar.Mods == null) newchar.Mods = new List<Mod>();
                    newchar.Mods.Add(mod1); count++;
                }

                Mod mod2 = FetchMod(html, newchar, ref Position, "2");
                if (mod2 != null)
                {
                    if (newchar.Mods == null) newchar.Mods = new List<Mod>();
                    newchar.Mods.Add(mod2); count++;
                }

                Mod mod3 = FetchMod(html, newchar, ref Position, "3");
                if (mod3 != null)
                {
                    if (newchar.Mods == null) newchar.Mods = new List<Mod>();
                    newchar.Mods.Add(mod3); count++;
                }

                Mod mod4 = FetchMod(html, newchar, ref Position, "4");
                if (mod4 != null)
                {
                    if (newchar.Mods == null) newchar.Mods = new List<Mod>();
                    newchar.Mods.Add(mod4); count++;
                }

                Mod mod5 = FetchMod(html, newchar, ref Position, "5");
                if (mod5 != null)
                {
                    if (newchar.Mods == null) newchar.Mods = new List<Mod>();
                    newchar.Mods.Add(mod5); count++;
                }

                Mod mod6 = FetchMod(html, newchar, ref Position, "6");
                if (mod6 != null)
                {
                    if (newchar.Mods == null) newchar.Mods = new List<Mod>();
                    newchar.Mods.Add(mod6); count++;
                }
            }
        }

        private Mod FetchMod(string html, CharacterDto newchar, ref int Position, string slot)
        {
            string value = "";
            string value1 = "";
            int count = 0;
            Mod mod = null;

            string rest = html.Substring(Position);
            string reststrTosearchStart = "pc-statmod-slot" + slot + " ";
            int restindexStart = rest.IndexOf(reststrTosearchStart);
            if (restindexStart != -1)
            {
                Position = restindexStart;
                mod = new Mod();

                count++;

                int endofmod = rest.IndexOf("</div></div></div>", Position);

                reststrTosearchStart = "statmod-title\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                string reststrTosearchEnd = "</div>";
                int restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;
                    mod.Name = value;
                    mod.Type = GetModType(value);
                    mod.Star = GetModStar(value);
                    mod.Rarity = GetModRarity(value);
                }


                reststrTosearchStart = "statmod-level\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;
                    bool ret = int.TryParse(value, out int level);
                    if (ret) mod.Level = level;
                }

                reststrTosearchStart = "statmod-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                if (restindexStart < endofmod)
                {
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = rest.Substring(start, length);
                        Position = restindexEnd;
                    }

                    reststrTosearchStart = "statmod-stat-label\">";
                    restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value1 = rest.Substring(start, length);
                        Position = restindexEnd;
                    }
                    mod.PrimaryStat = GetModFromString(value, value1);
                }

                reststrTosearchStart = "statmod-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                if (restindexStart < endofmod)
                {
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = rest.Substring(start, length);
                        Position = restindexEnd;
                    }

                    reststrTosearchStart = "statmod-stat-label\">";
                    restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value1 = rest.Substring(start, length);
                        Position = restindexEnd;
                    }
                    mod.SecondaryStat = new List<ModStat>
                    {
                        GetModFromString(value, value1)
                    };
                }

                reststrTosearchStart = "statmod-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                if (restindexStart < endofmod)
                {
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = rest.Substring(start, length);
                        Position = restindexEnd;
                    }

                    reststrTosearchStart = "statmod-stat-label\">";
                    restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value1 = rest.Substring(start, length);
                        Position = restindexEnd;
                    }
                    mod.SecondaryStat.Add(GetModFromString(value, value1));
                }

                reststrTosearchStart = "statmod-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                if (restindexStart < endofmod)
                {
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = rest.Substring(start, length);
                        Position = restindexEnd;
                    }

                    reststrTosearchStart = "statmod-stat-label\">";
                    restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value1 = rest.Substring(start, length);
                        Position = restindexEnd;
                    }
                    mod.SecondaryStat.Add(GetModFromString(value, value1));
                }

                reststrTosearchStart = "statmod-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                if (restindexStart < endofmod)
                {
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value = rest.Substring(start, length);
                        Position = restindexEnd;
                    }

                    reststrTosearchStart = "statmod-stat-label\">";
                    restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                    reststrTosearchEnd = "</span>";
                    restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                    if (restindexStart != -1 && restindexEnd != -1)
                    {
                        int start = restindexStart + reststrTosearchStart.Length;
                        int length = restindexEnd - start;
                        value1 = rest.Substring(start, length);
                        Position = restindexEnd;
                    }
                    mod.SecondaryStat.Add(GetModFromString(value, value1));
                }

                return mod;
            }
            else
            {
                mod = null;
                return null;
            }
        }

        public static void TestZetas()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = SWGoH.MongoDBRepo.BuildApiUrl("Config.Character", "&q={}", "", "", "");

                string response = client.GetStringAsync(url).Result;
                if (response != "" && response != "[  ]")
                {
                    List<CharacterConfigDto> document = BsonSerializer.Deserialize<List<CharacterConfigDto>>(response);

                    int totalzetacount = 0;
                    int characterzeta = 0;
                    if (document != null)
                    {
                        foreach (CharacterConfigDto character in document)
                        {
                            characterzeta = 0;
                            if (character.Abilities != null)
                            {
                                foreach (ConfigAbility ability in character.Abilities)
                                {
                                    if (ability.AbilityType == ConfigAbilityType.Zeta) characterzeta++;
                                }
                            }
                            totalzetacount += characterzeta;
                        }
                    }
                }
            }
        }

        #region Convertions
        private ModStat GetModFromString(string value, string value1)
        {
            ModStat ret = new ModStat();

            value1 = value1.Replace(" ", "");

            if (value.Contains("%")) ret.ValueType = ModValueType.Percentage;
            else ret.ValueType = ModValueType.Flat;

            if (value1.ToLower().Contains("speed")) ret.StatType = ModStatType.Speed;
            else if (value1.ToLower().Contains("criticalchance")) ret.StatType = ModStatType.CriticalChance;
            else if (value1.ToLower().Contains("criticaldamage")) ret.StatType = ModStatType.CriticalDamage;
            else if (value1.ToLower().Contains("potency")) ret.StatType = ModStatType.Potency;
            else if (value1.ToLower().Contains("tenacity")) ret.StatType = ModStatType.Tenacity;
            else if (value1.ToLower().Contains("accuracy")) ret.StatType = ModStatType.Accuracy;
            else if (value1.ToLower().Contains("criticalavoidance")) ret.StatType = ModStatType.CriticalAvoidance;
            else if (value1.ToLower().Contains("offense")) ret.StatType = ModStatType.Offense;
            else if (value1.ToLower().Contains("defense")) ret.StatType = ModStatType.Defense;
            else if (value1.ToLower().Contains("health")) ret.StatType = ModStatType.Health;
            else if (value1.ToLower().Contains("protection")) ret.StatType = ModStatType.Protection;

            string val = value.Replace('%', ' ');
            val = val.Replace('+', ' ');
            val = val.Trim();
            bool boolret = double.TryParse(val, NumberStyles.Any, new CultureInfo("en-US"), out double dec);
            if (boolret) ret.Value = dec;

            return ret;
        }

        private string GetModRarity(string value)
        {
            string rarity = "";
            int index = value.IndexOf('-');
            rarity = value.Substring(index + 1);
            index = rarity.IndexOf(' ');
            rarity = rarity.Substring(0, index);
            return rarity;
        }

        private int GetModStar(string value)
        {
            if (value.ToLower().StartsWith("mk v")) return 5;
            else if (value.ToLower().StartsWith("mk iv")) return 4;
            else if (value.ToLower().StartsWith("mk iii")) return 3;
            else if (value.ToLower().StartsWith("mk ii")) return 2;
            else if (value.ToLower().StartsWith("mk i")) return 1;
            return 0;
        }

        private ModSlot GetModType(string value)
        {
            if (value.ToLower().Contains("transmitter")) return ModSlot.Transmitter;
            else if (value.ToLower().Contains("receiver")) return ModSlot.Receiver;
            else if (value.ToLower().Contains("processor")) return ModSlot.Processor;
            else if (value.ToLower().Contains("holo-array")) return ModSlot.HoloArray;
            else if (value.ToLower().Contains("data-bus")) return ModSlot.DataBus;
            else if (value.ToLower().Contains("multiplexer")) return ModSlot.Multiplexer;
            return ModSlot.Transmitter;
        }

        private int GetStarsFromValue(string value)
        {
            string tmp = value.Replace(",", "");

            bool ret1 = int.TryParse(tmp, out int valueint);
            if (ret1)
            {
                if (valueint > 4000) return 7;
                else if (valueint > 2500) return 6; //(2660)
                else if (valueint > 1400) return 5; //(1520)
                else if (valueint > 1000) return 4; //(1013)
                else if (valueint > 670) return 3; //(675)
                else if (valueint > 400) return 2; //(450)
                else if (valueint > 0) return 1; //(1520)
            }
            return 0;
        }
        private void SetAbilityPropertiesFromString(Ability abil, string value)
        {
            string[] values = value.Split(new char[] { 'o', 'f' });
            if (values.Length != 3) return;
            string value1 = values[0];
            value1 = value1.Replace("Level", "");
            value1 = value1.Trim();
            bool ret = int.TryParse(value1, out int lvl);
            if (!ret) return;
            string value2 = values[2];
            value2 = value2.Replace("(MAXED)", "");
            value2 = value2.Trim();
            ret = int.TryParse(value2, out int maxlvl);
            if (!ret) return;
            abil.Level = lvl;
            abil.MaxLevel = maxlvl;
        }
        private int ConvertGearStr(string roman)
        {
            int ret = 0;
            Dictionary<char, int> RomanMap = new Dictionary<char, int>() { { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 } };
            for (int i = 0; i < roman.Length; i++)
            {
                if (i + 1 < roman.Length && RomanMap[roman[i]] < RomanMap[roman[i + 1]])
                {
                    ret -= RomanMap[roman[i]];
                }
                else
                {
                    ret += RomanMap[roman[i]];
                }
            }
            return ret;
        }
        #endregion
    }
}
