﻿using System;
using System.IO;

namespace SWGoH
{
    public class Log
    {
        static StreamWriter stream = null;
        static bool mExportToFile = false;
        public static void Initialize(string Fname, bool exporttofile)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory + "Settings";
            string finalname = directory + "\\" + Fname;

            stream = new StreamWriter(finalname, false);
            mExportToFile = exporttofile;
            stream.AutoFlush = true;
        }
        public static void FileFinalize()
        {
            stream.Close();
        }
        public static void ConsoleMessage(string message)
        {
            if (mExportToFile) stream.WriteLine(message + "  Time:" + DateTime.UtcNow.TimeOfDay.ToString("h':'m':'s''"));
            Console.WriteLine(message + "  Time:" + DateTime.UtcNow.TimeOfDay.ToString("h':'m':'s''"));
        }
        public static void ConsoleMessageNotInFile(string message)
        {
            Console.WriteLine(message + "  Time:" + DateTime.UtcNow.TimeOfDay.ToString("h':'m':'s''"));
        }
    }
}
