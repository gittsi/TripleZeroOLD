using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using SWGoH.Enums.QueueEnum;
using System.Runtime.InteropServices;

namespace SWGoH
{
    class Program
    {
        private static bool isWorking = false;
        private static DateTime mLastProcess = DateTime.MinValue;
        private static QueueDto workingQ = null;

        static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            if (!Settings.Get()) return;

            SWGoH.MongoDBRepo.SetWorking(true);

            if (SWGoH.Settings.appSettings.LogToFile == 1) SWGoH.Log.Initialize("log.txt" , SWGoH.Settings.appSettings.LogToFile == 1);

            Timer t = new Timer(new TimerCallback(TimerProc));
            t.Change(0, Settings.appSettings.GlobalConsoleTimerInterval);

            Console.ReadLine();

            if (SWGoH.Settings.appSettings.LogToFile == 1) SWGoH.Log.FileFinalize();
        }
        private static void TimerProc(Object o)
        {
            if (isWorking) return;
            isWorking = true;
            Timer t = o as Timer;
            t.Change(Timeout.Infinite, Timeout.Infinite);

            //SWGoH.QueueMethods.AddPlayer("newholborn", Command.UpdatePlayer, 1, Enums.QueueEnum.QueueType.Player, DateTime.UtcNow);
            //SWGoH.QueueMethods.AddPlayer("tsitas_66", Command.UpdatePlayer, 4, Enums.QueueEnum.QueueType.Player, DateTime.UtcNow.AddHours (15.0));
            //SWGoH.QueueMethods.AddPlayer("tsitas", Command.UpdatePlayer, 1, Enums.QueueEnum.QueueType.Player, DateTime.UtcNow);
            //SWGoH.QueueMethods.AddPlayer("Roukoun", Command.UpdatePlayer, 5, Enums.QueueEnum.QueueType.Player, DateTime.UtcNow);
            //SWGoH.QueueMethods.AddPlayer("aramil", Command.GetNewCharacters,  PriorityEnum.ManualLoad , Enums.QueueEnum.QueueType.Player, DateTime.UtcNow);

            //SWGoH.QueueMethods.AddPlayer("newholborn", Command.UpdatePlayer, 3, Enums.QueueEnum.QueueType.Player, DateTime.UtcNow);

            //SWGoH.QueueMethods.AddPlayer("41st", Command.UpdateGuildWithNoChars , PriorityEnum.DailyUpdate, Enums.QueueEnum.QueueType.Guild, DateTime.UtcNow);
            //ExecuteCommand(Command.GetNewCharacters, "aramil"); return; 
            //ExecuteCommand(Command.UpdatePlayer, "oaraug", null);
            //ExecuteCommand(Command.Test, "newholborn", null);
            //ExecuteCommand(Command.UpdateGuildWithNoChars, "41st", null);return;
            //ExecuteCommand(Command.UpdateUnknownGuild, "122#@#the-allidnce-pi", null); return;

            int now = DateTime.UtcNow.Minute;
            double minutes = 0.0;
            minutes = DateTime.UtcNow.Subtract(mLastProcess).TotalMinutes;
            bool check = minutes > Settings.appSettings.MinutesUntilNextProcess;
            if (check)
            {
                workingQ = QueueMethods.GetQueu();
                if (workingQ != null)
                {
                    int ret = ExecuteCommand(workingQ.Command, workingQ.Name, workingQ);
                    if (ret == 1 || ret == 5) mLastProcess = DateTime.UtcNow;
                    workingQ = null;
                }
                else
                {
                    //int now = DateTime.UtcNow.Minute;
                    //double minutes = 0.0;
                    //minutes = DateTime.UtcNow.Subtract(mLastProcess).TotalMinutes;
                    //bool check = minutes > Settings.appSettings.MinutesUntilNextProcess;
                    //if (check)
                    //{
                    //    PlayerDto player = QueueMethods.GetLastUpdatedPlayer("41st");
                    //    if (player != null)
                    //    {
                    //        QueueMethods.AddPlayer(player.PlayerName, Command.UpdatePlayer, 1 , Enums.QueueEnum.QueueType.Player , DateTime.UtcNow);
                    //    }
                    //}
                    Console.WriteLine("Nothing to process");
                }
            }
            else
            {
                Console.WriteLine("Waiting...  " + ((int)(Settings.appSettings.MinutesUntilNextProcess - minutes)).ToString () + " minutes");
            }
            isWorking = false;
            t.Change(Settings.appSettings.GlobalConsoleTimerInterval, Settings.appSettings.GlobalConsoleTimerInterval);
            GC.Collect();
        }
        private static int ExecuteCommand(Command commandstr, string pname, QueueDto q)
        {
            ExportMethodEnum mExportMethod = ExportMethodEnum.Database;

            switch (commandstr)
            {
                case Command.UpdatePlayer:
                    {
                        SWGoH.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true,false);
                        if (SWGoH.PlayerDto.isOnExit) return -1;
                        if (ret == 0 || (q != null && q.Priority == PriorityEnum.ManualLoad))
                        {
                            QueueMethods.RemoveFromQueu(q);
                        }
                        else if (ret == 1 || ret == 2)
                        {
                            player.LastClassUpdated = DateTime.UtcNow;
                            if (ret == 1)
                            {
                                player.Export(mExportMethod);
                                player.DeletePlayerFromDBAsync();
                                if (q != null) QueueMethods.UpdateQueueAndProcessLater(q, player, 24.2, false);
                            }
                            else if (ret == 2)
                            {
                                if (q != null) QueueMethods.UpdateQueueAndProcessLater(q, player, 0.5, true);
                            }

                        }
                        return ret;
                    }
                case Command.UpdateUnknownGuild:
                    {
                        string command = pname;
                        string[] opponent = command.Split("#@#");
                        if (opponent.Length > 0)
                        {
                            try
                            {
                                string IDstr = opponent[0];
                                string guildname = opponent[1];
                                int guildID = int.Parse(IDstr);
                                string guildURL = "/" + IDstr + "/" + guildname + "/";
                                bool ret = SWGoH.GuildConfigDto.AddGuildToConfig(guildname, guildID,guildURL );
                                if (ret)
                                {
                                    ExecuteCommand(Command.UpdateGuild, guildname, null);
                                }
                            }
                            catch (Exception e)
                            {
                                SWGoH.Log.ConsoleMessage(pname + " ERROR : " + e.Message);
                            }
                        }
                        break;
                    }
                case Command.UpdateGuild:
                    {
                        SWGoH.GuildDto guild = new GuildDto();
                        guild.Name = GuildDto.GetGuildNameFromAlias(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames!= null && guild.PlayerNames.Count> 0)
                        for (int i = 0; i < guild.PlayerNames.Count; i++)
                        {
                            QueueMethods.AddPlayer(guild.PlayerNames[i], Command.UpdatePlayer, PriorityEnum.ManualLoad, QueueType.Player, DateTime.UtcNow);
                        }
                        QueueMethods.AddPlayer(pname, Command.UpdateGuildWithNoChars, PriorityEnum.ManualLoad, QueueType.Guild, DateTime.UtcNow);
                        if (q != null) QueueMethods.RemoveFromQueu(q);
                        break;
                    }
                case Command.UpdatePlayers:
                    {
                        string[] arg = pname.Split(',');
                        for (int i = 0; i < arg.Length; i++)
                        {
                            ExecuteCommand(Command.UpdatePlayer, arg[i], null);
                        }
                        break;
                    }
                case Command.UpdateGuildWithNoChars:
                    {
                        SWGoH.GuildDto guild = new GuildDto{Name = GuildDto.GetGuildNameFromAlias(pname)};
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0)
                        {
                            guild.UpdateOnlyGuildWithNoChars(mExportMethod);
                            guild.CheckForNewPlayers();

                            if (q != null && q.Priority == PriorityEnum.ManualLoad)
                            {
                                QueueMethods.RemoveFromQueu(q);
                            }
                            else
                            {
                                if (q!=null) QueueMethods.UpdateQueueAndProcessLater(q, guild, 24.1, false);
                            }
                        }
                        break;
                    }
                case Command.GetNewCharacters:
                    {
                        SWGoH.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true, true);
                        if (q!= null) QueueMethods.RemoveFromQueu(q);
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

                        //SWGoH.GuildDto guild = new GuildDto();
                        //guild.Name = GuildDto.GetGuildNameFromAlias("41st");
                        //guild.ParseSwGoh();
                        //for (int i = 0; i < guild.PlayerNames.Count; i++)
                        //{
                        //    QueueMethods.AddPlayer(guild.PlayerNames[i], Command.UpdatePlayer, PriorityEnum.DailyUpdate, QueueType.Player,DateTime.UtcNow );
                        //}
                        //QueueMethods.AddPlayer("41st", Command.UpdateGuildWithNoChars, PriorityEnum.DailyUpdate, QueueType.Guild, DateTime.UtcNow);

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
                        
                        SWGoH.Log.ConsoleMessage("Unknown command , please try again.!!!!");
                        break;
                    }
            }
            return commandstr.GetHashCode();
        }


        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    {
                        PlayerDto.isOnExit = true;
                        isWorking = true;
                        SWGoH.MongoDBRepo.SetWorking(false);
                        if (workingQ != null)
                        {
                            QueueMethods.UpdateQueueAndProcessLater(workingQ, null , 0.5, true);
                        }
                        Thread.Sleep(5000);
                        return false;
                    }
                default:
                    return false;
            }
        }

    }
}
