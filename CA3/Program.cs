using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    internal class Program
    {
        private enum EndCondition
        {
            None,
            Draw,
            PlayerWins,
            DealerWins
        }

        static void Main(string[] args)
        {
            (int PlayerWins, int DealerWins, int Draws) score = (0, 0, 0);

            bool isPlayingGame = false;
            bool isFirstRound = true; 
            bool dealerBusts = false;
            bool playerBusts = false;
            bool keepAppRunning = true;

            EndCondition endCond = EndCondition.None;

            Deck deck = new Deck();

            List<Card> playersHand = new List<Card>();
            List<Card> dealersHand = new List<Card>();
            while (keepAppRunning)
            {
                while (!isPlayingGame)
                {
                    keepAppRunning = menu(deck, score, ref isPlayingGame);
                }

                startRound(deck, dealersHand, playersHand);
                pause();
                playerBusts = playersTurn(deck, playersHand);
                if (!playerBusts)
                {
                    dealerBusts = dealersTurn(deck, dealersHand, playersHand, ref isFirstRound);
                    pause();
                }

                endCond = determineWinner(playerBusts, dealerBusts, dealersHand, playersHand);
                pause();
                updateScore(endCond, ref score);
                resetGame(deck, dealersHand, playersHand, ref endCond, ref isPlayingGame);
            }

            Console.WriteLine("\n\n======================");
            Console.WriteLine("==APP CLOSED==");
            Console.ReadLine();
        }

        private static bool menu(Deck deck, (int, int, int) score, ref bool playGame)
        {
            string cardsGraphic = ".------..------..------..------.\r\n|A.--. ||J.--. ||K.--. ||Q.--. |\r\n| (\\/) || :(): || :/\\: || (\\/) |\r\n| :\\/: || ()() || :\\/: || :\\/: |\r\n| '--'A|| '--'J|| '--'K|| '--'Q|\r\n`------'`------'`------'`------'";
            string blackJackLogo = "______ _            _    _            _    \r\n| ___ \\ |          | |  (_)          | |   \r\n| |_/ / | __ _  ___| | ___  __ _  ___| | __\r\n| ___ \\ |/ _` |/ __| |/ / |/ _` |/ __| |/ /\r\n| |_/ / | (_| | (__|   <| | (_| | (__|   < \r\n\\____/|_|\\__,_|\\___|_|\\_\\ |\\__,_|\\___|_|\\_\\\r\n                       _/ |                \r\n                      |__/                 ";
            Console.WriteLine(blackJackLogo);
            Console.WriteLine(cardsGraphic);
            Console.WriteLine("\nWould You Like To Play A Game Or View Score? Type y, n Or score");
            string command = Console.ReadLine().ToLower();
            if (command == "n")
            {
                return false;
            }
            else if (command == "y")
            {
                playGame = true;
            }
            else if (command == "score") 
            {
                showScore(score);
                playGame = false;
            }
            else
            {
                Console.WriteLine("Please type yes, no or score");
            }

            Console.Clear();
            return true;
        }

        private static void showScore((int, int, int) score)
        {
            Console.Clear();
            Logger.Log("=====Score=====", 5, true);
            Logger.Log("Wins: " + score.Item1, 9);
            Logger.Log("Loses: " + score.Item2, 9);
            Logger.Log("Draws: " + score.Item3, 9);
            Logger.Log("================\n", 5);
            pause();
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
                        showHand(playersHand, false);
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
                    Console.WriteLine("Please type s or t");
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
                    Console.WriteLine("\nDealer Has Busted");
                    return true;
                }
                else if ((dealerHandValue < 16 && !dealerHasHigherValue))
                {
                    twist(deck, dealersHand);
                }
                else
                {
                    Logger.LogWithColor("\nDealer Has Stuck", ConsoleColor.Magenta);
                    return false;
                }
            }

        }

        //twist, draw card
        private static void twist(Deck deck, List<Card> hand)
        {
            Console.WriteLine("\n========");
            Logger.LogWithColor("Twisting", ConsoleColor.White, ConsoleColor.DarkGray);
            Console.WriteLine("========\n");
            hand.Add(deck.DrawCard());
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
                    handValue += card.GetCardValue(handValue);
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

            if (handValue > 21)
            {
                Logger.LogWithColor("================BUST================", ConsoleColor.White, ConsoleColor.Red, makeNewLn: true);
            }

            return handValue;
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


        private static EndCondition compareHands(List<Card> playersHand, List<Card> dealersHand) 
        {
            int playerHandValue = compareHandHelper(playersHand, "Your Hand", ConsoleColor.Green);
            int dealerHandValue = compareHandHelper(dealersHand, "Dealer's Hand", ConsoleColor.Magenta);

            if (playerHandValue == dealerHandValue)
            {
               return declareDraw();
            }
            else if (playerHandValue > dealerHandValue)
            {
                return declareWin();
            }
            else 
            {
                return declareLoss();
            }
        }

        private static int compareHandHelper(List<Card> hand, string msg, ConsoleColor color) 
        {
            Logger.LogWithColor("=========", color, spacing: 5, makeNewLn: true);
            Logger.LogWithColor($"{msg}", ConsoleColor.White, color, spacing: 5);
            Logger.LogWithColor("=========", color, spacing: 5);
            showHand(hand, false);
            return getHandValue(hand);
        }

        private static EndCondition determineWinner(bool playerBusts, bool dealerBusts, List<Card> dealersHand, List<Card> playersHand)
        {
            if (!playerBusts && !dealerBusts)
            {
                return compareHands(playersHand, dealersHand);
            }
            else if (playerBusts)
            {
                return declareLoss();
            }
            else if (dealerBusts)
            {
                return declareWin();
            }
            else if (dealerBusts && playerBusts) 
            {
                return declareDraw();
            }
            return declareNoOutcome();
        }

        private static EndCondition declareWin() 
        {
            Logger.LogWithColor("Player Wins!", ConsoleColor.White, ConsoleColor.Green, 5, true);
            return EndCondition.PlayerWins;
        }

        private static EndCondition declareLoss()
        {
            Logger.LogWithColor("House Wins!", ConsoleColor.White, ConsoleColor.Magenta, 5, true);
            return EndCondition.DealerWins;
        }

        private static EndCondition declareDraw()
        {
            Logger.LogWithColor("Draw!", ConsoleColor.White, ConsoleColor.DarkGray, 5, true);
            return EndCondition.Draw;
        }

        private static EndCondition declareNoOutcome()
        {
            Logger.LogWithColor("Could Not Determine Outcome".PadLeft(10), ConsoleColor.White, ConsoleColor.Red, 5, true);
            return EndCondition.None;
        }

        private static void updateScore(EndCondition endCond, ref (int, int, int) score) 
        {
            if (endCond == EndCondition.Draw)
            {
                score.Item3++;
            }
            if (endCond == EndCondition.PlayerWins) 
            {
                score.Item1++;
            }
            if (endCond == EndCondition.DealerWins)
            {
                score.Item2++;
            }
        }

        private static void resetGame(Deck deck, List<Card> dealersHand, List<Card> playersHand, ref EndCondition endCond, ref bool playGame)
        {
            playGame = false;
            endCond = EndCondition.None;
            dealersHand.Clear();
            playersHand.Clear();
            deck.ResetDeck();
            deck.ShuffleDeck();
        }
    }
}

