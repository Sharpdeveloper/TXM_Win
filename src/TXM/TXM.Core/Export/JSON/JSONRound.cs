using System;
using System.Text.Json.Serialization;

namespace TXM.Core.Export.JSON
{
	public class JSONRound
	{
        [JsonPropertyName("round-type")]
        public string RoundType { get; set; }
        [JsonPropertyName("round-number")]
		public int RoundNumber { get; set; }
        [JsonPropertyName("matches")]
		public JSONMatch[] Matches { get; set; }

        public JSONRound(string roundType, int roundNumber, JSONMatch[] matches)
            => (RoundType, RoundNumber, Matches) = (roundType, roundNumber, matches);
	}
}

