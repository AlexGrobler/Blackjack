﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    public class Card
    {

        public enum CardSuit
        {
            Hearts, 
            Diamonds,
            Clubs,
            Spades
        }

        public enum CardRank
        {
            One = 1, 
            Two = 2,
            Three = 3, 
            Four = 4, 
            Five = 5, 
            Six = 6, 
            Seven = 7, 
            Eight = 8, 
            Nine = 9, 
            Ten = 10, 
            Jack = 11, 
            Queen = 12, 
            King = 13, 
            Ace = 0
        }

        public CardSuit Suit { get; set; }
        public CardRank Rank { get; set; }

        public Dictionary<CardRank, int> CardValues;
 
        public Card(CardSuit suit, CardRank rank) 
        {
            Suit = suit;
            Rank = rank;    
        }

        public int GetCardValue(int handValue)
        {
            int value = (int)Rank;
            if (value >= 10) 
            {
                return 10;
            }
            if (value == 0) 
            {
                if (handValue + 11 > 21) 
                {
                    return 1;
                }
                return 11;
            }
            return value;
        }


    }
}