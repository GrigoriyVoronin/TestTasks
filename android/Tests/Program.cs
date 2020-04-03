using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    public class BowlingGame
    {

        private bool isSpareBonus;
        private int currentRollIndex;
        private bool isDoubleStrikeBonus;
        private int rollsWithStrikeBonusCount;
        public readonly List<int> Rolls;

        public BowlingGame(IEnumerable<int> rolls)
        {
            Rolls = rolls.ToList();
        }

        public void Roll(int pins)
        {
            Rolls.Add(pins);
        }

        private bool IsStrike(int pins)
        {
            return pins == 10;
        }

        private bool IsSpare(int pins, int prevRollPins)
        {
            return prevRollPins + pins == 10;
        }

        private bool IsExtraRoll()
        {
            return currentRollIndex >= 19;
        }

        private int AddBonusPoints(int pins)
        {
            var bonusPoints = 0;
            if (rollsWithStrikeBonusCount > 0)
            {
                rollsWithStrikeBonusCount--;
                bonusPoints += pins;
            }
            else if (isSpareBonus)
            {
                isSpareBonus = false;
                bonusPoints += pins;
            }

            if (isDoubleStrikeBonus)
            {
                isDoubleStrikeBonus = false;
                bonusPoints += pins;
            }

            return bonusPoints;
        }

        public int GetScore()
        {
            var prevRollPins = 0;
            var score = 0;
            foreach (var pins in Rolls)
            {
                score += AddBonusPoints(pins);
                if (!IsExtraRoll())
                {
                    isSpareBonus = IsSpare(pins, prevRollPins);

                    if (IsStrike(pins))
                    {
                        currentRollIndex++;
                        isDoubleStrikeBonus = rollsWithStrikeBonusCount > 0;
                        rollsWithStrikeBonusCount = 2;
                    }

                    score += pins;
                }

                currentRollIndex++;
                prevRollPins = pins;
            }

            return score > 0 ? score : int.MaxValue;
        }
    }
    class Program
    {
        public static BowlingGame InitGame()
        {
            var a = Console.ReadLine().Split().Select(x => int.Parse(x));
            return new BowlingGame(a);
        }

        static void Main()
        {
            var a = InitGame();
            Console.WriteLine(a.GetScore());
        }
    }
}