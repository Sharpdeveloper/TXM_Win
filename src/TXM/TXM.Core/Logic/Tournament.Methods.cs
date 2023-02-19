using System;

namespace TXM.Core;

/// <summary>
/// Tournament.Methods.cs
/// Collection of all Methods from the Tournament class
/// </summary>
public partial class Tournament
{
    #region public methods
    /// <summary>
    /// Undo the last round
    /// </summary>
    public void RemoveLastRound()
    {
        foreach (Player p in Participants)
        {
            Rule.RemoveLastResult(p);
        }
        foreach (var p in Pairings)
        {
            p.Player1Score = 0;
            p.Player2Score = 0;
            if (GetPlayer(p.Player1ID).HasBye || (GetPlayer(p.Player1ID).HasWonBye && FirstRound))
                p.IsResultEdited = true;
            else
                p.IsResultEdited = false;
        }
    }
    #endregion
}

