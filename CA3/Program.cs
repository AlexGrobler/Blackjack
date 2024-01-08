﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    internal class Program
    {
        static void Main(string[] args)
        {

            bool keepPlaying = false;
            bool isFirstRound = true; //change to int and keep track of rounds
            bool isPlayerTurn = true;

            bool dealerBusts = false;
            bool playerBusts = false;

            bool keepAppRunning = true;

            Deck deck = new Deck();

            List<Card> playersHand = new List<Card>();
            List<Card> dealersHand = new List<Card>();

            while (keepAppRunning)
            {
                if (!keepPlaying)
                {
                    Menu(deck, ref keepPlaying, ref keepAppRunning);
                }

                while (keepPlaying)
                {
                    StartRound(deck, dealersHand, playersHand);

                    playerBusts = PlayersTurn(deck, playersHand);

                    if(!playerBusts)
                    {
                        dealerBusts = DealersTurn(deck, dealersHand, ref isFirstRound);
                    }

                    if (!playerBusts && !dealerBusts) 
                    {

                    }
                }
            }

            Console.WriteLine("\n\n======================");
            Console.WriteLine("==GAME OVER==");
            Console.ReadLine();
        }

        private static void Menu(Deck deck, ref bool isPlaying, ref bool keepAppRunning)
        {
            Console.WriteLine("Would You Like To Play A Game? type yes or no");
            string keepPlaying = Console.ReadLine();
            if (keepPlaying.ToLower() == "no")
            {
                keepAppRunning = false;
            }
            else if (keepPlaying.ToLower() == "yes")
            {
                isPlaying = true;
                deck.ResetDeck();
                deck.ShuffleDeck();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Please type yes or no");
            }
        }

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
            Console.WriteLine("D================\n");

            while (true) 
            {
                ShowHand(playersHand);
                Console.WriteLine("Stick Or Twist?");
                string stickTwist = Console.ReadLine().ToLower();

                if (stickTwist == "t")
                {
                    Twist(deck, playersHand);

                    if (CheckHand(playersHand) > 21) 
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

        private static bool DealersTurn(Deck deck, List<Card> dealersHand, ref bool isFirstRound) 
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
                int handValue = CheckHand(dealersHand);

                if (handValue > 21) 
                {
                    Console.WriteLine("Dealer Has Busted");
                    return true;
                }
                else if (handValue <= 16)
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

        private static int CheckHand(List<Card> hand)
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

        private static void CompareHands(List<Card> playersHand, List<Card> dealersHand) 
        {
            int playerHandValue = 0;
            int dealerHandValue = 0;

            Console.WriteLine("\n\nYour Hand");
            Console.WriteLine("======================");
            foreach (Card card in playersHand)
            {
                playerHandValue += card.GetCardValue(playerHandValue);
            }

            Console.WriteLine("\n\nDealer's Hand");
            Console.WriteLine("======================");
            foreach (Card card in dealersHand)
            {
                dealerHandValue += card.GetCardValue(dealerHandValue);
            }

            if (playerHandValue == dealerHandValue)
            {
                //declare tie
            }
            else if (playerHandValue > dealerHandValue)
            {
                //declare player winner
            }
            else 
            {
                //declare dealer winner
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