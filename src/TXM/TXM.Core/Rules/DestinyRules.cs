using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TXM.Core
{
    [Serializable]
	public class DestinyRules : AbstractRules
	{
        private new static string name = "Star Wars\u2122: Destiny";

        public DestinyRules()
        {
            IsDrawPossible = false;
            OptionalFields = new List<string>() { "eSoS" };
            IsDoubleElimination = false;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = false;
            DefaultMaxPoints = 0;
            Factions = new string[] { "Heroes", "Villains" };
            DefaultTime = 35;
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
			if (result.EnemyID == -1 || result.EnemyID == -2)
			{
				newResult = new Result(1, 0, result.EnemyID, 1, result.WinnerID);
			}

			int tP = newResult.Destroyed - newResult.Lost;
			if (tP > 0)
			{
				tP = 1;
			}
			else if (tP == 0 && newResult.WinnerID != newResult.EnemyID)
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

        public override ObservableCollection<Player> SortTable(ObservableCollection<Player> unsorted)
        {
            ObservableCollection<Player> t = (ObservableCollection<Player>)unsorted.OrderByDescending(x => x.TournamentPoints).ThenByDescending(x => x.StrengthOfSchedule).ThenByDescending(x => x.ExtendedStrengthOfSchedule).ThenBy(x => x.Order);
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
