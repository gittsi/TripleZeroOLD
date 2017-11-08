using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Core.Net
{
    public static class Log
    {
        public static void ConsoleMessage(string message)
        {
            Console.WriteLine(message + "  Time:" + DateTime.Now.TimeOfDay.ToString("h':'m':'s''"));
        }
    }
}
