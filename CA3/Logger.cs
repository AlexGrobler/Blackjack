using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    public static class Logger
    {
        public static void LogWithColor(string msg, ConsoleColor textColor, ConsoleColor backgroundColor = ConsoleColor.Black, int spacing = 0, bool makeNewLn = false)
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;

            Log(msg, spacing, makeNewLn);     

            Console.ResetColor();
        }

        public static void Log(string msg, int spacing = 0, bool makeNewLn = false) 
        {
            string str = spacing > 0 ? new String(' ', spacing) + msg : msg;
            str = makeNewLn ? "\n" + str : str;
            Console.WriteLine(str);
        }
    }
}
