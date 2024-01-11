using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CA3.StatTracker;

namespace CA3
{
    internal class Program
    {
        //Used ASCII generator to create graphics
        const string BLACKJACK_LOGO = "\n" +
            "        ______ _            _    _            _    \r\n" +
            "        | ___ \\ |          | |  (_)          | |   \r\n" +
            "        | |_/ / | __ _  ___| | ___  __ _  ___| | __\r\n" +
            "        | ___ \\ |/ _` |/ __| |/ / |/ _` |/ __| |/ /\r\n" +
            "        | |_/ / | (_| | (__|   <| | (_| | (__|   < \r\n" +
            "        \\____/|_|\\__,_|\\___|_|\\_\\ |\\__,_|\\___|_|\\_\\\r\n" +
            "                               _/ |                \r\n" +
            "                              |__/                 ";
        const string CARDS_GRAPHIC = "" +
            "             .------..------..------..------.\r\n" +
            "             |A.--. ||J.--. ||Q.--. ||K.--. |\r\n" +
            "             | :/\\: || :(): || (\\/) || :/\\: |\r\n" +
            "             | (__) || ()() || :\\/: || :\\/: |\r\n" +
            "             | '--'A|| '--'J|| '--'Q|| '--'K|\r\n" +
            "             `------'`------'`------'`------'";
        const string GAME_OVER = "\n\n\n\n\n\n" +
            "                                   ___   ___  __ __  ___       ___  _ _  ___  ___ \r\n" +
            "                                  /  _> | . ||  \\  \\| __>     | . || | || __>| . \\\r\n" +
            "                                  | <_/\\|   ||     || _>      | | || ' || _> |   /\r\n" +
            "                                  `____/|_|_||_|_|_||___>     `___'|__/ |___>|_\\_\\\r\n";

        const string YOU_WIN = "\n\n\n\n\n\n" +
            "                                 __   __            _    _ _         _ \r\n" +
            "                                 \\ \\ / /           | |  | (_)       | |\r\n" +
            "                                  \\ V /___  _   _  | |  | |_ _ __   | |\r\n" +
            "                                   \\ // _ \\| | | | | |/\\| | | '_ \\  | |\r\n" +
            "                                   | | (_) | |_| | \\  /\\  / | | | | |_|\r\n" +
            "                                   \\_/\\___/ \\__,_|  \\/  \\/|_|_| |_| (_)\r\n";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool stayInMenu = true;
            bool isFirstRound = true; 
            bool dealerBusts = false;
            bool playerBusts = false;
            bool keepAppRunning = true;


            Deck deck = new Deck();
            StatTracker stats = new StatTracker(0, 0, 0, 1000);    

            //used to hold cards as they are dealt
            List<Card> playersHand = new List<Card>();
            List<Card> dealersHand = new List<Card>();

            while (keepAppRunning)
            {
                while (stayInMenu)
                {
                    stayInMenu = menu(deck, stats, ref keepAppRunning);
                }

                if (keepAppRunning)
                {
                    startRound(deck, dealersHand, playersHand);
                    pause();
                    playerBusts = playersTurn(deck, playersHand);
                    if (!playerBusts)
                    {
                        dealerBusts = dealersTurn(deck, dealersHand, playersHand, ref isFirstRound);
                        pause();
                    }

                    showHandWithColor(playersHand, "Your Hand", ConsoleColor.Green);
                    showHandWithColor(dealersHand, "Dealer's Hand", ConsoleColor.Magenta);
                    stats.DetermineWinner(playerBusts, dealerBusts, getHandValue(playersHand), getHandValue(dealersHand));
                    pause();
                    stats.ShowStats();
                    pause();

                    if (stats.GameOver || stats.GameWon)
                    {
                        if (isGameEnded(stats))
                        {
                            break;
                        }
                        else
                        {
                            resetGame(deck, dealersHand, playersHand, stats, true, ref stayInMenu);
                        }
                    }
                    else 
                    {
                        resetGame(deck, dealersHand, playersHand, stats, false, ref stayInMenu);
                    }
                }
            }

            Logger.LogWithColor("\n\n===========GAME ENDED===========\n\n", ConsoleColor.Green);
            Logger.Log("Thanks For Playing!", 5);
            Console.ReadLine();
        }

        private static void showLogo() 
        {
            Logger.LogWithColor(BLACKJACK_LOGO, ConsoleColor.White);
            Logger.LogWithColor(CARDS_GRAPHIC, ConsoleColor.White);
        }

