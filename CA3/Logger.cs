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
        //Used ASCII generator to create graphics
        public const string BLACKJACK_LOGO = "\n" +
            "        ______ _            _    _            _    \r\n" +
            "        | ___ \\ |          | |  (_)          | |   \r\n" +
            "        | |_/ / | __ _  ___| | ___  __ _  ___| | __\r\n" +
            "        | ___ \\ |/ _` |/ __| |/ / |/ _` |/ __| |/ /\r\n" +
            "        | |_/ / | (_| | (__|   <| | (_| | (__|   < \r\n" +
            "        \\____/|_|\\__,_|\\___|_|\\_\\ |\\__,_|\\___|_|\\_\\\r\n" +
            "                               _/ |                \r\n" +
            "                              |__/                 ";
        public const string CARDS_GRAPHIC = "" +
            "             .------..------..------..------.\r\n" +
            "             |A.--. ||J.--. ||Q.--. ||K.--. |\r\n" +
            "             | :/\\: || :(): || (\\/) || :/\\: |\r\n" +
            "             | (__) || ()() || :\\/: || :\\/: |\r\n" +
            "             | '--'A|| '--'J|| '--'Q|| '--'K|\r\n" +
            "             `------'`------'`------'`------'";
        public const string GAME_OVER = "\n\n\n\n\n\n" +
            "                                   ___   ___  __ __  ___       ___  _ _  ___  ___ \r\n" +
            "                                  /  _> | . ||  \\  \\| __>     | . || | || __>| . \\\r\n" +
            "                                  | <_/\\|   ||     || _>      | | || ' || _> |   /\r\n" +
            "                                  `____/|_|_||_|_|_||___>     `___'|__/ |___>|_\\_\\\r\n";

        public const string YOU_WIN = "\n\n\n\n\n\n" +
            "                                 __   __            _    _ _         _ \r\n" +
            "                                 \\ \\ / /           | |  | (_)       | |\r\n" +
            "                                  \\ V /___  _   _  | |  | |_ _ __   | |\r\n" +
            "                                   \\ // _ \\| | | | | |/\\| | | '_ \\  | |\r\n" +
            "                                   | | (_) | |_| | \\  /\\  / | | | | |_|\r\n" +
            "                                   \\_/\\___/ \\__,_|  \\/  \\/|_|_| |_| (_)\r\n";

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

        //create card graphic with specified rank and value for the tutorial
        public static void LogCardInfoGraphic(string rank, string value)
        {
            string card = "" +
            " .------.\r\n" +
            $" |{rank}.--. |\r\n" +
            $" | :/\\: |      = {value}\r\n" +
            " | (__) |\r\n" +
            $" | '--'{rank}|\r\n" +
            " `------'";

            Logger.LogWithColor(card, ConsoleColor.White);
        }


    }
}
