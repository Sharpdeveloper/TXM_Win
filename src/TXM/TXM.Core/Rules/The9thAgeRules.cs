using System;
using System.Collections.Generic;
using System.Linq;

namespace TXM.Core
{
    [Serializable]
	public class The9thAgeRules : AbstractRules
	{
        private new static string name = "The 9th Age\u2122";

        public The9thAgeRules()
        {
            IsDrawPossible = true;
            OptionalFields = new List<string>() { "MoV" };
            IsDoubleElimination = false;
            IsRandomSeeding = false;
            IsWinnerDropDownNeeded = false;
            DefaultMaxPoints = 2500;
            Factions = new string[] { "Beast Herds", "Daemon Legions", "Dread Elves", "Dwarven Holds",
                                      "Empire of Sonnstahl", "Highborn Elves", "infernal Dwarves",
                                      "Kingdom of Equitane", "Ogre Khans", "Orcs and Goblins",
                                      "Saurian Ancients", "Sylvan Elves", "Undying Dynasties",
                                      "The Vermin Swarm", "Vampire Coevenant", "Warriors of the Dark Gods"};
            DefaultTime = 180;
            base.name = name;
            movName = "KP";
            tournamentPoints = true;
        }

        public static string GetRuleName()
		{
            return name;
		}

        protected override bool CalculateResult(Result result, Func<int, int, int> f)
		{
			Result newResult = result;

            if (newResult.MaxPoints == 0)
            {
                newResult.MaxPoints = DefaultMaxPoints;
            }

            //ID == -1 => Bye
            if (result.Enemy.ID <= -1)
			{
				newResult = new Result((int)0.5 * result.MaxPoints, 0, result.Enemy, result.MaxPoints, result.WinnerID, 15);
			}

            int tP = newResult.TournamentPoints;

			TmarginOfVictory = f.Invoke(0, (newResult.Destroyed - newResult.Lost));
			TdestroyedPoints = f.Invoke(0, newResult.Destroyed);
			TlostPoints = f.Invoke(0, newResult.Lost);
			TtournamentPoints = f.Invoke(0, tP);
            if (tP > 10)
                Twins = f.Invoke(0, 1);
            else if (tP == 10)
                Tdraws = f.Invoke(0, 1);
            else
                Tlosses = f.Invoke(0, 1);

			return tP >10;
		}

        public override List<Player> SortTable(List<Player> unsorted)
        {
         
            List<Player> t = unsorted.OrderByDescending(x => x.IsInCut).ThenByDescending(x => x.TournamentPoints).ThenByDescending(x => x.Wins).ThenByDescending(x => x.Order).ToList<Player>();
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
