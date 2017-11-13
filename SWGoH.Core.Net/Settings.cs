using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SwGoh
{
    public class Settings
    {
        public static Settings appSettings = new Settings();
        public static bool Get()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory + "Settings";
            string fname = directory + "\\" + "Settings" + @".json";

            if (File.Exists(fname))
            {
                var lines = File.ReadAllText(fname);
                JsonConvert.PopulateObject(lines, appSettings);
            }
            else
            {
                SwGoh.Log.ConsoleMessage("Unable To Load Settings");
                return false;
            }
            return true;
        }
        public string MongoApiKey { get; set; }
        public double HoursForNextCheckLastswGohUpdate { get; set; }

        public int DelayPerCharacter { get; set; }
        public int DelayErrorAtCharacter { get; set; }

        public int DelayPerPlayerAtGuildSearch { get; set; }
        public int DelayErrorPerPlayerAtGuildSearch { get; set; }

        public int GlobalConsoleTimerInterval { get; set; }

        public  double MinutesUntilNextProcess { get; set; }
    }
}