//to do to make game playable:
//initial round
//player's turn, they may keep taking cards till they bust or decide to stick
//once they stick, reveal dealer's hidden card
//now it is dealer's turn
//they keep taking cards until they bust, or twist/hit on 16 or stand/stick on 17
//if dealer has stuck, hand values are compared
//if player chooses to not draw card, and dealer choose to not draw card, and neither busted, hand values are compared

//might be best to have a class for player's score and money

//settings:
//beginner vs. expert mode where beginner prints card values and shows hints if you should stick or twist
//let user choose color, regardless change color of different logged elements, maybe color match suits too
//can do an override of writeline that takes color as an argument to fascilitate this
//setting for player and dealer color
//wildcard mode where dealer can keep playing past 16 hand value if player has higher hand value (playerHandValue > dealerHandValue && dealerHandValue < 21)


//have option to view score, games played, games won, games lost, win-lose ratio
//allow user to view revealed cards and hand after every draw or simpl write it every time.

//display user score
//prompt user for playing a game
//clear console
//select 2 card objects from shuffled list
//display card value
//ask user if they want to receive another card or stop (stick or twist)

//if stick loop over adding another card then ask if they want to s/t

//every time player receives another card, check if value is over 21 (they lose if so)

//if the player hasn't lost, and choose stick, then deal dealer's cards
//dealer must keep drawing until they get at least 17 (dealer only stops once score is 17 or higher or bust)

//compare player's card value vs. dealer's and determine winner

//record win/loss/draw and add to user score

//prompt user for a new game or to quit