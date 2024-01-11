using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CA3
{

    //easier way to keep console writing more consistent and reduce repitition
    public static class Logger
    {
        public static void LogWithColor(string msg, ConsoleColor textColor, ConsoleColor backgroundColor = ConsoleColor.Black, int spacing = 0, bool newLn = false)
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;

            Log(msg, spacing, newLn);     

            Console.ResetColor();
        }

        public static void Log(string msg, int spacing = 0, bool newLn = false) 
        {
            string str = spacing > 0 ? new String(' ', spacing) + msg : msg;
            str = newLn ? "\n" + str : str;
            Console.WriteLine(str);
        }

        //pseudo-animate game over graphic if the player loses or wins the game (dealer or player runs out of money)
        //uses Thread.Sleep() method to pause code execution to create blinking effect
        public static void DoTextAnimation(string graphic, ConsoleColor color, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            for (int i = 0; i < 5; i++)
            {
                Logger.LogWithColor(graphic, color, backgroundColor);
                Thread.Sleep(700);

                Console.Clear();
                Thread.Sleep(250);
            }
        }

    }
}
