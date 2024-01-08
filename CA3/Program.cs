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
            bool playGame = false;
            bool isFirstRound = true; //change to int and keep track of rounds

            bool dealerBusts = false;
            bool playerBusts = false;

            bool keepAppRunning = true;

            EndCondition endCond = EndCondition.None;

            Deck deck = new Deck();

            List<Card> playersHand = new List<Card>();
            List<Card> dealersHand = new List<Card>();
            while (keepAppRunning)
            {
                while (!playGame)
                {
                    keepAppRunning = Menu(deck, score, ref playGame);
                }

                StartRound(deck, dealersHand, playersHand);
                playerBusts = PlayersTurn(deck, playersHand);
                if (!playerBusts)
                {
                    dealerBusts = DealersTurn(deck, dealersHand, playersHand, ref isFirstRound);
                }

                endCond = DetermineWinner(playerBusts, dealerBusts, dealersHand, playersHand);
                UpdateScore(endCond, ref score);
                ResetGame(deck, dealersHand, playersHand, ref endCond, ref playGame);
                Console.WriteLine("\n\n==GAME OVER==");
            }

            Console.WriteLine("\n\n======================");
            Console.WriteLine("==APP CLOSED==");
            Console.ReadLine();
        }

        private static EndCondition DetermineWinner(bool playerBusts, bool dealerBusts, List<Card> dealersHand, List<Card> playersHand)
        {
            if (!playerBusts && !dealerBusts)
            {
                Console.WriteLine("DRAW!");
                return CompareHands(playersHand, dealersHand);
            }
            else if (playerBusts)
            {
                LogWithColor("House Wins!", ConsoleColor.Magenta);
                return EndCondition.DealerWins;
            }
            else if (dealerBusts)
            {
                LogWithColor("Player Wins!", ConsoleColor.Green);
                return EndCondition.PlayerWins;
            }
            LogWithColor("Could Not Determine Outcome", ConsoleColor.Red);   
            return EndCondition.None;
        }


        private static void LogWithColor(string msg, ConsoleColor color) 
        {
            Console.ForegroundColor = color;    
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private static void ResetGame(Deck deck, List<Card> dealersHand, List<Card> playersHand, ref EndCondition endCond, ref bool playGame)
        {
            playGame = false;
            endCond = EndCondition.None;
            dealersHand.Clear();
            playersHand.Clear();
            deck.ResetDeck();
            deck.ShuffleDeck();
        }

        private static void ShowScore((int, int, int) score) 
        {
            Console.WriteLine("\n\n=====Score=====");
            Console.WriteLine("Wins: {0}", score.Item1);
            Console.WriteLine("Loses: {0}", score.Item2);
            Console.WriteLine("Draws: {0}", score.Item3);
            Console.WriteLine("==============\n\n");
        }

        private static bool Menu(Deck deck, (int, int, int) score, ref bool playGame)
        {
            Console.WriteLine("Would You Like To Play A Game Or View Score? type yes, no or score");
            string command = Console.ReadLine().ToLower();
            if (command == "no")
            {
                return false;
            }
            else if (command == "yes")
            {
                playGame = true;
            }
            else if (command == "score") 
            {
                ShowScore(score);
                playGame = false;
            }
            else
            {
                Console.WriteLine("Please type yes, no or score");
            }

            return true;
        }

        //ideally should alternate between player and dealer when dealing
        //should only print hands after the fact
        //and then not show dealer's second card
        private static void StartRound(Deck deck, List<Card> dealersHand, List<Card> playersHand)
        {
            Console.WriteLine("----------First Draw----------");

            Console.WriteLine("\n\nThe Dealer Is Dealing For Themself");
            Console.WriteLine("======================");
            dealersHand.Add(deck.DrawCard(true));
            dealersHand.Add(deck.DrawCard(false));

            Console.WriteLine("\nThe Dealer Is Giving You Two Cards");
            Console.WriteLine("======================");
            playersHand.Add(deck.DrawCard(true));
            playersHand.Add(deck.DrawCard(true));
        }

        private static void Twist(Deck deck, List<Card> hand)
        {
            Console.WriteLine("\n\nTwisting");
            Console.WriteLine("=========\n\n");
            hand.Add(deck.DrawCard(true));
        }

        private static bool PlayersTurn(Deck deck, List<Card> playersHand)
        {
            Console.WriteLine("\n\nPlayer's Turn");
            Console.WriteLine("================\n");

            while (true) 
            {
                ShowHand(playersHand);
                Console.WriteLine("Stick Or Twist?");
                string stickTwist = Console.ReadLine().ToLower();

                if (stickTwist == "t")
                {
                    Twist(deck, playersHand);

                    if (GetHandValue(playersHand) > 21) 
                    {
                        return true;
                    }

                }
                else if (stickTwist == "s") 
                {
                    return false;
                }
                else 
                {
                    Console.WriteLine("Please type s or t");
                }
            }
        }

        private static bool DealersTurn(Deck deck, List<Card> dealersHand, List<Card> playersHand, ref bool isFirstRound) 
        {
            Console.WriteLine("\n\nDealer's Turn");
            Console.WriteLine("D================\n");
            if (isFirstRound)
            {
                Console.WriteLine("\n\nRevealing dealers second card");
                Console.WriteLine("Dealer Has {0} of {1}", dealersHand[1].Rank, dealersHand[1].Suit);
                isFirstRound = false;
            }

            while (true) 
            {
                ShowHand(dealersHand);
                int dealerHandValue = GetHandValue(dealersHand);
                int playerHandValue = GetHandValue(playersHand);
                bool dealerHasHigherValue = dealerHandValue > playerHandValue;

                if (dealerHandValue > 21)
                {
                    Console.WriteLine("Dealer Has Busted");
                    return true;
                }
                else if (dealerHandValue <= 16 && !dealerHasHigherValue)
                {
                    Twist(deck, dealersHand);
                }
                else
                {
                    Console.WriteLine("Dealer Has Stuck");
                    return false;
                }
            }

        }

        private static int GetHandValue(List<Card> hand)
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

            Console.WriteLine("Hand value: {0}", handValue);
            if (handValue > 21)
            {
                Console.WriteLine("================BUST================");
            }

            return handValue;
        }

        private static void ShowHand(List<Card> hand)
        {
            Console.WriteLine("================================");
            foreach (Card card in hand)
            {
                Console.WriteLine("{0} of {1}", card.Rank, card.Suit);
            }
            Console.WriteLine("================================");
        }

        private static EndCondition CompareHands(List<Card> playersHand, List<Card> dealersHand) 
        {
            int playerHandValue = 0;
            int dealerHandValue = 0;

            Console.WriteLine("\n\nYour Hand");
            Console.WriteLine("======================");
            ShowHand(playersHand);
            foreach (Card card in playersHand)
            {
                playerHandValue += card.GetCardValue(playerHandValue);
            }
            Console.WriteLine("Value = " + playerHandValue);

            Console.WriteLine("\n\nDealer's Hand");
            Console.WriteLine("======================");
            ShowHand(dealersHand);
            foreach (Card card in dealersHand)
            {
                dealerHandValue += card.GetCardValue(dealerHandValue);
            }
            Console.WriteLine("Value = " + dealerHandValue);

            if (playerHandValue == dealerHandValue)
            {
                Console.WriteLine("Draw!");
                return EndCondition.Draw;
            }
            else if (playerHandValue > dealerHandValue)
            {
                LogWithColor("Player Wins!", ConsoleColor.Green);
                return EndCondition.PlayerWins;
            }
            else 
            {
                LogWithColor("House Wins!", ConsoleColor.Magenta);
                return EndCondition.DealerWins;
            }
        }

        private static void UpdateScore(EndCondition endCond, ref (int, int, int) score) 
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

//add tostring method for deck and card, card shows suit, rank and value. Deck prints the print of card in a loop

//settings:
//beginner vs. expert mode where beginner prints card values and shows hints if you should stick or twist
//let user choose color, regardless change color of different logged elements, maybe color match suits too
//can do an override of writeline that takes color as an argument to fascilitate this

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