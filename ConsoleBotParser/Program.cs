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
    }
    

    class Program
    {
        
        static void Main(string[] args)
        {
            //string pname = "41st";
            //Command command = Command.UpdateGuild;

            ExportMethodEnum mExportMethod  = ExportMethodEnum.Database;

            string pname = "tsitas_66";
            Command command = Command.UpdatePlayer;

            if (args.Length > 1)
            {
                string commandstr = args[0];
                if (args.Length > 2 && commandstr == "ups") command = Command.UpdatePlayers;
                else if (commandstr == "up") command = Command.UpdatePlayer;
                else if (commandstr == "ug") command = Command.UpdateGuild;
                else if (commandstr == "ugnochars") command = Command.UpdateGuildWithNoChars;
                pname = args[1];
            }

            switch (command )
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
                        SwGoh.GuildDto guild = new GuildDto(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames!=null && guild.PlayerNames.Count > 0) guild.UpdateAllPlayers(mExportMethod, true);
                        break;
                    }
                case Command.UpdatePlayers:
                    {
                        for(int i = 1; i < args.Length; i++)
                        {
                            SwGoh.PlayerDto player = new PlayerDto(args[i]);
                            int ret = player.ParseSwGoh(mExportMethod, true);
                            if (ret==1)
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
            }
            
        }
    }
}
