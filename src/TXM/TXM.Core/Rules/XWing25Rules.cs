﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TXM.Core
{
    [Serializable]
    public class XWing25Rules : AbstractRules
    {
        private new static string name = "Star Wars\u2122: X-Wing\u2122 Miniatures Game 2.5";

        public XWing25Rules()
        {
            IsDrawPossible = true;
            OptionalFields = new List<string>() { "MoV" };
            IsDoubleElimination = false;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = false;
            DefaultMaxPoints = 20;
            Factions = new string[] { "First Order", "Galactic Empire", "Galactic Republic", "Rebel Alliance", "Resistance", "Scum and Villainy", "Separatist Allaince" };
            DefaultTime = 75;
            DefaultRandomMins = 3;
            movName = "MP";
            base.name = name;
            hasScenarios = true;
            Scenarios = new string[] { "Assault at the Satellite Array", "Chance Engagement", "Salvage Mission", "Scramble the Transmissions" };
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
            if (result.Enemy.ID == -1)
            {
                newResult = new Result((int)(0.9 * result.MaxPoints), 0, result.Enemy, result.MaxPoints, result.WinnerID);
            }
            //ID == -2 => WonBye
            else if (result.Enemy.ID == -2)
            {
                newResult = new Result( result.MaxPoints, 0, result.Enemy, result.MaxPoints, result.WinnerID);
            }

            int tP = newResult.WinnerID == -99 ? -1 : newResult.Destroyed - newResult.Lost;
            if (tP > 0)
            {
                tP = 3;
            }
            else if (tP == 0)
            {
                tP = 1;
            }
            else
            {
                tP = 0;
            }

            TmarginOfVictory = f.Invoke(0, newResult.Destroyed);
            //	TdestroyedPoints = f.Invoke(0, newResult.Destroyed);
            //	TlostPoints = f.Invoke(0, newResult.Lost);
            TtournamentPoints = f.Invoke(0, tP);
            switch (tP)
            {
                case 3:
                    Twins = f.Invoke(0, 1);
                    break;
                case 1:
                    Tdraws = f.Invoke(0, 1);
                    break;
                case 0:
                    Tlosses = f.Invoke(0, 1);
                    break;
            }

            return tP == 3;
        }

        public override List<Player> SortTable(List<Player> unsorted)
        {
            List<Player> t = unsorted.OrderByDescending(x => x.TournamentPoints).ThenByDescending(x => x.StrengthOfSchedule).ThenByDescending(x => x.MarginOfVictory).ThenBy(x => x.Order).ToList<Player>();
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}
