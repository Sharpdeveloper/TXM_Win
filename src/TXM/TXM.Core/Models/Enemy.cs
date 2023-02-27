using System.Text.Json.Serialization;

namespace TXM.Core.Models;

public struct Enemy
{
    [JsonInclude]
    public int ID;
    [JsonInclude]
    public bool WonAgainst;

    public Enemy(int id, bool wonAgainst) => (ID, WonAgainst) = (id, wonAgainst);
}

