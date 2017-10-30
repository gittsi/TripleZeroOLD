using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSwGohParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string pname = "newholborn";

            if (args.Length != 0) pname = args[0];

            PlayerDto player = new PlayerDto(pname);
            //player.ParseSwGoh();
            //player.Export();
            player.Import();
            Environment.Exit(0);
        }
    }
}