        //main menu for app that will be returned to until the app is closed
        //allows player to play, quit, view stats or view tutorial
        private static bool menu(Deck deck, StatTracker stats, ref bool keepAppRunning)
        {
            showLogo();
            Logger.Log("Welcome To Blackjack!", 20, true);
            Logger.Log("-------------------------", 18);
            Logger.Log("Would You Like To Play A Game, View Stats, Or View The Tutorial?", newLn: true);
            Logger.Log("Type y To Play, n To Quit, s For Stats, Or t For Tutorial");
            string command = Console.ReadLine().ToLower();

            while (command != "n" || command != "y" || command != "s" || command != "t")
            {
                if (command == "n")
                {
                    Console.Clear();
                    keepAppRunning = false;
                    return false;
                }
                else if (command == "y")
                {
                    Console.Clear();
                    stats.GetPlayerBet();
                    return false;
                }
                else if (command == "s")
                {
                    stats.ShowStats();
                    pause();
                    return true;
                }
                else if (command == "t") 
                {
                    tutorial();
                    pause();
                    return true;
                }
                else
                {
                    Console.WriteLine("Please Type y, n, t Or s");
                    command = Console.ReadLine().ToLower();
                }
            }
            
            return true;
        }


        private static void tutorial() 
        {
            Console.Clear();
            Logger.Log("You bet money at the start of a round");
            Logger.Log("If you run out of funds, you lose!");
            Logger.Log("If the dealer runs out of funds, you win!");
            pause();
            Logger.Log("Your and the dealer's remaining funds are shown in the stats screen at the start of rounds");
            Logger.Log("Your goal is to get a hand with a value higher than the dealers");
            Logger.Log("A hand with a value over 21 means you bust, you lose that round!");
            Logger.Log("The card's suit does not matter!");
            pause();
            Logger.Log("Card Values", 20, true);
            Logger.Log("-------------------------", 15);
            Logger.Log("All numbered cards are worth that amount: ", 5, true);
            logCardInfoGraphic("5", "5");
            Logger.Log("Jack, Queen and King are worth 10: ", 5, true);
            logCardInfoGraphic("K", "10");
            Logger.Log("An Ace can be worth 1 or 11, whichever helps the player the most: ", 5, true);
            logCardInfoGraphic("A", "1/11");
        }

        //create card graphic with specified rank and value for the tutorial
        private static void logCardInfoGraphic(string rank, string value)
        {
            string card = "" +
            " .------.\r\n" +
            $" |{rank}.--. |\r\n" +
            $" | :/\\: |      = {value}\r\n" +
            " | (__) |\r\n" +
            $" | '--'{rank}|\r\n" +
            " `------'";

            Console.WriteLine(card);
            Console.Write("");
        }

        //pause app and wait for user input to continue
        private static void pause() 
        {
            Logger.Log("Press Any Key To Continue", 1, true);
            Console.ReadLine();
            Console.Clear();
        }

