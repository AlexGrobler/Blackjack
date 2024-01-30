using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CA3
{
    //class for keeping track of and calculating player stats, determining if player won
    public class StatTracker
    {
        public enum EndCondition
        {
            None,
            Draw,
            PlayerWins,
            DealerWins
        }

        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Draws { get; private set; }

        public int PlayerFunds { get; private set; }
        public int DealerFunds { get; private set; }
        public int StartingFunds { get; private set; }
        public int CurrentBet { get; set; }

        public EndCondition CurrentEndCondition { get; set; }

        //game over is declared if player runs out of funds, which shows game over screen and closes app
        public bool GameOver { get; private set; }
        public bool GameWon { get; private set; }

        const int MINIMUM_BET = 1;

        public StatTracker(int wins, int loses, int draws, int startingFunds) 
        {
            Wins = wins;
            Losses = loses;
            Draws = draws;
            PlayerFunds = startingFunds;
            DealerFunds = startingFunds;
            StartingFunds = startingFunds;
            CurrentBet = 0;
            CurrentEndCondition = EndCondition.None;
            GameOver = false;
            GameWon = false;
        }

        public StatTracker(int wins, int loses, int draws)
        {
            Wins = wins;
            Losses = loses;
            Draws = draws;
            PlayerFunds = 1000;
            DealerFunds = 1000;
            CurrentBet = 0;
            CurrentEndCondition = EndCondition.None;
            GameOver = false;
            GameWon = false;
        }

        public StatTracker() { }

        //logs game's stats
        public void ShowStats()
        {
            Console.Clear();
            Logger.LogWithColor("=====Stats=====", ConsoleColor.DarkGreen, spacing: 5, newLn: true);
            Logger.Log($"Your Funds: {PlayerFunds:C}", 9);
            Logger.Log($"Dealer's Funds: {DealerFunds:C}", 9);
            Logger.Log("Wins: " + Wins, 9);
            Logger.Log("Loses: " + Losses, 9);
            Logger.Log("Draws: " + Draws, 9);
            Logger.LogWithColor("================\n", ConsoleColor.DarkGreen, spacing: 5);
        }

        public void ResetStats(bool doFullReset) 
        {
            CurrentEndCondition = EndCondition.None;
            CurrentBet = 0;
            GameOver = false;
            GameWon = false;
            if (doFullReset)
            {
                PlayerFunds = StartingFunds;
                DealerFunds = StartingFunds;
                Wins = 0;
                Losses = 0;
                Draws = 0;
            }
        }

        //Determines the end state of the round based on if player/dealer bust and if neither then hand value
        //then update stats
        public void DetermineWinner(bool playerBusts, bool dealerBusts, int playerHandValue, int dealerHandValue)
        {
            if (!playerBusts && !dealerBusts)
            {
                CurrentEndCondition = compareHandValues(playerHandValue, dealerHandValue);
            }
            else if (playerBusts)
            {
                CurrentEndCondition = declareLoss();
            }
            else if (dealerBusts)
            {
                CurrentEndCondition = declareWin();
            }
            else if (dealerBusts && playerBusts)
            {
                CurrentEndCondition = declareDraw();
            }
            else 
            {
                CurrentEndCondition = declareNoOutcome();
            }

            updateStats();
        }

        //compare value of dealer and player hands to determine who won
        private EndCondition compareHandValues(int playerHandValue, int dealerHandValue)
        {
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


        //update tally of wins, loses and draws
        private void updateStats()
        {
            if (CurrentEndCondition == EndCondition.Draw)
            {
                Draws++;
            }
            if (CurrentEndCondition == EndCondition.PlayerWins)
            {
               Wins++;
            }
            if (CurrentEndCondition == EndCondition.DealerWins)
            {
                Losses++;
            }            
        }

        //get player's bet, make sure it's a valid amount
        public void GetPlayerBet()
        {
            CurrentBet = 0;
            Logger.Log($"Your Current Balance: {PlayerFunds:C}", newLn: true);
            Logger.Log($"Dealer's Balance: {DealerFunds:C}", newLn: true);
            Logger.Log("How Much Would You Like To Bet? Cents Aren't Accepted", newLn: true);
            while (true)
            {
                int bet;
                bool success = int.TryParse(Console.ReadLine(), out bet);

                if (success && bet <= PlayerFunds && bet >= MINIMUM_BET)
                {
                    CurrentBet = bet;
                    Console.Clear();
                    Logger.Log($"Current Bet: {CurrentBet:C}", 5);
                    return;
                }
                if (success && bet > PlayerFunds) 
                {
                    Logger.Log("Insufficient Funds");
                }
                if (success && bet < MINIMUM_BET)
                {
                    Logger.Log($"Bet Must Be At Least Minimum {MINIMUM_BET:C}");
                }
                Console.WriteLine("Please Enter Valid Value For Bet");
            }
        }

        //update player's current funds, check if they ran out and set GameOver to true is so
        private int updateFunds(int difference, bool isPlayer, int funds) 
        {
            funds = Math.Max(funds + difference, 0);
            if (funds <= 0) 
            {
                if (isPlayer)
                {
                    Logger.LogWithColor("You Ran Out Of Funds!", ConsoleColor.Red, spacing: 2);
                    GameOver = true;
                }
                else 
                {
                    Logger.LogWithColor("Dealer Ran Out Of Funds!", ConsoleColor.Green, spacing: 2);
                    GameWon = true;
                }
            }
            return funds;
        }


        //log that player won, update funds
        private EndCondition declareWin()
        {
            Logger.LogWithColor("You Win!", ConsoleColor.White, ConsoleColor.Green, 5, true);
            Logger.LogWithColor($"Rewarding Bet Of {CurrentBet:C}", ConsoleColor.Green, spacing: 2, newLn: true);
            PlayerFunds = updateFunds(CurrentBet, true, PlayerFunds);
            DealerFunds = updateFunds(-CurrentBet, false, DealerFunds);
            return EndCondition.PlayerWins;
        }

        //log that player lost, update funds
        private EndCondition declareLoss()
        {
            Logger.LogWithColor("House Wins!", ConsoleColor.White, ConsoleColor.Magenta, 5, true);
            Logger.LogWithColor($"Deducting Bet Of {CurrentBet:C}", ConsoleColor.Red, spacing: 2, newLn: true);
            PlayerFunds = updateFunds(-CurrentBet, true, PlayerFunds);
            DealerFunds = updateFunds(CurrentBet, false, DealerFunds);
            return EndCondition.DealerWins;
        }

        //log that player drew, update funds
        private EndCondition declareDraw()
        {
            Logger.LogWithColor("Draw!", ConsoleColor.White, ConsoleColor.DarkGray, 5, true);
            Logger.Log($"Returning Bet Of {CurrentBet:C}", 2, true);
            return EndCondition.Draw;
        }

        //log that no valid outcome was found for debugging purposes
        private EndCondition declareNoOutcome()
        {
            Logger.LogWithColor("Could Not Determine Outcome".PadLeft(10), ConsoleColor.White, ConsoleColor.Red, 5, true);
            return EndCondition.None;
        }
    }
}
