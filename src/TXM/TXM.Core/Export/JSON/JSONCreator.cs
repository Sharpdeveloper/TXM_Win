using System;
using System.Linq;
using System.Text.Json;
using System.Net.Http;

using TXM.Core.Logic;

namespace TXM.Core.Export.JSON
{
	public class JSONCreator
	{
		private static HttpClient client = new HttpClient()
		{
			BaseAddress = new Uri("https://www.pattern-analyzer.app/api/yasb/xws")
		};
		private JSONCreator()
		{
		}

		public static string TournamentToListFortress(Tournament t)
		{
			JSONPlayer[] players = t.Participants
                .Select(x => new JSONPlayer(x.DisplayName, "", ListLinkToJson(x.SquadList), x.MarginOfVictory, x.TournamentPoints, x.StrengthOfSchedule, x.Dropped ? 0 : t.DisplayedRound, new Dictionary<string, int>() { { "swiss", x.Rank } })).ToArray();
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

		private static string ListLinkToJson(string link)
		{
			var start = link.IndexOf('?');
			var list = link.Substring(start);
			var task = client.GetStringAsync(list);
			task.Wait();
			return task.Result;
		}
	}
}

