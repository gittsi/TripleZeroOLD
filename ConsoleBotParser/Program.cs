﻿using System;
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            //string pname = "41st";
            //Command command = Command.UpdateGuild;

            string pname = "tsitas_66";
            Command command = Command.UpdatePlayer;

            if (args.Length == 2)
            {
                string commandstr = args[0];
                if (commandstr == "up") command = Command.UpdatePlayer;
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
                        if (guild.Players.Count > 0) guild.UpdateAllPlayers();
                        Environment.Exit(0);
                        break;
                    }
            }
            
        }
    }
}
