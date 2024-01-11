using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CA3.Card;

namespace CA3
{
    //Deck class to hold all the cards that can be used during a game
    class Deck
    {
        public List<Card> CardDeck = new List<Card>();

        public Deck() 
        {
            InitializeDeck();
        }

        public Deck(List<Card> customDeck)
        {
            CardDeck = customDeck;
        }

        //Fisher-Yates shuffle found online and adapted
        public void ShuffleDeck() 
        {
            Random rnd = new Random();
            int n = CardDeck.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Card tempValue = CardDeck[k];
                CardDeck[k] = CardDeck[n];
                CardDeck[n] = tempValue;
            }
        }

        //easy way to log all the ranks and suits of all cards in the deck
        public override string ToString()
        {
            string cards = "";

            foreach (Card card in CardDeck) 
            {
                cards += card.ToString() + "\n";
            }

            return cards;
        }

        //take first card obj from list and pass to local variable. Remove card obj from list, then return the local variable's reference to it
        public Card DrawCard() 
        {
            Card drawnCard = CardDeck[0];
            CardDeck.RemoveAt(0);
            return drawnCard;
        }

        //For initialzing and reseting the deck. Use card class's enums to iterate all the possible the suits, then all the poissible ranks
        //ensures 52 cards are added without manually creating each card
        public void InitializeDeck() 
        {
            CardDeck.Clear();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    CardDeck.Add(new Card(suit, rank));
                }
            }
        }
    }
}
