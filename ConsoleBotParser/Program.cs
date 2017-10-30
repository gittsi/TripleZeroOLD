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
    }

    class Program
    {
        static void Main(string[] args)
        {
            string pname = "newholborn";
            Command command = Command.UpdatePlayer;

            if (args.Length == 1)
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
                        PlayerDto player = new PlayerDto(pname);
                        bool ret = player.ParseSwGoh();
                        if (ret) player.Export();
                        Environment.Exit(0);
                        break;
                    }
                case Command.UpdateGuild:
                    {
                        
                        Environment.Exit(0);
                        break;
                    }
            }
            
        }
    }
}
