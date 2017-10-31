using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwGoh
{
    public partial class PlayerDto
    {
        private System.Net.WebClient web = null;

        public PlayerDto(string name)
        {
            PlayerName = name;
        }

        public string PlayerName { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<CharacterDto> Characters { get; set; }


        public void Import()
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
        public void Export()
        {
            try
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory +  "PlayerJsons";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Formatting = Formatting.Indented;

                string fname = directory + "\\" + PlayerName + @".json";
                using (StreamWriter sw = new StreamWriter(fname))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this);
                }
                Console.WriteLine("Created : " + PlayerName + "'s json File");

            }
            catch
            {
                //Error Occured , Contact Developer
            }
        }
        public bool ParseSwGoh()
        {
            if (PlayerName == null || PlayerName == "") return false;
            bool retbool = true;

            web = new System.Net.WebClient();
            Uri uri = new Uri("https://swgoh.gg/u/" + PlayerName + "/collection/");

            string html = "";
            try
            {
                html = web.DownloadString(uri);
            }
            catch { web = null; return false; }

            int Position = 0;
            FillPlayerData(html, out Position);
            bool ret = CheckLastUpdateWithCurrent();
            if (ret) { FillPlayerCharacters(html, Position); retbool = true; }
            else { Import(); retbool = false; }
            web = null;
            return retbool;
        }

        private bool CheckLastUpdateWithCurrent()
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
                    if (filelastupdated.CompareTo(this.LastUpdated) == 0) return false;
                }
            }
            else return true;
            return true;
        }

        /// <summary>
        /// Fills Players characters
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Position"></param>
        private void FillPlayerCharacters(string html,int Position)
        {
            if (Position == -1) return;

            Characters = new List<CharacterDto>();

            html = html.Substring(Position);

            bool exit = false;
            int count = 0;
            while (!exit)
            {
                count++;
                CharacterDto newchar = GetChar(html, out Position);
                bool ret = FillCharData(newchar);
                if (Position > 0) html = html.Substring(Position);
                else exit = true;
                if (newchar.Name != null) Characters.Add(newchar);
                if (html.Length < 500) exit = true;
                Console.WriteLine("          " + count.ToString () + ") Added character : " + newchar.Name);
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Fills player properties like LastUpdated
        /// </summary>
        /// <param name="html"></param>
        private void FillPlayerData(string html, out int Position)
        {

            string strtosearch = "Last updated:";
            int index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string rest = html.Substring(Position);
                string reststrTosearchStart = "data-datetime=\"";
                int restindexStart = rest.IndexOf(reststrTosearchStart);
                string reststrTosearchEnd = "\" data";
                int restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    string value = rest.Substring(start, length);
                    Position = restindexEnd;
                    //DateTime updated = DateTime.from
                    value = value.Replace('T', ',');
                    value = value.Replace('Z', ' ');
                    value = value.Trim();
                    LastUpdated = DateTime.ParseExact(value, "yyyy-MM-dd,HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
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
                    string name = rest.Substring(start, length);
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
            decimal valuedecimal = 0;

            Uri uri = new Uri("https://swgoh.gg" + newchar.SWGoHUrl);
            try
            {
                html = web.DownloadString(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
                return false;
            }

            string value;

            #region General
            string strtosearch = "<h5>General</h5>";
            int index = html.IndexOf(strtosearch);
            int Position = index + strtosearch.Length;
            if (index != -1)
            {
                string rest = html.Substring(Position);
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = rest.IndexOf(reststrTosearchStart);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Health = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Protection = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.Speed = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newchar.CriticalDamage = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;
                    
                    ret1 = decimal.TryParse(value.Replace ('%',' ').Trim (), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.Potency = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = decimal.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.Tenacity = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newchar.HealthSteal = valueint;
                }

            }
            #endregion

            #region Physical Offense
            strtosearch = "<h5>Physical Offense</h5>";
            index = html.IndexOf(strtosearch);
            Position = index + strtosearch.Length;
            if (index != -1)
            {
                string rest = html.Substring(Position);
                string reststrTosearchStart = "pc-stat-value\">";
                int restindexStart = rest.IndexOf(reststrTosearchStart);
                string reststrTosearchEnd = "</span>";
                int restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.PhysicalDamage = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = decimal.TryParse(value.Replace('%', ' ').Trim(), NumberStyles.Any, new CultureInfo("en-US"), out valuedecimal);
                    if (ret1) newchar.PhysicalCriticalChance = valuedecimal;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value, out valueint);
                    if (ret1) newchar.ArmorPenetration = valueint;
                }

                reststrTosearchStart = "pc-stat-value\">";
                restindexStart = rest.IndexOf(reststrTosearchStart, Position);
                reststrTosearchEnd = "</span>";
                restindexEnd = rest.IndexOf(reststrTosearchEnd, restindexStart + reststrTosearchStart.Length);
                if (restindexStart != -1 && restindexEnd != -1)
                {
                    int start = restindexStart + reststrTosearchStart.Length;
                    int length = restindexEnd - start;
                    value = rest.Substring(start, length);
                    Position = restindexEnd;

                    ret1 = int.TryParse(value.Replace('%', ' ').Trim(), out valueint);
                    if (ret1) newchar.PhysicalAccuracy = valueint;
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

                newchar.Mods = new List<Mod>();

                Mod mod1 = FetchMod(html, newchar, ref Position, "1");
                if (mod1 != null) { newchar.Mods.Add(mod1); count++; }

                Mod mod2 = FetchMod(html, newchar, ref Position, "2");
                if (mod2 != null) { newchar.Mods.Add(mod2); count++; }

                Mod mod3 = FetchMod(html, newchar, ref Position, "3");
                if (mod3 != null) { newchar.Mods.Add(mod3); count++; }

                Mod mod4 = FetchMod(html, newchar, ref Position, "4");
                if (mod4 != null) { newchar.Mods.Add(mod4); count++; }

                Mod mod5 = FetchMod(html, newchar, ref Position, "5");
                if (mod5 != null) { newchar.Mods.Add(mod5); count++; }

                Mod mod6 = FetchMod(html, newchar, ref Position, "6");
                if (mod6 != null) { newchar.Mods.Add(mod6); count++; }
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
                    int level = 0;
                    bool ret = int.TryParse(value, out level);
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
                    mod.SecondaryStat = new List<ModStat>();
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

        private ModStat GetModFromString(string value, string value1)
        {
            ModStat ret = new ModStat();

            if (value.Contains("%")) ret.ValueType = ModValueType.Percentage;
            else ret.ValueType = ModValueType.Flat;

            if (value1.ToLower().Contains("speed")) ret.StatType = ModStatType.Speed;
            else if (value1.ToLower().Contains("criticalchance")) ret.StatType = ModStatType.CriticalChance;
            else if (value1.ToLower().Contains("sriticaldamage")) ret.StatType = ModStatType.CriticalDamage;
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
            decimal dec = 0;
            bool boolret = decimal.TryParse(val, NumberStyles.Any, new CultureInfo("en-US"), out dec);
            if (boolret) ret.Value = dec;

            return ret;
        }

        private string GetModRarity(string value)
        {
            string rarity = "";
            int index = value.IndexOf('-');
            rarity = value.Substring(index+1);
            index = rarity.IndexOf(' ');
            rarity = rarity.Substring(0,index);
            return rarity;
        }

        private long GetModStar(string value)
        {
            if (value.ToLower ().StartsWith("mk v")) return 5;
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
    }
}
