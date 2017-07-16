﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TXM.Core
{
    [Serializable]
	public class RuneWars_Armada_Rules : AbstractRules
	{
        private new static string name = "Experimental - Runewars (With Armada Rules)";

        public RuneWars_Armada_Rules()
        {
            IsDrawPossible = false;
            OptionalFields = new List<string>() { "MoV" };
            IsDoubleElimination = false;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = true;
            DefaultMaxPoints = 200;
            Factions = new string[] { "Daqan", "Latari", "Waiqar" };
            DefaultTime = 105;
        }

        public static string GetRuleName()
		{
            return name;
		}

        protected override bool CalculateResult(Result result, Func<int, int, int> f)
		{
			Result newResult = result;

			//ID == -1 => Bye
			if (result.Enemy.ID == -1)
			{
				newResult = new Result((int)0.35 * result.MaxPoints, 0, result.Enemy, result.MaxPoints, result.WinnerID);
			}
			//ID == -2 => WonBye
			else if (result.Enemy.ID == -2)
			{
				newResult = new Result((int)0.35 * result.MaxPoints, 0, result.Enemy, result.MaxPoints, result.WinnerID);
			}

			int tP = newResult.Destroyed - newResult.Lost;
            if (tP == 0 && newResult.WinnerID != newResult.Enemy.ID)
            {
                tP = 6;
            }
            else if (tP <= -150)
			{
				tP = 1;
			}
			else if (tP <= -110)
			{
				tP = 2;
			}
            else if (tP <= -70)
            {
                tP = 3;
            }
            else if (tP <= -30)
            {
                tP = 4;
            }
            else if (tP <= 0)
            {
                tP = 5;
            }
            else if (tP <= 29)
            {
                tP = 6;
            }
            else if (tP <= 69)
            {
                tP = 7;
            }
            else if (tP <= 109)
            {
                tP = 8;
            }
            else if (tP <= 149)
            {
                tP = 9;
            }
            else
			{
				tP = 10;
			}

			TmarginOfVictory = f.Invoke(0, (newResult.Destroyed - newResult.Lost));
			TdestroyedPoints = f.Invoke(0, newResult.Destroyed);
			TlostPoints = f.Invoke(0, newResult.Lost);
			TtournamentPoints = f.Invoke(0, tP);
			if(tP >= 6)
				Twins = f.Invoke(0, 1);
			else
				Tlosses = f.Invoke(0, 1);

			return tP >=6;
		}

        public override List<Player> SortTable(List<Player> unsorted)
        {
            List<Player> t = unsorted.OrderByDescending(x => x.TournamentPoints).ThenByDescending(x => x.MarginOfVictory).ThenByDescending(x => x.StrengthOfSchedule).ThenBy(x => x.Order).ToList<Player>();
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
