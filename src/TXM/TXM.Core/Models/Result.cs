using System;

namespace TXM.Core;

public class Result
{
    public int EnemyID { get; set; }
    public int Destroyed { get; set; }
    public int Lost { get; set; }
    public int MaxPoints { get; set; }
    public int WinnerID { get; set; }
    public int TournamentPoints { get; set; }

    public Result(int destroyed, int lost, int enemyID, int maxPoints, int winnerID, int torurnamentPoints = 0)
        => (EnemyID, Destroyed, Lost, MaxPoints, WinnerID, TournamentPoints) = (enemyID, lost, enemyID, maxPoints, winnerID, torurnamentPoints);
}

