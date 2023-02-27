using System.Text.Json;

using TXM.Core.Logic;

namespace TXM.Core.Export.JSON
{
	public class JSONCreator
	{
		private JSONCreator()
		{
		}

		public static string TournamentToListFortress(Tournament t)
		{
			JSONPlayer[] players = t.Participants
                .Select(x => new JSONPlayer(x.DisplayName, "", x.SquadList, x.MarginOfVictory, x.TournamentPoints, x.StrengthOfSchedule, x.HasDropped ? 0 : t.Rounds.Count - 1, new Dictionary<string, int>() { { "swiss", x.Rank } })).ToArray();
			JSONRound[] rounds = t.Rounds
                .Where(x => x.RoundNo >= 1)
                .Select(x =>
                {
                    List<JSONMatch> jSONMatches = ((List<JSONMatch>)x.Pairings.
                                    Select(y => new JSONMatch(y.Player1Name, y.Player1Score, y.Player2Name.Contains("Bye") ? "bye" : "win", y.Player2Name.Contains("Bye") ? "" : y.Player2Name, y.Player2Name.Contains("Bye") ? 0 : y.Player2Score)).ToList());
                    return new JSONRound("swiss", x.RoundNo, jSONMatches.ToArray());
                }).ToArray();
            JSONTournament tournament = new JSONTournament(t.Name, players, rounds);

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(tournament, options);
		}
	}
}

