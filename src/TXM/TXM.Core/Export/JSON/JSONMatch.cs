using System;
using System.Text.Json.Serialization;

namespace TXM.Core.Export.JSON
{
	public class JSONMatch
	{
        [JsonPropertyName("player1")]
        public string Player1 { get; set; }
        [JsonPropertyName("player1points")]
        public int Player1Points { get; set; }
        [JsonPropertyName("player2")]
        public string Player2 { get; set; }
        [JsonPropertyName("player2points")]
        public int Player2Points { get; set; }
        [JsonPropertyName("result")]
        public string Result { get; set; }

        public JSONMatch(string player1, int player1points, string result, string player2 = "", int player2points = 0)
            => (Player1, Player1Points, Result, Player2, Player2Points) = (player1, player1points, result, player2, player2points);
	}
}

