using System;
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
            int playerScore = 0;
            int handValue = 0;
            Deck deck = new Deck();
            deck.ShuffleDeck();

            foreach (Card card in deck.CardDeck) 
            {
                Console.WriteLine("Suit: {0}, Rank: {1}", card.Suit, card.Rank);
            }

            while (true)
            {
                Console.WriteLine("Your Score Is {0}", playerScore);
                Console.WriteLine("Would You Like To Play A Game? type yes or no");
                string keepPlaying = Console.ReadLine();

                if (keepPlaying.ToLower() == "no")
                {
                    break;
                }
                else if (keepPlaying.ToLower() == "yes")
                {
                    continue;
                }
                else
                {
                    Console.WriteLine("Please type yes or no");
                }


 
            }

            Console.ReadLine(); 

            //display user score
            //prompt user for playing a game
            //clear console
            //select 2 card objects from list of all posible cards at random
            //display card value
            //ask user if they want to receive another card or stop (stick or twist)

            //if twist loop over adding another card then ask if they want to s/t
            
            //every time player receives another card, check if value is over 21 (they lose if so)

            //if the player hasn't lost, and choose stick, then deal dealer's cards
            //dealer must keep drawing until they get at least 17 (dealer only stops once score is 17 or higher or bust)
            
            //compare player's card value vs. dealer's and determine winner

            //record win/loss/draw and add to user score
            
            //prompt user for a new game or to quit
        }
    }
}