        //pause app and wait for user input to continue
        private static bool isGameEnded(StatTracker stats)
        {
            if (stats.GameOver)
            {
                Logger.DoTextAnimation(GAME_OVER, ConsoleColor.Red);
            }
            if (stats.GameWon)
            {
                Logger.DoTextAnimation(YOU_WIN, ConsoleColor.Green);
            }

            Logger.Log("Would You Like To Try Again? Type y Or n", 1, true);
            while (true)
            {
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    Console.Clear();
                    return false;
                }
                else if (answer == "n")
                {
                    Console.Clear();
                    return true;
                }
                else
                {
                    Logger.Log("Please Type y Or n", 1, true);
                }
            }
        }

        //suffle deck and deal first cards, hide dealer's 2nd card
        private static void startRound(Deck deck, List<Card> dealersHand, List<Card> playersHand)
        {
            Logger.LogWithColor("----------First Draw----------", ConsoleColor.White, ConsoleColor.DarkGray);
           
            deck.ShuffleDeck();
            dealersHand.Add(deck.DrawCard());
            playersHand.Add(deck.DrawCard());
            dealersHand.Add(deck.DrawCard());
            playersHand.Add(deck.DrawCard());

            Logger.LogWithColor("Player's Hand", ConsoleColor.Green, spacing: 5, newLn: true);
            showHand(playersHand, false);

            Logger.LogWithColor("Dealer's Hand", ConsoleColor.Magenta, spacing: 5, newLn: true);
            showHand(dealersHand, true);

        }

        //player's turn logic
        private static bool playersTurn(Deck deck, List<Card> playersHand)
        {
            Logger.LogWithColor("----------Your Turn----------", ConsoleColor.White, ConsoleColor.Green, newLn: true);

            while (true) 
            {
                Logger.LogWithColor("Your Hand", ConsoleColor.Green, spacing: 5, newLn: true);
                showHand(playersHand, false);
                Console.WriteLine("\nStick Or Twist? Type s Or t");
                string stickTwist = Console.ReadLine().ToLower();

                while (stickTwist != "t" || stickTwist !="s") 
                {
                    if (stickTwist == "t")
                    {
                        twist(deck, playersHand);
                        if (getHandValue(playersHand) > 21)
                        {
                            Logger.LogWithColor("================BUST================", ConsoleColor.White, ConsoleColor.Red, newLn: true);
                            pause();
                            return true;
                        }
                        Console.Clear();
                        break;

                    }
                    else if (stickTwist == "s")
                    {
                        Console.Clear();
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Please type s Or t");
                        stickTwist = Console.ReadLine().ToLower();
                    }
                }
            }
        }

        //dealer's turn logic
        private static bool dealersTurn(Deck deck, List<Card> dealersHand, List<Card> playersHand, ref bool isFirstRound) 
        {
            Logger.LogWithColor("----------Dealer's Turn----------", ConsoleColor.White, ConsoleColor.Magenta, newLn: true);
            if (isFirstRound)
            {
                isFirstRound = false;
            }

            while (true) 
            {
                Logger.LogWithColor("Dealer's Hand", ConsoleColor.Magenta, spacing: 5, newLn: true);
                showHand(dealersHand, false);
                int dealerHandValue = getHandValue(dealersHand);
                int playerHandValue = getHandValue(playersHand);
                bool dealerHasHigherValue = dealerHandValue > playerHandValue;

                if (dealerHandValue > 21)
                {
                    Logger.LogWithColor("Dealer Has Gone Bust!", ConsoleColor.White, ConsoleColor.Green, 5, true);
                    return true;
                }
                else if ((dealerHandValue < 16 && !dealerHasHigherValue))
                {
                    twist(deck, dealersHand);
                }
                else
                {
                    Logger.LogWithColor("Dealer Has Stuck", ConsoleColor.Magenta, spacing: 5, newLn: true);
                    return false;
                }
            }

        }

        //twist, draw card
        private static void twist(Deck deck, List<Card> hand)
        {
            Logger.Log("========", 8, true);
            Logger.LogWithColor("Twisting", ConsoleColor.White, ConsoleColor.DarkGray, 8);
            Logger.Log("========\n", 8);
            hand.Add(deck.DrawCard());
        }

        //iterate through list of cards, showing their rank and suit. Option to hide 2nd card for dealer if first round
        private static void showHand(List<Card> hand, bool isFirstDraw)
        {
            Logger.Log("-------------", 5);
            for (int i = 0; i < hand.Count; i++)
            {
                if (isFirstDraw && i == 1)
                {
                    Logger.Log("[Hidden Card]", 5);
                }
                else 
                {
                    Logger.Log(hand[i].ToString(), 5);
                }

            }
            Logger.Log("-------------", 5);
        }

        //call show hand but with extra decoration
        private static void showHandWithColor(List<Card> hand, string msg, ConsoleColor color)
        {
            Logger.LogWithColor("=========", color, spacing: 5, newLn: true);
            Logger.LogWithColor($"{msg}", ConsoleColor.White, color, spacing: 5);
            Logger.LogWithColor("=========", color, spacing: 5);
            showHand(hand, false);
        }

        //reset deck, hands and other relevant values
        private static void resetGame(Deck deck, List<Card> dealersHand, List<Card> playersHand, StatTracker stats, bool doFullReset, ref bool isInMenu)
        {
            isInMenu = true;
            dealersHand.Clear();
            playersHand.Clear();
            stats.ResetStats(doFullReset);
            deck.InitializeDeck();
        }

        //get numeric value of hand.
        //initially treat ace as having value of 11, then retroactively adjust the value of aces in hand till not bust
        private static int getHandValue(List<Card> hand)
        {
            int handValue = 0;
            int aceCount = 0;
            foreach (Card card in hand)
            {
                if (card.Rank == Card.CardRank.Ace)
                {
                    aceCount++;
                }
                handValue += card.CardValue;
            }

            //need to retroactively adjust value of aces till no aces or hand value is less than 21
            while (handValue > 21 && aceCount > 0)
            {
                handValue -= 10; 
                aceCount--;
            }

            return handValue;
        }
    }
}