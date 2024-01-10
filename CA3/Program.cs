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
        const string BLACKJACK_LOGO = "______ _            _    _            _    \r\n| ___ \\ |          | |  (_)          | |   \r\n| |_/ / | __ _  ___| | ___  __ _  ___| | __\r\n| ___ \\ |/ _` |/ __| |/ / |/ _` |/ __| |/ /\r\n| |_/ / | (_| | (__|   <| | (_| | (__|   < \r\n\\____/|_|\\__,_|\\___|_|\\_\\ |\\__,_|\\___|_|\\_\\\r\n                       _/ |                \r\n                      |__/                 ";
        const string CARDS_GRAPHIC = ".------..------..------..------.\r\n|A.--. ||J.--. ||Q.--. ||K.--. |\r\n| :/\\: || :(): || (\\/) || :/\\: |\r\n| (__) || ()() || :\\/: || :\\/: |\r\n| '--'A|| '--'J|| '--'Q|| '--'K|\r\n`------'`------'`------'`------'";
        const string GAME_OVER = " ___   ___  __ __  ___       ___  _ _  ___  ___ \r\n/  _> | . ||  \\  \\| __>     | . || | || __>| . \\\r\n| <_/\\|   ||     || _>      | | || ' || _> |   /\r\n`____/|_|_||_|_|_||___>     `___'|__/ |___>|_\\_\\\r\n                                                ";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool stayInMenu = true;
            bool isFirstRound = true; 
            bool dealerBusts = false;
            bool playerBusts = false;
            bool keepAppRunning = true;

            Deck deck = new Deck();
            deck.ShuffleDeck();
            PlayerStatTracker playerStats = new PlayerStatTracker(0, 0, 0, 1000, 0, EndCondition.None, false);    

            List<Card> playersHand = new List<Card>();
            List<Card> dealersHand = new List<Card>();

            while (keepAppRunning)
            {
                while (stayInMenu)
                {
                    keepAppRunning = menu(deck, playerStats, ref stayInMenu);
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
                        gameOverScreen(playerStats);
                        break;
                    }
                    pause();
                    resetGame(deck, dealersHand, playersHand, playerStats, ref stayInMenu);
                }
            }

            Logger.LogWithColor("\n\n===========GAME CLOSED===========\n\n", ConsoleColor.Green);
            Logger.Log("Thanks For Playing!", 5);
            Console.ReadLine();
        }


    
        private static bool menu(Deck deck, PlayerStatTracker playerStats, ref bool stayInMenu)
        {
            Console.WriteLine(BLACKJACK_LOGO);
            Console.WriteLine(CARDS_GRAPHIC);
            Console.WriteLine("\nWould You Like To Play A Game Or View Score? Type y, n Or s");
            string command = Console.ReadLine().ToLower();

            while (command != "n" || command != "y" || command != "s")
            {
                if (command == "n")
                {
                    Console.Clear();
                    stayInMenu = false;
                    return false;
                }
                else if (command == "y")
                {
                    Console.Clear();
                    stayInMenu = false;
                    playerStats.GetPlayerBet();
                    return true;
                }
                else if (command == "s")
                {
                    showScore(playerStats);
                    stayInMenu = true;
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


        private static void showScore(PlayerStatTracker playerStats)
        {
            Console.Clear();
            Logger.LogWithColor("=====Score=====", ConsoleColor.DarkGreen, spacing: 5, makeNewLn: true);
            Logger.Log($"Funds: {playerStats.PlayerFunds}", 9);
            Logger.Log("Wins: " + playerStats.Wins, 9);
            Logger.Log("Loses: " + playerStats.Losses, 9);
            Logger.Log("Draws: " + playerStats.Draws, 9);
            Logger.LogWithColor("================\n", ConsoleColor.DarkGreen, spacing: 5);
            pause();
        }

        private static void gameOverScreen(PlayerStatTracker playerStats) 
        {
            for (int i = 0; i < 5; i++)
            {
                Logger.LogWithColor(GAME_OVER, ConsoleColor.Red);
                Thread.Sleep(750);

                Console.Clear(); 
                Thread.Sleep(250); 
            }

            showScore(playerStats);
        }

        private static void pause() 
        {
            Logger.Log("Press Any Key To Continue", 1, true);
            Console.ReadLine();
            Console.Clear();
        }

        private static void startRound(Deck deck, List<Card> dealersHand, List<Card> playersHand)
        {
            Logger.LogWithColor("----------First Draw----------", ConsoleColor.White, ConsoleColor.DarkGray);

            dealersHand.Add(deck.DrawCard());
            playersHand.Add(deck.DrawCard());
            dealersHand.Add(deck.DrawCard());
            playersHand.Add(deck.DrawCard());

            Logger.LogWithColor("Player's Hand", ConsoleColor.Green, spacing: 5, makeNewLn: true);
            showHand(playersHand, false);

            Logger.LogWithColor("Dealer's Hand", ConsoleColor.Magenta, spacing: 5, makeNewLn: true);
            showHand(dealersHand, true);
        }

        //player's turn logic
        private static bool playersTurn(Deck deck, List<Card> playersHand)
        {
            Logger.LogWithColor("----------Your Turn----------", ConsoleColor.White, ConsoleColor.Green, makeNewLn: true);

            while (true) 
            {
                Logger.LogWithColor("Your Hand", ConsoleColor.Green, spacing: 5, makeNewLn: true);
                showHand(playersHand, false);
                Console.WriteLine("\nStick Or Twist? Type s Or t");
                string stickTwist = Console.ReadLine().ToLower();

                if (stickTwist == "t")
                {
                    twist(deck, playersHand);
                    Console.Clear();

                    if (getHandValue(playersHand) > 21)
                    {
                        Logger.LogWithColor("================BUST================", ConsoleColor.White, ConsoleColor.Red, makeNewLn: true);
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
            Logger.LogWithColor("----------Dealer's Turn----------", ConsoleColor.White, ConsoleColor.Magenta, makeNewLn: true);
            if (isFirstRound)
            {
                isFirstRound = false;
            }

            while (true) 
            {
                Logger.LogWithColor("Dealer's Hand", ConsoleColor.Magenta, spacing: 5, makeNewLn: true);
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
                    Logger.LogWithColor("Dealer Has Stuck", ConsoleColor.Magenta, spacing: 5, makeNewLn: true);
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


        private static void showHandWithColor(List<Card> hand, string msg, ConsoleColor color)
        {
            Logger.LogWithColor("=========", color, spacing: 5, makeNewLn: true);
            Logger.LogWithColor($"{msg}", ConsoleColor.White, color, spacing: 5);
            Logger.LogWithColor("=========", color, spacing: 5);
            showHand(hand, false);
        }

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

        private static void resetGame(Deck deck, List<Card> dealersHand, List<Card> playersHand, PlayerStatTracker playerStats, ref bool isInMenu)
        {
            isInMenu = true;
            playerStats.CurrentEndCondition = EndCondition.None;
            playerStats.CurrentBet = 0;
            dealersHand.Clear();
            playersHand.Clear();
            deck.ResetDeck();
            deck.ShuffleDeck();
        }

        //get numeric value of hand
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
                else
                {
                    handValue += card.CardValue;
                }
            }

            for (int i = 0; i < aceCount; i++)
            {
                if (handValue + 11 <= 21)
                {
                    handValue += 11;
                }
                else
                {
                    handValue += 1;
                }
            }

            return handValue;
        }
    }
}