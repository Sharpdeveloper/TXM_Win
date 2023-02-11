using System;
using System.Linq;
using System.Text.Json;

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
                .Select(x => new JSONPlayer(x.DisplayName, "", x.SquadList, x.MarginOfVictory, x.TournamentPoints, x.StrengthOfSchedule, x.Dropped ? 0 : t.DisplayedRound, new Dictionary<string, int>() { { "swiss", x.Rank } })).ToArray();
            ;
			JSONRound[] rounds = t.Rounds
                .Where(x => x.RoundNo >= 1)
                .Select(x => new JSONRound("swiss", x.RoundNo, ((List<JSONMatch>)x.Pairings.
				Select(y => new JSONMatch(y.Player1Name, y.Player1Score, y.Player2Name.Contains("Bye") ? "bye" : "win", y.Player2Name.Contains("Bye") ? "" : y.Player2Name, y.Player2Name.Contains("Bye") ? 0 : y.Player2Score))).ToArray()
				)).ToArray();
            JSONTournament tournament = new JSONTournament(t.Name, players, rounds);

			return JsonSerializer.Serialize(tournament);
		}
	}
}

