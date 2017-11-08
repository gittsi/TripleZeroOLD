using SWGoH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwGoh
{
    public enum Command
    {
        UpdatePlayer = 1,
        UpdateGuild = 2,
        UpdateGuildWithNoChars = 3,
        UpdatePlayers = 4,
        Help = 5,
        UnKnown = 6,
        Test = 7,
    }


    class Program
    {
        public static bool isWorking = false;
        public static bool mPrintedNothingToProcess = false;
        public static int mPrintedNothingToProcessdots = 0;
        public static int mPrintedNothingToProcessdotsTotal = 4;

        static void Main(string[] args)
        {
            Timer t = new Timer(new TimerCallback(TimerProc));
            t.Change(0, 3000);
            
            Console.ReadLine();
        }

        private static void TimerProc(Object o)
        {
            if (isWorking) return;
            isWorking = true;
            //ExecuteCommand("test", ""); return;
            QueuePlayer q = QueueMethods.GetQueu();
            if (q != null)
            {
                ExecuteCommand(q.Command, q.PlayerName);
                QueueMethods.RemoveFromQueu(q);
                mPrintedNothingToProcess = false;
                mPrintedNothingToProcessdots = 0;
            }
            else
            {
                string mMessage = "Nothing to process";
                if (!mPrintedNothingToProcess) Console.Write(mMessage);
                Console.Write("."); mPrintedNothingToProcessdots++;
                if (mPrintedNothingToProcessdots == mPrintedNothingToProcessdotsTotal)
                {
                    mPrintedNothingToProcessdots = 0;
                    for (int i=0;i< mPrintedNothingToProcessdotsTotal;i++)  Console.Write("\b \b");
                }
                mPrintedNothingToProcess = true;
            }
            isWorking = false;
            GC.Collect();
        }

        private static void ExecuteCommand(string commandstr, string pname)
        {
            ExportMethodEnum mExportMethod = ExportMethodEnum.Database;

            Command command = Command.UnKnown;
            if (commandstr == "ups") command = Command.UpdatePlayers;
            else if (commandstr == "up") command = Command.UpdatePlayer;
            else if (commandstr == "ug") command = Command.UpdateGuild;
            else if (commandstr == "ugnochars") command = Command.UpdateGuildWithNoChars;
            else if (commandstr == "help") command = Command.Help;
            else if (commandstr == "test") command = Command.Test;
            else command = Command.UnKnown;

            switch (command)
            {
                case Command.UpdatePlayer:
                    {
                        SwGoh.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true);
                        if (ret == 1)
                        {
                            player.LastClassUpdated = DateTime.UtcNow;
                            player.Export(mExportMethod);
                        }
                        break;
                    }
                case Command.UpdateGuild:
                    {
                        SwGoh.GuildDto guild = new GuildDto();
                        guild.Name = guild.GetGuildNameFromAlias(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateAllPlayers(mExportMethod, true);
                        break;
                    }
                case Command.UpdatePlayers:
                    {
                        string[] arg = pname.Split(',');
                        for (int i = 0; i < arg.Length; i++)
                        {
                            SwGoh.PlayerDto player = new PlayerDto(arg[i]);
                            int ret = player.ParseSwGoh(mExportMethod, true);
                            if (ret == 1)
                            {
                                player.LastClassUpdated = DateTime.UtcNow;
                                player.Export(mExportMethod);
                            }
                        }
                        break;
                    }
                case Command.UpdateGuildWithNoChars:
                    {
                        SwGoh.GuildDto guild = new GuildDto();
                        guild.Name = guild.GetGuildNameFromAlias(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateOnlyGuildWithNoChars(mExportMethod);
                        break;
                    }
                case Command.Help:
                    {
                        Console.WriteLine("Command Update Player");
                        Console.WriteLine("Usage : <app> up <playername>");
                        Console.WriteLine("Update only one player with his characters.");
                        Console.WriteLine("");
                        Console.WriteLine("Command Update Players");
                        Console.WriteLine("Usage : <app> ups <playername1> <playername2> <playername3>");
                        Console.WriteLine("Update provided players with their characters.");
                        Console.WriteLine("");
                        Console.WriteLine("Command Update Guild");
                        Console.WriteLine("Usage : <app> ug <guildname>");
                        Console.WriteLine("Update all players with their characters and at the end update the guild file.");
                        Console.WriteLine("");
                        Console.WriteLine("Command Update Guild without the characters of the players");
                        Console.WriteLine("Usage : <app> ugnochars <guildname>");
                        Console.WriteLine("Update the guild file.");
                        Console.WriteLine("");
                        Console.WriteLine("Command Help");
                        Console.WriteLine("Usage : <app> help");
                        Console.WriteLine("You already know this command!!!!!");
                        break;
                    }
                case Command.Test:
                    {
                        //SwGoh.CharactersConfig.ExportCharacterFilesToDB();

                        //SwGoh.GuildDto guild = new GuildDto();
                        //guild.Name = guild.GetGuildNameFromAlias("41st");
                        //guild.ParseSwGoh();
                        //for (int i = 0; i < guild.PlayerNamesForURL.Count; i++)
                        //{
                        //    QueueMethods.AddPlayer(guild.PlayerNamesForURL[i], "up", 2);
                        //}
                        //QueueMethods.AddPlayer("41st", "ugnochars", 1);

                        //QueueMethods.AddPlayer("newholborn", "up",3);
                        //QueueMethods.AddPlayer("oaraug", "up", 3);
                        //QueueMethods.AddPlayer("tsitas_66", "up",1);
                        //QueueMethods.AddPlayer("tsitas_66", "up", 3);
                        QueueMethods.AddPlayer("41st", "ugnochars", 3);
                        //for (int i = 0; i < 10; i++)
                        //{
                        //    QueueMethods.AddPlayer("tsitas_66", "up");
                        //}
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown command , please try again.!!!!");
                        break;
                    }
            }
        }
    }
}
