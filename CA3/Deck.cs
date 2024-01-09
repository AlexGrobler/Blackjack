using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CA3.Card;

namespace CA3
{
    class Deck
    {
        public List<Card> CardDeck = new List<Card>();

        public Deck() 
        {
            ResetDeck();
        }

        public Deck(List<Card> customDeck)
        {
            CardDeck = customDeck;
        }

        //Fisher-Yates shuffle
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

        public override string ToString()
        {
            string cards = "";

            foreach (Card card in CardDeck) 
            {
                cards += card.ToString() + "\n";
            }

            return cards;
        }

        public Card DrawCard() 
        {
            Card drawnCard = CardDeck[0];
            CardDeck.RemoveAt(0);
            return drawnCard;
        }


        public void ResetDeck() 
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
