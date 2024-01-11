using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    //class to represent cards
    public class Card
    {
        public enum CardSuit
        {
            Hearts, 
            Diamonds,
            Clubs,
            Spades
        }

        //enum values used to determine card value
        public enum CardRank
        {
            Ace = 0,
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
            King = 13
        }

        public CardSuit Suit { get; private set; }
        public CardRank Rank { get; private set; }

        //if card has enum value over 10, it should equal 10
        public int CardValue 
        {
            get
            {
                int cardValue = (int)Rank;
                if (cardValue > 10)
                {
                    return 10;
                }
                if (Rank == CardRank.Ace) 
                {
                    return 11;
                }
                return cardValue;
            }
        }

        public Card(CardSuit suit, CardRank rank) 
        {
            Suit = suit;
            Rank = rank;    
        }

        public Card() { }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }


        //relate each suit to a color for logging
        private static ConsoleColor getSuitColor(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Diamonds:
                    return ConsoleColor.Red;
                case CardSuit.Hearts:
                    return ConsoleColor.Red;
                case CardSuit.Clubs:
                    return ConsoleColor.Gray;
                case CardSuit.Spades:
                    return ConsoleColor.Gray;
                default: return ConsoleColor.White;
            }
        }

    }
}
