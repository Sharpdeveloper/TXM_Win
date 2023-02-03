using System;
using System.Collections.Generic;

namespace TXM.Core
{
    [Serializable]
    public abstract class AbstractRules
    {
        #region static
        private static string[] RuleNames =
        {
            GameOfThrones2ndRules.GetRuleName(),
            AndroidNetrunnerRules.GetRuleName(),
            InfinityRules.GetRuleName(),
            DiceThroneRules.GetRuleName(),
            LegendOfThe5RingesRules.GetRuleName(),
            RuneWarsRules.GetRuleName(),
            ArmadaRules.GetRuleName(),
            DestinyRules.GetRuleName(),
            IARules.GetRuleName(),
            LegionRules.GetRuleName(),
            SWLCGRules.GetRuleName(),
            XWingRules.GetRuleName(),
            XWing2Rules.GetRuleName(),
            XWing2LegacyRules.GetRuleName(),
            XWing25Rules.GetRuleName(),
            The9thAgeRules.GetRuleName(),
            W40KKPRules.GetRuleName(),
            W40KWLRules.GetRuleName(),
            WarmachineRules.GetRuleName()
        };
        private static string[] RuleNamesT3Able =
        {
            InfinityRules.GetRuleName(),
            RuneWarsRules.GetRuleName(),
            ArmadaRules.GetRuleName(),
            IARules.GetRuleName(),
            LegionRules.GetRuleName(),
            XWingRules.GetRuleName(),
            XWing2Rules.GetRuleName(),
            XWing2LegacyRules.GetRuleName(),
            XWing25Rules.GetRuleName(),
            The9thAgeRules.GetRuleName(),
            W40KKPRules.GetRuleName(),
            W40KWLRules.GetRuleName(),
            WarmachineRules.GetRuleName()
        };

        public static AbstractRules GetRule(string name)
        {
            if (name == XWingRules.GetRuleName())
                return new XWingRules();
            else if (name == DestinyRules.GetRuleName())
                return new DestinyRules();
            else if (name == IARules.GetRuleName())
                return new IARules();
            else if (name == InfinityRules.GetRuleName())
                return new InfinityRules();
            else if (name == ArmadaRules.GetRuleName())
                return new ArmadaRules();
            else if (name == RuneWarsRules.GetRuleName())
                return new RuneWarsRules();
            else if (name == SWLCGRules.GetRuleName())
                return new SWLCGRules();
            else if (name == GameOfThrones2ndRules.GetRuleName())
                return new GameOfThrones2ndRules();
            else if (name == AndroidNetrunnerRules.GetRuleName())
                return new AndroidNetrunnerRules();
            else if (name == LegionRules.GetRuleName())
                return new LegionRules();
            else if (name == LegendOfThe5RingesRules.GetRuleName())
                return new LegendOfThe5RingesRules();
            else if (name == W40KKPRules.GetRuleName())
                return new W40KKPRules();
            else if (name == W40KWLRules.GetRuleName())
                return new W40KWLRules();
            else if (name == XWing2Rules.GetRuleName())
                return new XWing2Rules();
            else if (name == XWing2LegacyRules.GetRuleName())
                return new XWing2LegacyRules();
            else if (name == XWing25Rules.GetRuleName())
                return new XWing25Rules();
            else if (name == The9thAgeRules.GetRuleName())
                return new The9thAgeRules();
            else if (name == DiceThroneRules.GetRuleName())
                return new DiceThroneRules();
            else if (name == WarmachineRules.GetRuleName())
                return new WarmachineRules();
            return null;
        }

        public static string[] GetAllRuleNames(bool T3Able = false)
        {
            if (T3Able)
            {
                return RuleNamesT3Able;
            }
            else
            {
                return RuleNames;
            }
        }
        #endregion

        #region protected
        protected int Twins;
        protected int TmodifiedWins;
        protected int Tdraws;
        protected int TmodifiedLosses;
        protected int Tlosses;
        protected int TtournamentPoints;
        protected int TdestroyedPoints;
        protected int TlostPoints;
        protected int TmarginOfVictory;
        protected string name;
        protected string movName = "MoV";
        protected bool tournamentPoints = false;
        protected bool hasScenarios = false;
        #endregion

        #region Public Properties
        public bool IsDrawPossible { get; protected set; }
        public List<string> OptionalFields { get; protected set; }
        public bool IsDoubleElimination { get; protected set; }
        public bool IsWinnerDropDownNeeded { get; protected set; }
        public bool IsRandomSeeding { get; protected set; }
        public int DefaultMaxPoints { get; protected set; }
        public string[] Factions { get; protected set; }
        public int DefaultTime { get; protected set; }
        public int DefaultRandomMins { get; protected set; } = 0;
        public string MoVName
        {
            get
            {
                return movName;
            }
        }
        public bool IsTournamentPointsInputNeeded
        {
            get
            {
                return tournamentPoints;
            }
        }

        public bool UsesScenarios
        {
            get
            {
                return hasScenarios;
            }
        }

        public string[] Scenarios { get; protected set; }
        #endregion

        #region Public Methods
        public bool AddResult(Player player, Result result)
        {
            player.Enemies.Add(result.Enemy);
            return ChangeResult(player, result, true);

        }

        public bool RemoveLastResult(Player player, Result result)
        {
            player.Enemies.RemoveAt(player.Enemies.Count - 1);
            player.Results.RemoveAt(player.Results.Count - 1);
            return ChangeResult(player, result, false);

        }

        public bool ChangeResult(Player player, Result result, bool add)
        {
            Init();

            if (player.Results == null)
                player.Results = new List<Result>();

            player.Results.Add(result);

            bool winner;
            if (add)
            {
                winner = CalculateResult(result, (x, y) => { return x + y; });
            }
            else
            {
                winner = CalculateResult(result, (x, y) => { return x - y; });
            }

            player.Wins += Twins;
            player.ModifiedWins += TmodifiedWins;
            player.Draws += Tdraws;
            player.ModifiedLosses += TmodifiedLosses;
            player.Losses += Tlosses;
            player.TournamentPoints += TtournamentPoints;
            player.DestroyedPoints += TdestroyedPoints;
            player.LostPoints += TlostPoints;
            player.MarginOfVictory += TmarginOfVictory;

            return winner;
        }

        public void AddBonus(Player player, Result result)
        {
            player.Enemies.Add(result.Enemy);
            player.TournamentPoints += result.TournamentPoints;
        }

        public void Update(Player player, Result result, int round)
        {
            ChangeResult(player, player.Results[round - 1], false);
            ChangeResult(player, result, true);
            player.Results[round - 1] = result;
        }

        public string GetName()
        {
            return name;
        }
        #endregion

        #region Private Function
        private void Init()
        {
            Twins = 0;
            TmodifiedWins = 0;
            Tdraws = 0;
            TmodifiedLosses = 0;
            Tlosses = 0;
            TtournamentPoints = 0;
            TdestroyedPoints = 0;
            TlostPoints = 0;
            TmarginOfVictory = 0;
        }
        #endregion

        #region Abstract Methods
        protected abstract bool CalculateResult(Result result, Func<int, int, int> f);
        public abstract List<Player> SortTable(List<Player> unsorted);
        #endregion
    }
}
