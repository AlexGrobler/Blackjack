using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CA3.PlayerStatTracker;

namespace CA3
{
    internal class Program
    {
        //Used ASCII generator to create graphics
        const string BLACKJACK_LOGO = "" +
            "     ______ _            _    _            _    \r\n" +
            "     | ___ \\ |          | |  (_)          | |   \r\n" +
            "     | |_/ / | __ _  ___| | ___  __ _  ___| | __\r\n" +
            "     | ___ \\ |/ _` |/ __| |/ / |/ _` |/ __| |/ /\r\n" +
            "     | |_/ / | (_| | (__|   <| | (_| | (__|   < \r\n" +
            "     \\____/|_|\\__,_|\\___|_|\\_\\ |\\__,_|\\___|_|\\_\\\r\n" +
            "                            _/ |                \r\n" +
            "                           |__/                 ";
        const string CARDS_GRAPHIC = "" +
            "          .------..------..------..------.\r\n" +
            "          |A.--. ||J.--. ||Q.--. ||K.--. |\r\n" +
            "          | :/\\: || :(): || (\\/) || :/\\: |\r\n" +
            "          | (__) || ()() || :\\/: || :\\/: |\r\n" +
            "          | '--'A|| '--'J|| '--'Q|| '--'K|\r\n" +
            "          `------'`------'`------'`------'";
        const string GAME_OVER = "" +
            "      ___   ___  __ __  ___       ___  _ _  ___  ___ \r\n" +
            "     /  _> | . ||  \\  \\| __>     | . || | || __>| . \\\r\n" +
            "     | <_/\\|   ||     || _>      | | || ' || _> |   /\r\n" +
            "     `____/|_|_||_|_|_||___>     `___'|__/ |___>|_\\_\\\r\n                                                ";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool stayInMenu = true;
            bool isFirstRound = true; 
            bool dealerBusts = false;
            bool playerBusts = false;
            bool keepAppRunning = true;


            Deck deck = new Deck();
            PlayerStatTracker playerStats = new PlayerStatTracker(0, 0, 0, 1000, 0, EndCondition.None, false);    

            //used to hold cards as they are dealt
            List<Card> playersHand = new List<Card>();
            List<Card> dealersHand = new List<Card>();

            while (keepAppRunning)
            {
                while (stayInMenu)
                {
                    stayInMenu = menu(deck, playerStats, ref keepAppRunning);
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
                    playerStats.DetermineWinner(playerBusts, dealerBusts, getHandValue(playersHand), getHandValue(dealersHand));
                    pause();

                    if (playerStats.GameOver) 
                    {
                        playerStats.GameOverScreen(GAME_OVER);
                        break;
                    }
                    resetGame(deck, dealersHand, playersHand, playerStats, ref stayInMenu);
                }
            }

            playerStats.ShowStats();
            pause();

            Logger.LogWithColor("\n\n===========GAME CLOSED===========\n\n", ConsoleColor.Green);
            Logger.Log("Thanks For Playing!", 5);
            Console.ReadLine();
        }

        //main menu for app that will be returned to until the app is closed
        private static bool menu(Deck deck, PlayerStatTracker playerStats, ref bool keepAppRunning)
        {
            Logger.LogWithColor(BLACKJACK_LOGO, ConsoleColor.White);
            Logger.LogWithColor(CARDS_GRAPHIC, ConsoleColor.White);
            Console.WriteLine("\nWould You Like To Play A Game Or View Stats? Type y, n Or s");
            string command = Console.ReadLine().ToLower();

            while (command != "n" || command != "y" || command != "s")
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
                    playerStats.GetPlayerBet();
                    return false;
                }
                else if (command == "s")
                {
                    playerStats.ShowStats();
                    pause();
                    return true;
                }
                else
                {
                    Console.WriteLine("Please Type y, n Or s");
                    command = Console.ReadLine().ToLower();
                }
            }
            
            return true;
        }

        //pause app and wait for user input to continue
        private static void pause() 
        {
            Logger.Log("Press Any Key To Continue", 1, true);
            Console.ReadLine();
            Console.Clear();
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

                if (stickTwist == "t")
                {
                    twist(deck, playersHand);
                    Console.Clear();

                    if (getHandValue(playersHand) > 21)
                    {
                        Logger.LogWithColor("================BUST================", ConsoleColor.White, ConsoleColor.Red, newLn: true);
                        return true;
                    }
 
                }
                else if (stickTwist == "s")
                {
                    Console.Clear();
                    return false;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Please type s Or t");
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
                    Logger.LogWithColor("Dealer Has Busted!", ConsoleColor.White, ConsoleColor.Magenta, 5, true);
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
        private static void resetGame(Deck deck, List<Card> dealersHand, List<Card> playersHand, PlayerStatTracker playerStats, ref bool isInMenu)
        {
            isInMenu = true;
            playerStats.CurrentEndCondition = EndCondition.None;
            playerStats.CurrentBet = 0;
            dealersHand.Clear();
            playersHand.Clear();
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