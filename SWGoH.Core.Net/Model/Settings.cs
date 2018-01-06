using Newtonsoft.Json;
using System;
using System.IO;

namespace SWGoH
{
    public class Settings
    {
        public static Settings appSettings = new Settings();
        public static bool Get()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory + "Settings";
            string fname = directory + "/" + "Settings" + @".json";

            if (File.Exists(fname))
            {
                var lines = File.ReadAllText(fname);
                JsonConvert.PopulateObject(lines, appSettings);
                if (appSettings.Database.Equals("triplezero", StringComparison.InvariantCultureIgnoreCase))
                {
                    appSettings.DatabaseID1 = 245805;
                    appSettings.DatabaseID2 = 45805;
                }
                else if (appSettings.Database.Equals("triplezerodev", StringComparison.InvariantCultureIgnoreCase))
                {
                    appSettings.DatabaseID1 = 161455;
                    appSettings.DatabaseID2 = 61455;
                }

                try
                {
                    string computer = System.Environment.MachineName;
                    if (computer != null && computer != "") appSettings.ComputerName = computer;
                }
                catch (Exception e)
                {
                    //SWGoH.Log.ConsoleMessage("Error retrieving computer name");
                    Console.WriteLine("Error retrieving computer name");
                }
            }
            else
            {
                //SWGoH.Log.ConsoleMessage("Unable To Load Settings : " + directory + " : " + fname);
                Console.WriteLine("Unable To Load Settings : " + directory + " : " + fname);
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
        public int LogToFile { get; set; }
        public string Database { get; set; }

        public bool UpdateOnlyManual { get; set; }
        public int DatabaseID1 { get; set; }
        public int DatabaseID2 { get; set; }
        public string ComputerName { get; set; }
        public string UpdateOnlyGuild { get; set; }
    }
}
