using System;

namespace TXM.Core;

/// <summary>
/// Tournament.Player.cs
/// In this file are all methods which manipulate only players bundled.
/// </summary>
public partial class Tournament
{
    #region public methods
    /// <summary>
    /// Disqualify a player
    /// </summary>
    /// <param name="player">The player who should disqualified</param>
    public void DisqualifyPlayer(Player player)
    {
        for (int i = 0; i < player.Enemies.Count; i++)
        {
            Enemy enemy = player.Enemies[i];
            Rule.Update(player, new Result(0, MaxPoints, enemy.ID, MaxPoints, enemy.ID), i + 1);
            Rule.Update(Participants.Where(x => x.ID == enemy.ID).First(), new Result(MaxPoints, 0, player.ID, MaxPoints, enemy.ID), i + 1);
        }
        foreach (Enemy enemy in player.Enemies)
        {
            CalculateStrengthOfSchedule(Participants.Where(x => x.ID == enemy.ID).First());
            CalculateExtendedStrengthOfSchedule(Participants.Where(x => x.ID == enemy.ID).First());
        }
        CalculateStrengthOfSchedule(player);
        CalculateExtendedStrengthOfSchedule(player);

        player.IsDisqualified = true;
        if (player.Nickname != null)
        {
            player.Nickname += $" <{Texts.Disqualified}>";
        }
        else
        {
            player.Name += $" <{Texts.Disqualified}>";
        }
    }

    /// <summary>
    /// Drops a player
    /// </summary>
    /// <param name="player">the player who should be dropped</param>
    public void DropPlayer(Player player)
    {
        player.HasDropped = true;
        if (player.Nickname != null)
        {
            player.Nickname += $" <{Texts.Dropped}>";
        }
        else
        {
            player.Name += $" <{Texts.Dropped}>";
        }
    }
    #endregion

    #region private methods
    /// <summary>
    /// Calculate the Strength of Schedule for a given Player
    /// </summary>
    /// <param name="player">The player for who the SoS should be calculated</param>
    private void CalculateStrengthOfSchedule(Player player)
    {
        double sos = 0.0;

        foreach (Enemy enemy in player.Enemies)
        {
            if (enemy.ID >= 0)
            {
                Player enemyPlayer = Participants.Where(x => x.ID == enemy.ID).First();
                sos += (enemyPlayer.TournamentPoints / enemyPlayer.Enemies.Count);
            }
        }

        player.StrengthOfSchedule = sos;
    }

    /// <summary>
    /// Calculate the extended Strength of Schedule for a given Player
    /// </summary>
    /// <param name="player">The player for who the eSoS should be calculated</param>
    private void CalculateExtendedStrengthOfSchedule(Player player)
    {
        double esos = 0.0;

        foreach (Enemy enemy in player.Enemies)
        {
            if (enemy.ID >= 0)
            {
                Player enemyPlayer = Participants.Where(x => x.ID == enemy.ID).First();
                esos += (enemyPlayer.StrengthOfSchedule / enemyPlayer.Enemies.Count);
            }
        }

        player.ExtendedStrengthOfSchedule = esos;
    }
    #endregion
}