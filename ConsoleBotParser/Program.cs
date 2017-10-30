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

            PlayerDto newholborn = new PlayerDto(pname);
            newholborn.ParseSwGoh();
            newholborn.Export();
            Environment.Exit(0);
        }
    }
}
