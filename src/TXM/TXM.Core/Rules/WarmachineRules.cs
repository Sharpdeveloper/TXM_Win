using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TXM.Core.Models;

namespace TXM.Core
{
    [Serializable]
    public class WarmachineRules : AbstractRules
    {
        private new static string name = "Warmachine & Hordes";

        public WarmachineRules()
        {
            IsDrawPossible = true;
            OptionalFields = new List<string>() { "SoS" };
            IsDoubleElimination = false;
            IsRandomSeeding = false;
            IsWinnerDropDownNeeded = true;
            DefaultMaxPoints = 75;
            Factions = new string[] {"Circle of Orboros", "Convergence of Cyriss", "Crucible Guard", "Cryx", "Cygnar",
                                     "Grymkin", "Infernals", "Khador", "Legion of Everblight", "Mercenaries", "Minions",
                                     "Protectorate of Menoth", "Retribution of Scyrah", "Skorne", "Trollblood" };
            DefaultTime = 120;
            base.name = name;
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
            /*if (result.Enemy.ID == -1)
			{
				newResult = new Result((int)(0.5 * result.MaxPoints), 0, result.Enemy, result.MaxPoints, result.WinnerID);
			}
			//ID == -2 => WonBye
			else if (result.Enemy.ID == -2)
			{
				newResult = new Result((int) result.MaxPoints, 0, result.Enemy, result.MaxPoints, result.WinnerID);
			}*/

            int tP = newResult.Destroyed - newResult.Lost;

            //TdestroyedPoints = f.Invoke(0, newResult.Destroyed);
            //TlostPoints = f.Invoke(0, newResult.Lost);
            TtournamentPoints = f.Invoke(0, tP);
            if (newResult.WinnerID == newResult.EnemyID)
                Tlosses = f.Invoke(0, 1);
            else
                Twins = f.Invoke(0, 1);

            return newResult.WinnerID != newResult.EnemyID;
        }

        public override ObservableCollection<Models.Player> SortTable(ObservableCollection<Models.Player> unsorted)
        {
            ObservableCollection<Models.Player> t = (ObservableCollection<Models.Player>)unsorted.OrderByDescending(x => x.Wins).ThenByDescending(x => x.StrengthOfSchedule).ThenByDescending(x => x.TournamentPoints).ThenBy(x => x.Order);
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
