using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TXM.Core.Models;

namespace TXM.Core
{
    [Serializable]
	public class GameOfThrones2ndRules : AbstractRules
	{
        private new static string name = "A Game of Thrones\u2122: The Card Game 2nd Edition";

        public GameOfThrones2ndRules()
        {
            IsDrawPossible = false;
            OptionalFields = new List<string>() { " ModWins",  "eSoS" };
            IsDoubleElimination = false;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = true;
            DefaultMaxPoints = 0;
            Factions = new string[] { "House Baratheon", "House Greyjoy", "House Lannister", "House Martell", "House Stark", "House Targaryen", "House Tyrell", "The Night's Watch" };
            DefaultTime = 55;
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
				tP = 5;
			}
			else if (tP == 0 && newResult.WinnerID != newResult.EnemyID)
			{
				tP = 4;
			}
			else
			{
				tP = 0;
			}

			TtournamentPoints = f.Invoke(0, tP);
			switch (tP)
			{
				case 5:
                    Twins = f.Invoke(0, 1);
                    break;
                case 4:
					TmodifiedWins = f.Invoke(0, 1);
					break;
				case 0:
					Tlosses = f.Invoke(0, 1);
					break;
			}

			return tP == 1;
		}

        public override ObservableCollection<Models.Player> SortTable(ObservableCollection<Models.Player> unsorted)
        {
            ObservableCollection<Models.Player> t = (ObservableCollection<Models.Player>)unsorted.OrderByDescending(x => x.TournamentPoints).ThenByDescending(x => x.StrengthOfSchedule).ThenByDescending(x => x.ExtendedStrengthOfSchedule).ThenBy(x => x.Order);
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
