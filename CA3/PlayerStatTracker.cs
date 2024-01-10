using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA3
{
    public class PlayerStatTracker
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

        public decimal PlayerFunds { get; private set; }
        public decimal CurrentBet { get; set; }

        public EndCondition CurrentEndCondition { get; set; }

        public bool GameOver { get; private set; }  

        public PlayerStatTracker(int wins, int loses, int draws, decimal funds, decimal bet, EndCondition endCond, bool gameOver) 
        {
            Wins = wins;
            Losses = loses;
            Draws = draws;
            PlayerFunds = funds;
            CurrentBet = bet;
            CurrentEndCondition = endCond;
            GameOver = false;
        }

        public PlayerStatTracker(int wins, int loses, int draws)
        {
            Wins = wins;
            Losses = loses;
            Draws = draws;
        }

        public PlayerStatTracker() { }

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

        public void GetPlayerBet()
        {
            CurrentBet = 0;
            Logger.Log($"Current Balance: {PlayerFunds:C}", makeNewLn: true);
            Logger.Log("How Much Would You Like To Bet?", makeNewLn: true);
            while (true)
            {
                decimal bet;
                bool success = decimal.TryParse(Console.ReadLine(), out bet);

                if (success && bet <= PlayerFunds)
                {
                    CurrentBet = bet;
                    Console.Clear();
                    Logger.Log($"Current Bet: {CurrentBet:C}", 5);
                    return;
                }
                if (bet > PlayerFunds) 
                {
                    Logger.Log("Insufficient Funds");
                }
                Console.WriteLine("Please Enter Valid Value For Bet");
            }
        }

        private void updateFunds(decimal difference) 
        {
            PlayerFunds = Math.Max(PlayerFunds + difference, 0);
            if (PlayerFunds <= 0) 
            {
                Logger.LogWithColor("You Ran Out Of Funds!", ConsoleColor.Red);
                GameOver = true;
            }
        }

        private EndCondition declareWin()
        {
            Logger.LogWithColor("Player Wins!", ConsoleColor.White, ConsoleColor.Green, 5, true);
                            Logger.LogWithColor($"Rewarding Bet Of {CurrentBet:C}", ConsoleColor.Green);
                updateFunds(CurrentBet);
            return EndCondition.PlayerWins;
        }

        private EndCondition declareLoss()
        {
            Logger.LogWithColor("House Wins!", ConsoleColor.White, ConsoleColor.Magenta, 5, true);
            Logger.LogWithColor($"Deducting Bet Of {CurrentBet:C}", ConsoleColor.Red);
            updateFunds(-CurrentBet);
            return EndCondition.DealerWins;
        }

        private EndCondition declareDraw()
        {
            Logger.LogWithColor("Draw!", ConsoleColor.White, ConsoleColor.DarkGray, 5, true);
            Logger.Log($"Returning Bet Of {CurrentBet:C}");
            return EndCondition.Draw;
        }

        private EndCondition declareNoOutcome()
        {
            Logger.LogWithColor("Could Not Determine Outcome".PadLeft(10), ConsoleColor.White, ConsoleColor.Red, 5, true);
            return EndCondition.None;
        }
    }
}
