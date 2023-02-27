using System.Text.Json.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core.Models;

public partial class Player : ObservableObject
{
    #region static
    internal static int currentID = 0;

    private static Player? bye = null;
    public static Player Bye
    {
        get
        {
            if (bye == null)
            {
                bye = new Player("Bye")
                {
                    ID = -1
                };
            }
            return bye;
        }
    }

    private static Player? wonBye = null;
    public static Player WonBye
    {
        get
        {
            if(wonBye == null)
            {
                wonBye = new Player("WonBye")
                {
                    ID = -2
                };
            }
            return wonBye;
        }
    }

    private static Player? bonus = null;
    public static Player Bonus
    {
        get
        {
            if (bonus == null)
            {
                bonus = new Player("Bonus")
                {
                    ID = -2
                };
            }
            return bonus;
        }
    }

    /// <summary>
    /// Resets the internal player ID which is used to give each player an unique ID
    /// </summary>
    public static void ResetID()
    {
        currentID = 0;
    }
    #endregion

    #region Player Information
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public string name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public string firstname;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public string nickname;

    [ObservableProperty]
    public int tableNo;

    [ObservableProperty]
    public string team;

    [ObservableProperty]
    public string city;

    [ObservableProperty]
    public string squadList;

    [ObservableProperty]
    public bool isDisqualified;

    [ObservableProperty]
    public bool hasDropped;

    [ObservableProperty]
    public bool isPresent;

    [JsonIgnore]
    public int Order
    {
        get
        {
            return new Random().Next(0, 99999);
        }
    }
    #endregion

    #region Game Infomation
    [ObservableProperty]
    public int iD;

    [ObservableProperty]
    public int wins;

    [ObservableProperty]
    public int modifiedWins;

    [ObservableProperty]
    public int losses;

    [ObservableProperty]
    public int modifiedLosses;

    [ObservableProperty]
    public int draws;

    [ObservableProperty]
    public int tournamentPoints;

    [ObservableProperty]
    public int destroyedPoints;

    [ObservableProperty]
    public int lostPoints;

    [ObservableProperty]
    public double strengthOfSchedule;

    [ObservableProperty]
    public double extendedStrengthOfSchedule;

    [ObservableProperty]
    public int marginOfVictory;

    [ObservableProperty]
    public string faction;

    public List<Result> Results { get; set; }
    #endregion

    #region Tournament Information
    public List<Enemy> Enemies { get; set; }

    [ObservableProperty]
    public bool hasBye;

    [ObservableProperty]
    public bool isPaired;

    [ObservableProperty]
    public bool hasWonBye;
    #endregion

    #region T3 Information
    public int T3ID { get; private set; }

    [ObservableProperty]
    public int rank;

    public int ArmyRank { get; set; }

    [ObservableProperty]
    public bool hasPaid;

    [ObservableProperty]
    public bool hasListGiven;
    #endregion

    #region Derived Information
    public string DisplayName
    {
        get
        {
            if (Nickname != "")
            {
                return Firstname + " \"" + Nickname + "\"";
            }
            else
            {
                return Firstname + " " + Name.ToCharArray()[0] + ".";
            }
        }
    }
    #endregion

    #region Constructors
    //Copy Constructor
    public Player(Player p) : this(p.Name, p.Firstname, p.Nickname, p.Team, p.City, p.Wins, p.ModifiedWins, p.Losses, p.Draws, p.TournamentPoints, p.DestroyedPoints, p.LostPoints, p.StrengthOfSchedule, p.MarginOfVictory, p.Faction, p.HasBye, p.IsPaired, p.HasWonBye, p.T3ID, p.Rank, p.ArmyRank, p.HasPaid, p.HasListGiven, p.ID)
    {

    }

    public Player(string name, string firstname, string nickname, string team, string city, int wins, int modifiedWins, int looses, int draws, int points, int pointsDestroyed, int pointsLost, double strengthOfSchedule, int marginOfVictory, string faction, bool hasBye, bool isPaired, bool hasWonBye, int t3ID, int rank, int armyRank, bool hasPaid, bool hasListGiven, int nr = -1)
    {
        Name = name;
        Firstname = firstname;
        Nickname = nickname;
        Team = team;
        City = city;
        Wins = wins;
        ModifiedWins = modifiedWins;
        Losses = looses;
        Draws = draws;
        TournamentPoints = points;
        DestroyedPoints = pointsDestroyed;
        LostPoints = pointsLost;
        StrengthOfSchedule = strengthOfSchedule;
        MarginOfVictory = marginOfVictory;
        Faction = faction;
        HasBye = hasBye;
        IsPaired = isPaired;
        HasWonBye = hasWonBye;
        T3ID = t3ID;
        Rank = rank;
        ArmyRank = armyRank;
        HasPaid = hasPaid;
        HasListGiven = hasListGiven;
        if (nr == -1)
        {
            ID = ++currentID;
        }
        else
        {
            ID = nr;
        }

        Enemies = new List<Enemy>();
        Results = new List<Result>();
    }

    public Player(int t3ID, string firstname, string name, string nickname, string faction, string city, string team, bool payed, bool armylistgiven)
        : this(name, firstname, nickname, team, city, 0, 0, 0, 0, 0, 0, 0, 0, 0, faction, false, false, false, t3ID, 0, 0, payed, armylistgiven)
    { }


    public Player(string nickname)
        : this(0, "", "", nickname, "", "", "", false, false)
    { }
    #endregion

    #region internal methods
    /// <summary>
    /// Checks if the player has played vs this Enemy
    /// </summary>
    /// <param name="enemyID">The Player.ID of the Enemy</param>
    /// <returns>Returns true if the player has played against this Enemy. Otherwise false.</returns>
    internal bool HasPlayedVS(int enemyID)
    {
        if (Enemies == null)
        {
            return false;
        }
        foreach (Enemy enemy in Enemies)
        {
            if (enemy.ID == enemyID)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the player has played vs this Enemy and won the match
    /// </summary>
    /// <param name="enemyID">The Player.ID of the Enemy</param>
    /// <returns>Returns true if the player has played against this Enemy and won the amtch. Otherwise false.</returns>
    internal bool HasPlayedAndWonVS(int enemyID)
    {
        if (Enemies == null)
        {
            return false;
        }
        foreach (Enemy enemy in Enemies)
        {
            if (enemy.ID == enemyID)
            {
                return enemy.WonAgainst;
            }
        }
        return false;
    }
    #endregion

    #region public methods
    /// <summary>
    /// Checks if 2 Player Objects are the same
    /// </summary>
    /// <param name="obj">the other object for the check</param>
    /// <returns>True if the objects are the same</returns>
    public override bool Equals(Object? obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return ID == ((Player) obj).ID;
        }
    }

    /// <summary>
    /// no change in this method
    /// </summary>
    /// <returns>base.GetHasCode()</returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}