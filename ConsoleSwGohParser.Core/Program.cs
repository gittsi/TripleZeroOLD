using SwGoH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SwGoh
{
    public enum Command
    {
        UpdatePlayer = 1,
        UpdateGuild = 2,
        UpdateGuildWithNoChars = 3,
        UpdatePlayers = 4,
        GetNewCharacters = 5,
        Help = 6,
        UnKnown = 7,
        Test = 8,
    }


    class Program
    {
        private static bool isWorking = false;
        private static bool mPrintedNothingToProcess = false;
        private static int mPrintedNothingToProcessdots = 0;
        private static int mPrintedNothingToProcessdotsTotal = 4;
        private static int mTimerdelay = 5000;
        private static bool mExportLog = false;
        
        static void Main(string[] args)
        {
            if (mExportLog) SwGoH.Log.Initialize("log.txt" , mExportLog );

            Timer t = new Timer(new TimerCallback(TimerProc));
            t.Change(0, mTimerdelay);

            Console.ReadLine();

            if (mExportLog) SwGoH.Log.FileFinalize();
        }
        private static void TimerProc(Object o)
        {
            if (isWorking) return;
            isWorking = true;
            Timer t = o as Timer;
            t.Change(Timeout.Infinite, Timeout.Infinite);

            //ExecuteCommand("getnewchars", "aramil"); return; 

            QueuePlayer q = QueueMethods.GetQueu();
            if (q != null)
            {
                Console.WriteLine("");
                ExecuteCommand(q.Command, q.PlayerName);
                QueueMethods.RemoveFromQueu(q);

                mPrintedNothingToProcess = false;
                mPrintedNothingToProcessdots = 0;
            }
            else
            {
                int now = DateTime.Now.Minute;
                if (now == 0 || now == 15 || now == 30 || now == 45)
                {
                    PlayerDto player = QueueMethods.GetLastUpdatedPlayer("41st");
                    if (player != null)
                    {
                        QueueMethods.AddPlayer(player.PlayerName, "up", 1);
                    }
                }

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
            t.Change(mTimerdelay, mTimerdelay);
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
            else if (commandstr == "getnewchars") command = Command.GetNewCharacters;
            else if (commandstr == "help") command = Command.Help;
            else if (commandstr == "test") command = Command.Test;
            else command = Command.UnKnown;

            switch (command)
            {
                case Command.UpdatePlayer:
                    {
                        SwGoh.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true,false);
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
                        guild.Name = GuildDto.GetGuildNameFromAlias(pname);
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
                            int ret = player.ParseSwGoh(mExportMethod, true,false);
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
                        guild.Name = GuildDto.GetGuildNameFromAlias(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateOnlyGuildWithNoChars(mExportMethod);
                        break;
                    }
                case Command.GetNewCharacters:
                    {
                        SwGoh.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true, true);
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
                        //guild.Name = GuildDto.GetGuildNameFromAlias("41st");
                        //guild.ParseSwGoh();
                        //for (int i = 0; i < guild.PlayerNames.Count; i++)
                        //{

                        //    QueueMethods.AddPlayer(guild.PlayerNames[i], "up", 2);
                        //}
                        //QueueMethods.AddPlayer("41st", "ugnochars", 1);

                        //QueueMethods.AddPlayer("newholborn", "up",3);
                        //QueueMethods.AddPlayer("oaraug", "up", 3);
                        //QueueMethods.AddPlayer("tsitas_66", "up",1);
                        //QueueMethods.AddPlayer("tsitas_66", "up", 3);
                        //QueueMethods.AddPlayer("41st", "ugnochars", 3);
                        //for (int i = 0; i < 10; i++)
                        //{
                        //    QueueMethods.AddPlayer("tsitas_66", "up");
                        //}
                        break;
                    }
                default:
                    {
                        SwGoh
                        SwGoH.Log.ConsoleMessage("Unknown command , please try again.!!!!");
                        break;
                    }
            }
        }
    }
}
