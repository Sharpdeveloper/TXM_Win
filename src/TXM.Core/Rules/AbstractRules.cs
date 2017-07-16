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
            AndroidNetrunnerRules.GetRuleName(),
            GameOfThrones2ndRules.GetRuleName(),
            ArmadaRules.GetRuleName(),
            DestinyRules.GetRuleName(),
            IARules.GetRuleName(),
            SWLCGRules.GetRuleName(),
            XWingRules.GetRuleName(),
            RuneWars_Armada_Rules.GetRuleName(),
            RuneWars_XWING_Rules.GetRuleName()
        };
        private static string[] RuleNamesT3Able =
        {
            ArmadaRules.GetRuleName(),
            IARules.GetRuleName(),
            XWingRules.GetRuleName(),
            RuneWars_Armada_Rules.GetRuleName(),
            RuneWars_XWING_Rules.GetRuleName()
        };

        public static AbstractRules GetRule(string name)
        {
            if (name == XWingRules.GetRuleName())
                return new XWingRules();
            else if (name == DestinyRules.GetRuleName())
                return new DestinyRules();
            else if (name == IARules.GetRuleName())
                return new IARules();
            else if (name == ArmadaRules.GetRuleName())
                return new ArmadaRules();
            else if (name == RuneWars_Armada_Rules.GetRuleName())
                return new RuneWars_Armada_Rules();
            else if (name == RuneWars_XWING_Rules.GetRuleName())
                return new RuneWars_XWING_Rules();
            else if (name == SWLCGRules.GetRuleName())
                return new SWLCGRules();
            else if (name == GameOfThrones2ndRules.GetRuleName())
                return new GameOfThrones2ndRules();
            else if (name == AndroidNetrunnerRules.GetRuleName())
                return new AndroidNetrunnerRules();
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
            if(add)
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
