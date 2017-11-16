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

namespace SWGoH
{
    class Program
    {
        private static bool isWorking = false;
        private static DateTime mLastProcess = DateTime.MinValue;
        
        static void Main(string[] args)
        {
            if (!Settings.Get()) return;

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

            //SWGoH.QueueMethods.AddPlayer("newholborn", Command.UpdatePlayer, 4, Enums.QueueEnum.QueueType.Player, DateTime.Now);
            //SWGoH.QueueMethods.AddPlayer("newholborn", Command.UpdatePlayer, 2, Enums.QueueEnum.QueueType.Player, DateTime.Now);
            //SWGoH.QueueMethods.AddPlayer("newholborn", Command.UpdatePlayer, 3, Enums.QueueEnum.QueueType.Player, DateTime.Now);
            
            //SWGoH.QueueMethods.AddPlayer("41st", Command.UpdateGuildWithNoChars , 4, Enums.QueueEnum.QueueType.Guild, DateTime.Now);
            //ExecuteCommand(Command.GetNewCharacters, "aramil"); return; 
            //ExecuteCommand(Command.UpdatePlayer, "newholborn");
            //ExecuteCommand(Command.Test, "newholborn");

            QueueDto q = QueueMethods.GetQueu();
            if (q != null)
            {
                int ret = ExecuteCommand(q.Command, q.Name);
                QueueMethods.RemoveFromQueu(q);
                if (ret != 3) mLastProcess = DateTime.Now;
            }
            else
            {
                int now = DateTime.Now.Minute;
                double minutes = 0.0;
                minutes = DateTime.Now.Subtract(mLastProcess).TotalMinutes;
                bool check = minutes > Settings.appSettings.MinutesUntilNextProcess;
                if (check)
                {
                    PlayerDto player = QueueMethods.GetLastUpdatedPlayer("41st");
                    if (player != null)
                    {
                        QueueMethods.AddPlayer(player.PlayerName, Command.UpdatePlayer, 1 , Enums.QueueEnum.QueueType.Player , DateTime.Now);
                    }
                }
                Console.WriteLine("Nothing to process");
            }
            isWorking = false;
            t.Change(Settings.appSettings.GlobalConsoleTimerInterval, Settings.appSettings.GlobalConsoleTimerInterval);
            GC.Collect();
        }
        private static int ExecuteCommand(Command commandstr, string pname)
        {
            ExportMethodEnum mExportMethod = ExportMethodEnum.Database;

            switch (commandstr)
            {
                case Command.UpdatePlayer:
                    {
                        SWGoH.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true,false);
                        if (ret == 1)
                        {
                            player.LastClassUpdated = DateTime.UtcNow;
                            player.Export(mExportMethod);
                        }
                        return ret;
                    }
                case Command.UpdateGuild:
                    {
                        SWGoH.GuildDto guild = new GuildDto
                        {
                            Name = GuildDto.GetGuildNameFromAlias(pname)
                        };
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateAllPlayers(mExportMethod, true);
                        break;
                    }
                case Command.UpdatePlayers:
                    {
                        string[] arg = pname.Split(',');
                        for (int i = 0; i < arg.Length; i++)
                        {
                            SWGoH.PlayerDto player = new PlayerDto(arg[i]);
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
                        SWGoH.GuildDto guild = new GuildDto
                        {
                            Name = GuildDto.GetGuildNameFromAlias(pname)
                        };
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateOnlyGuildWithNoChars(mExportMethod);
                        break;
                    }
                case Command.GetNewCharacters:
                    {
                        SWGoH.PlayerDto player = new PlayerDto(pname);
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

                        SWGoH.GuildDto guild = new GuildDto();
                        guild.Name = GuildDto.GetGuildNameFromAlias("41st");
                        guild.ParseSwGoh();
                        for (int i = 0; i < guild.PlayerNames.Count; i++)
                        {
                            QueueMethods.AddPlayer(guild.PlayerNames[i], Command.UpdatePlayer, 2, QueueType.Player,DateTime.Now );
                        }
                        QueueMethods.AddPlayer("41st", Command.UpdateGuildWithNoChars, 1, QueueType.Guild, DateTime.Now);

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
    }
}
