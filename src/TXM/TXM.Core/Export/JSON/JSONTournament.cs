using System;
using System.Text.Json.Serialization;

namespace TXM.Core.Export.JSON
{
	public class JSONTournament
	{
        [JsonPropertyName("name")]
		public string Name { get; set; }
        [JsonPropertyName("players")]
		public JSONPlayer[] Players { get; set; }
        [JsonPropertyName("rounds")]
		public JSONRound[] Rounds { get; set; }

		public JSONTournament(string name, JSONPlayer[] players, JSONRound[] rounds)
			=> (Name, Players, Rounds) = (name, players, rounds);
	}
}

