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
        UpdatePlayers = 3,
    }

    class Program
    {
        static void Main(string[] args)
        {
            //string pname = "41st";
            //Command command = Command.UpdateGuild;

            string pname = "briglja";
            Command command = Command.UpdatePlayer;

            if (args.Length > 1)
            {
                string commandstr = args[0];
                if (args.Length > 2 && commandstr == "ups") command = Command.UpdatePlayers;
                else if (commandstr == "up") command = Command.UpdatePlayer;
                else if (commandstr == "ug") command = Command.UpdateGuild;
                pname = args[1];
            }

            switch (command )
            {
                case Command.UpdatePlayer:
                    {
                        SwGoh.PlayerDto player = new PlayerDto(pname);
                        bool ret = player.ParseSwGoh();
                        if (ret) player.Export();
                        Environment.Exit(0);
                        break;
                    }
                case Command.UpdateGuild:
                    {
                        SwGoh.GuildDto guild = new GuildDto(pname);
                        guild.ParseSwGoh();
                        if (guild.PlayerNames.Count > 0) guild.UpdateAllPlayers();
                        Environment.Exit(0);
                        break;
                    }
                case Command.UpdatePlayers:
                    {
                        for(int i = 1; i < args.Length; i++)
                        {
                            SwGoh.PlayerDto player = new PlayerDto(args[i]);
                            bool ret = player.ParseSwGoh();
                            if (ret)
                            {
                                player.Export();
                            }
                        }
                        break;
                    }
            }
            
        }
    }
}
