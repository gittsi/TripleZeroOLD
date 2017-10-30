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
            PlayerDto newholborn = new PlayerDto("newholborn");
            newholborn.ParseSwGoh();
        }
    }
}
