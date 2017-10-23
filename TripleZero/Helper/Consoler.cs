using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Helper
{
    public static class Consoler
    {
        public static void WriteLineInColor(string msg, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            catch (Exception) { }
        }
    }
}
