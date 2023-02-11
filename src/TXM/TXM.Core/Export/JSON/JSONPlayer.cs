using System;
using System.Text.Json.Serialization;

namespace TXM.Core.Export.JSON
{
	public class JSONPlayer
	{
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("list-id")]
        public string ListID { get; set; }
        [JsonPropertyName("mov")]
        public int MoV { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("sos")]
        public double SoS { get; set; }
        [JsonPropertyName("dropped")]
        public int Dropped { get; set; }
        [JsonPropertyName("rank")]
        public Dictionary<string, int> Rank {get; set;}

		public JSONPlayer(string name, string email, string listid, int mov, int score, double sos, int dropped, Dictionary<string, int> rank)
			=> (Name, Email, ListID, MoV, Score, SoS, Dropped, Rank) = (name, email, listid, mov, score, sos, dropped, rank);
	}
}

