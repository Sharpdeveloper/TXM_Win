using System;
using System.Collections.Generic;
using System.Linq;

namespace TXM.Core
{
    [Serializable]
    public class MastersofTheUniverseBattlegroundRules : AbstractRules
    {
        private new static string name = "Masters of The Universe\u2122: Battleground";

        public MastersofTheUniverseBattlegroundRules()
        {
            IsDrawPossible = true;
            IsWinnerDropDownNeeded = true;
            OptionalFields = new List<string>() { "MoV" };
            movName = "Victory Points";
            IsDoubleElimination = false;
            IsRandomSeeding = true;
            IsWinnerDropDownNeeded = true;
            DefaultMaxPoints = 100;
            Factions = new string[] { "Masters of The Universe™", "Evil Warriors", "Evil Horde", "The Great Rebellion" };
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
            if (result.Enemy.ID == -1)
            {
                newResult = new Result(120, 0, result.Enemy, result.MaxPoints
                    , result.WinnerID);
            }
            //ID == -2 => WonBye
            else if (result.Enemy.ID == -2)
            {
                newResult = new Result(120, 0, result.Enemy, result.MaxPoints, result.WinnerID);
            }

            int tP = newResult.Destroyed - newResult.Lost;
            if (tP >= 20)
            {
                tP = 2;
            }
            else if (tP <= -20)
            {
                tP = 0;
            }
            else
            {
                tP = 1;
            }

            TmarginOfVictory = f.Invoke(0, newResult.Destroyed);
            TdestroyedPoints = f.Invoke(0, newResult.Destroyed);
            TlostPoints = f.Invoke(0, newResult.Lost);
            TtournamentPoints = f.Invoke(0, tP);
            switch (tP)
            {
                case 2:
                    Twins = f.Invoke(0, 1);
                    break;
                case 1:
                    Tdraws = f.Invoke(0, 1);
                    break;
                case 0:
                    Tlosses = f.Invoke(0, 1);
                    break;
            }

            return tP == 1;
        }

        public override List<Player> SortTable(List<Player> unsorted)
        {
            List<Player> t = unsorted.OrderByDescending(x => x.IsInCut).ThenByDescending(x => x.TournamentPoints)
                .ThenByDescending(x => x.MarginOfVictory).ThenByDescending(x => x.StrengthOfSchedule)
                .ThenByDescending(x => x.Order).ToList<Player>();
            for (int i = 0; i < t.Count; i++)
                t[i].Rank = i + 1;
            return t;
        }
    }
}