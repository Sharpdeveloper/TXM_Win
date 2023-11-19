using System;
using System.Collections.Generic;
using System.Linq;

namespace TXM.Core
{
    [Serializable]
    public class DiceThroneRules : AbstractRules
    {
        private new static string name = "Dice Throne";

        public DiceThroneRules()
        {
            IsDrawPossible = false;
            OptionalFields = new List<string>() { "eSoS" };
            IsDoubleElimination = false;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = true;
            DefaultMaxPoints = 0;
            Factions = new string[] { "Artificer", "Barbarian", "Black Panther", "Black Widow", "Captain Marvel", "Cursed Pirate", "Doctor Strange", "Gunslinger",
                                      "Huntress", "Krampus", "Loki", "Monk", "Moon Elf", "Ninja", "Paladin",
                                      "Pyromancer", "Samurai", "Santa", "Scarlet Witch", "Shadow Thief", "Seraph", "Spider-Man",
                                      "Tactican", "Thor", "Treant", "Vampire Lord" };
            DefaultTime = 90;
            base.name = name;
        }

        public static string GetRuleName()
        {
            return name;
        }

        protected override bool CalculateResult(Result result, Func<int, int, int> f)
        {
            Result newResult = result;

            //ID == -1 => Bye
            if (result.Enemy.ID == -1 || result.Enemy.ID == -2)
            {
                newResult = new Result(1, 0, result.Enemy, 1, result.WinnerID);
            }

            int tP = newResult.Destroyed - newResult.Lost;
            if (tP > 0)
            {
                tP = 1;
            }
            else if (tP == 0 && newResult.WinnerID != newResult.Enemy.ID)
            {
                tP = 1;
            }
            else
            {
                tP = 0;
            }

            TtournamentPoints = f.Invoke(0, tP);
            switch (tP)
            {
                case 1:
                    Twins = f.Invoke(0, 1);
                    break;
                case 0:
                    Tlosses = f.Invoke(0, 1);
                    break;
            }

            return tP == 1;
        }

        public override List<Player> SortTable(List<Player> unsorted)
        {

            List<Player> t = unsorted.OrderByDescending(x => x.IsInCut).ThenByDescending(x => x.TournamentPoints).ThenByDescending(x => x.StrengthOfSchedule).ThenByDescending(x => x.ExtendedStrengthOfSchedule).ThenByDescending(x => x.Order).ToList<Player>();
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
