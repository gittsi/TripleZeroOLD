using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        static void Main(string[] args)
        {
            

            //SwGoh.CharactersConfig.ExportCharacterFilesToDB();

            ExportMethodEnum mExportMethod = ExportMethodEnum.Database;

            string pname = "newholborn";
            Command command = Command.UpdatePlayer;
            //string pname = "41st";
            //Command command = Command.UpdateGuildWithNoChars;


            if (args.Length > 0)
            {
                string commandstr = args[0];
                if (args.Length > 2 && commandstr == "ups") command = Command.UpdatePlayers;
                else if (commandstr == "up") command = Command.UpdatePlayer;
                else if (commandstr == "ug") command = Command.UpdateGuild;
                else if (commandstr == "ugnochars") command = Command.UpdateGuildWithNoChars;
                else if (commandstr == "help") command = Command.Help;
                else if (commandstr == "test") command = Command.Test;
                else command = Command.UnKnown;
                if (args.Length > 1) pname = args[1];
            }

            switch (command)
            {
                case Command.UpdatePlayer:
                    {
                        SwGoh.PlayerDto player = new PlayerDto(pname);
                        int ret = player.ParseSwGoh(mExportMethod, true);
                        if (ret == 1) player.Export(mExportMethod);
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
                        for (int i = 1; i < args.Length; i++)
                        {
                            SwGoh.PlayerDto player = new PlayerDto(args[i]);
                            int ret = player.ParseSwGoh(mExportMethod, true);
                            if (ret == 1)
                            {
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
                        SwGoh.CharactersConfig.ExportCharacterFilesToDB();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown command , please try again.!!!!");
                        break;
                    }
            }
            Console.WriteLine("");
            Console.WriteLine("Press Enter to close!!!!");
            Console.Read();
        }
    }
}
