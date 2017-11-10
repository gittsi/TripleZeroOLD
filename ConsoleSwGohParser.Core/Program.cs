using System;
using System.Threading;

namespace SwGoh
{
    class Program
    {
        private static bool isWorking = false;
        //private static bool mPrintedNothingToProcess = false;
        //private static int mPrintedNothingToProcessdots = 0;
        //private static int mPrintedNothingToProcessdotsTotal = 4;
        private static int mTimerdelay = 5000;
        private static bool mExportLog = false;
        
        
        static void Main(string[] args)
        {
            if (mExportLog) SwGoh.Log.Initialize("log.txt" , mExportLog );

            Timer t = new Timer(new TimerCallback(TimerProc));
            t.Change(0, mTimerdelay);

            Console.ReadLine();

            if (mExportLog) SwGoh.Log.FileFinalize();
        }
        private static void TimerProc(Object o)
        {
            if (isWorking) return;
            isWorking = true;
            Timer t = o as Timer;
            t.Change(Timeout.Infinite, Timeout.Infinite);

            //SwGoh.QueueMethods.AddPlayer("newholborn", "up", 4, Enums.QueueEnum.QueueType.Player);
            //SwGoh.QueueMethods.AddPlayer("41st", "ug", 4, Enums.QueueEnum.QueueType.Guild);
            //ExecuteCommand("getnewchars", "aramil"); return; 

            Queue q = QueueMethods.GetQueu();
            if (q != null)
            {
                Console.WriteLine("");
                ExecuteCommand(q.Command, q.Name);
                QueueMethods.RemoveFromQueu(q);
                
                //mPrintedNothingToProcess = false;
                //mPrintedNothingToProcessdots = 0;
            }
            else
            {
                int now = DateTime.Now.Minute;
                if (now == 0 || now == 15 || now == 30 || now == 45)
                {
                    PlayerDto player = QueueMethods.GetLastUpdatedPlayer("41st");
                    if (player != null)
                    {
                        QueueMethods.AddPlayer(player.PlayerName, "up", 1 , Enums.QueueEnum.QueueType.Player);
                    }
                }

                string mMessage = "Nothing to process";
                //if (!mPrintedNothingToProcess) 
                Console.WriteLine(mMessage);
                //Console.Write("."); mPrintedNothingToProcessdots++;
                //if (mPrintedNothingToProcessdots == mPrintedNothingToProcessdotsTotal)
                //{
                //    mPrintedNothingToProcessdots = 0;
                //    for (int i=0;i< mPrintedNothingToProcessdotsTotal;i++)  Console.Write("\b \b");
                //}
                //mPrintedNothingToProcess = true;
            }
            isWorking = false;
            t.Change(mTimerdelay, mTimerdelay);
            GC.Collect();
        }
        private static void ExecuteCommand(SwGoh.Enums.Command commandstr, string pname)
        {
            ExportMethodEnum mExportMethod = ExportMethodEnum.Database;

            switch (commandstr)
            {
                case SwGoh.Enums.Command.UpdatePlayer:
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
                case SwGoh.Enums.Command.UpdateGuild:
                    {
                        SwGoh.GuildDto guild = new GuildDto();
                        guild.Name = GuildDto.GetGuildNameFromAlias(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateAllPlayers(mExportMethod, true);
                        break;
                    }
                case SwGoh.Enums.Command.UpdatePlayers:
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
                case SwGoh.Enums.Command.UpdateGuildWithNoChars:
                    {
                        SwGoh.GuildDto guild = new GuildDto();
                        guild.Name = GuildDto.GetGuildNameFromAlias(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames != null && guild.PlayerNames.Count > 0) guild.UpdateOnlyGuildWithNoChars(mExportMethod);
                        break;
                    }
                case SwGoh.Enums.Command.GetNewCharacters:
                    {
                        SwGoh.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true, true);
                        break;
                    }
                case SwGoh.Enums.Command.Help:
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
                case SwGoh.Enums.Command.Test:
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
                        
                        SwGoh.Log.ConsoleMessage("Unknown command , please try again.!!!!");
                        break;
                    }
            }
        }
    }
}
