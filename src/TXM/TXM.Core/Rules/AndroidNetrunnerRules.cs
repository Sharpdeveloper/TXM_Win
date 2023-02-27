using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TXM.Core.Models;

namespace TXM.Core
{
    [Serializable]
	public class AndroidNetrunnerRules : AbstractRules
	{
        protected new static string name = "Android: Netrunner - The Card Game";

        public AndroidNetrunnerRules()
        {
            IsDrawPossible = true;
            OptionalFields = new List<string>() { "ModWins", "Draw", "eSoS" };
            IsDoubleElimination = true;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = false;
            DefaultMaxPoints = 0;
            Factions = new string[] { "Hass-Bioroid/Anarch", "Hass-Bioroid/Criminal", "Hass-Bioroid/Shaper",
                "Jinteki/Anarch", "Jinteki/Criminal", "Jinteki/Shaper",
                "NBN/Anarch", "NBN/Criminal", "NBN/Shaper",
                "Weyland Consortium/Anarch", "Weyland Consortium/Criminal", "Weyland Consortium/Shaper" };
            DefaultTime = 65;
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

            int tP = newResult.Destroyed;
			TtournamentPoints = f.Invoke(0, tP);
			switch (tP)
			{
				case 3:
					Twins = f.Invoke(0, 1);
					break;
                case 2:
                    TmodifiedWins = f.Invoke(0, 1);
                    break;
                case 1:
                    Tdraws = f.Invoke(0, 1);
                    break;
                case 0:
					Tlosses = f.Invoke(0, 1);
					break;
			}

			return tP > 1;
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
