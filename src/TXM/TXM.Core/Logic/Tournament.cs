using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Export;
using TXM.Core.Models;
using TXM.Core.Text;

namespace TXM.Core.Logic;

public partial class Tournament : ObservableObject
{
    #region Tournament Information

    public List<Pairing>? PrePaired { get; set; }

    [ObservableProperty]
    public ObservableCollection<Player> participants;

    [ObservableProperty]
    public string chosenScenario;

    private List<Player> winnerLastRound;
    public string Name { get; set; }
    public List<string> Nicknames { get; internal set; }
    public int MaxPoints { get; set; }
    public ObservableCollection<Round> Rounds { get; internal set; }
    public string FilePath { get; internal set; }
    public string AutoSavePath { get; internal set; }
    public string DisplayedRound { get; set; }
    public int CutSize { get; set; }
    public bool WonByeCalculated { get; internal set; }
    public bool TeamProtection { get; set; }

    [JsonConverter(typeof(RuleConverter))]
    public AbstractRules Rule
    {
        get { return rule; }
        set
        {
            rule = value;
            if (rule != null && rule.UsesScenarios)
            {
                ResetScenarios();
            }
        }
    }

    public List<string> ActiveScenarios { get; private set; }

    #endregion

    #region GUI_State

    public string ButtonGetResultsText { get; set; }
    public bool ButtonCutState { get; set; }

    #endregion

    #region T3 Information

    public int T3ID { get; set; }
    public string GOEPPVersion { get; internal set; }

    #endregion

    #region internal Fields

    internal static List<int> givenStartNo = new List<int>();
    public Player WonBye = Player.WonBye;
    public Player Bye = Player.Bye;
    private Player Bonus = Player.Bonus;
    internal int WonByes;
    internal int currentCountOfPlayer;
    internal List<Player> WinnerLastRound;
    internal bool bye;
    private AbstractRules rule;
    private int currentCutSize = -1;

    #endregion

    #region Constructors

    public Tournament(string name
        , int t3ID
        , string GOEPPversion
        , AbstractRules rules
        , bool firstround = true
        , int maxPoints = 100
        , int cut = 0)
    {
        Player.ResetID();
        Name = name;
        T3ID = t3ID;
        GOEPPVersion = GOEPPversion;
        Participants = new ObservableCollection<Player>();
        Nicknames = new List<string>();
        givenStartNo = new List<int>();
        MaxPoints = maxPoints;
        CutSize = cut;
        WonByeCalculated = false;
        Rounds = new ObservableCollection<Round>();
        Rule = rules;
    }

    public Tournament(int t3ID, string name, AbstractRules rules, string GOEPPversion = "")
        : this(name, t3ID, GOEPPversion, rules) { }

    public Tournament(string name, AbstractRules rules)
        : this(0, name, rules) { }

    public Tournament(string name, int maxPoints, int cut, AbstractRules rules)
        : this(name, 0, "", rules, true, maxPoints, cut) { }

    #endregion

    #region public methods

    /// <summary>
    /// Get the Player with given id.
    /// </summary>
    /// <param name="id">The id of the player which should be returned</param>
    /// <returns>The player with the ID</returns>
    public Player GetPlayerById(int id) => Enumerable.First<Player>(Participants, x => x.ID == id);

    /// <summary>
    /// Add a player to the tournament for begin and first round only
    /// </summary>
    /// <param name="player">The player that should be added</param>
    public void AddPlayer(Player player)
    {
        Participants.Add(player);
        Nicknames.Add(player.Nickname);
        if (Rounds.Count > 0 && Rounds[1].Pairings.Count > 0)
        {
            if (Rounds[1].Pairings[^1].Player2ID == Bye.ID)
            {
                Rounds[1].Pairings[^1].Player2 = player;
                Rounds[1].Pairings[^1].IsResultEdited = false;
            }
            else
            {
                Rounds[1].Pairings.Add(new Pairing() { Player1 = player, IsResultEdited = true, Player2 = Bye });
            }
        }
    }

    /// <summary>
    /// Sorts the player table
    /// </summary>
    public void Sort()
    {
        Participants = Rule.SortTable(Participants);
    }

    /// <summary>
    /// Start the tournament and pair the first round
    /// </summary>
    public void StartTournament()
    {
        PrePaired ??= new List<Pairing>();
        foreach (var p in Participants)
        {
            if (p.HasWonBye)
            {
                PrePaired.Add(new Pairing());
                PrePaired[^1].Player1 = p;
                PrePaired[^1].Player2 = WonBye;
                PrePaired[^1].IsResultEdited = true;
            }
        }

        SeedPlayers(new Random());
    }

    /// <summary>
    /// Remove the player from the tournament
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayer(Player player)
    {
        Participants.Remove(player);
    }

    /// <summary>
    /// Undo the last round
    /// </summary>
    public void RemoveLastRound()
    {
        foreach (Player p in Participants)
        {
            Rule.RemoveLastResult(p);
        }

        foreach (var p in Rounds[^1].Pairings)
        {
            p.Player1Score = 0;
            p.Player2Score = 0;
            if (GetPlayerById(p.Player1ID).HasBye || (GetPlayerById(p.Player1ID).HasWonBye && Rounds.Count == 2))
            {
                p.IsResultEdited = true;
            }
            else
            {
                p.IsResultEdited = false;
            }
        }
    }

    /// <summary>
    /// Disqualify a player
    /// </summary>
    /// <param name="player">The player who should disqualified</param>
    public void DisqualifyPlayer(Player player)
    {
        for (int i = 0; i < player.Enemies.Count; i++)
        {
            Enemy enemy = player.Enemies[i];
            Rule.Update(player, new Result(0, MaxPoints, enemy.ID, MaxPoints, enemy.ID), i);
            Rule.Update(Enumerable.Where<Player>(Participants, x => x.ID == enemy.ID).First()
                , new Result(MaxPoints, 0, player.ID, MaxPoints, enemy.ID), i);
        }

        foreach (Enemy enemy in player.Enemies)
        {
            CalculateStrengthOfSchedule(Enumerable.Where<Player>(Participants, x => x.ID == enemy.ID).First());
            CalculateExtendedStrengthOfSchedule(Enumerable.Where<Player>(Participants, x => x.ID == enemy.ID).First());
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


    /// <summary>
    /// Makes the cut Pairings
    /// </summary>
    public void OperateCut()
    {
        //TODO Double Elimination 
        var cutPlayer = new ObservableCollection<Player>();
        var Pairings = new ObservableCollection<Pairing>();
        var scenario = Rule.UsesScenarios ? GetScenario() : "";

        //calculate won byes => make ranking
        if (currentCutSize == -1)
        {
            currentCutSize = (int)Math.Ceiling(Math.Log2(CutSize));
            int margin = 0;
            for (int i = 0; i > CutSize; i++)
            {
                for (int j = margin; j > Participants.Count; j++)
                {
                    var p = Participants[i + j];
                    if (!(p.HasDropped || p.IsDisqualified))
                    {
                        cutPlayer.Add(p);
                        margin = j;
                        break;
                    }
                }
            }

            if (currentCutSize >= CutSize)
            {
                for (int i = 0; i > (currentCutSize - CutSize); i++)
                {
                    cutPlayer.Add(Bye);
                }
            }
        }
        else
        {
            currentCutSize /= 2;
            foreach (var player in winnerLastRound)
            {
                cutPlayer.Add(player);
            }
        }

        for (var i = 0; i < (currentCutSize / 2); i++)
        {
            var p = new Pairing
            {
                Player1 = cutPlayer[i], Player2 = cutPlayer[^(i + 1)]
            };
            Pairings.Add(p);
        }

        Rounds.Add(new Round($"Top {currentCutSize}", Pairings, scenario));

        DisplayedRound = Rounds[^1].RoundText;
    }

    /// <summary>
    /// Take the Results from the Pairings and calculate the values for the player
    /// </summary>
    public void GetResults()
    {
        var activeRound = Rounds.First(x => x.RoundText == DisplayedRound);
        var update = activeRound.RoundText != Rounds[^1].RoundText;
        var results = activeRound.Pairings;
        var roundIdx = Rounds.IndexOf(activeRound);
        Result r;
        int winnerID = 0;
        bool winner;
        winnerLastRound = new List<Player>();

        if (activeRound.RoundText == Literals.Bonus)
        {
            foreach (Pairing pairing in results)
            {
                r = new Result(0, 0, Bonus.ID, MaxPoints, 1, pairing.Player1Score);
                Rule.AddBonus(Enumerable.First<Player>(Participants, x => x.ID == pairing.Player1ID), r);
            }

            return;
        }

        foreach (var pairing in results)
        {
            var player1 = Enumerable.First<Player>(Participants, x => x.ID == pairing.Player1ID);
            var player2 = Enumerable.First<Player>(Participants, x => x.ID == pairing.Player2ID);
            Result diffResult;
            if (pairing.Winner == $"{Texts.Player} 1")
            {
                winnerID = pairing.Player1ID;
            }
            else if (pairing.Winner == $"{Texts.Player} 2")
            {
                winnerID = pairing.Player2ID;
            }
            else
            {
                winnerID = (pairing.Player1Score > pairing.Player2Score)
                    ? pairing.Player1ID
                    : pairing.Player2ID;
            }

            r = new Result(pairing.Player1Score, pairing.Player2Score, player2.ID, MaxPoints, winnerID
                , pairing.Player1Points);
            if (update)
            {
                //Only update if the Result was changed
                if (pairing.IsResultEdited)
                {
                    Rule.Update(player1, r, roundIdx + 1);
                }
            }
            else
            {
                winner = Rule.AddResult(player1, r);
                if (winner)
                {
                    WinnerLastRound.Add(Enumerable.First<Player>(Participants, x => x.ID == pairing.Player1ID));
                }
            }

            if (player2.ID != WonBye.ID && player2.ID != Bye.ID)
            {
                r = new Result(pairing.Player2Score, pairing.Player1Score, player1.ID, MaxPoints
                    , winnerID, pairing.Player2Points);
                if (update)
                {
                    //Only update if the Result was changed
                    if (pairing.IsResultEdited)
                    {
                        Rule.Update(player2, r, roundIdx + 1);
                    }
                }
                else
                {
                    winner = Rule.AddResult(player2, r);
                    if (winner)
                    {
                        WinnerLastRound.Add(Enumerable.First<Player>(Participants, x => x.ID == pairing.Player2ID));
                    }
                }
            }
        }

        foreach (var player in Participants)
        {
            CalculateStrengthOfSchedule(player);
        }

        foreach (var player in Participants)
        {
            CalculateExtendedStrengthOfSchedule(player);
        }

        Sort();

        PrePaired = new List<Pairing>();
    }

    /// <summary>
    /// Returns the Index of a player
    /// </summary>
    /// <param name="p">The player whose index is searched for</param>
    /// <returns>The index of p</returns>
    public int GetIndexOfPlayer(Player p)
    {
        return Participants.IndexOf(p);
    }

    /// <summary>
    /// THe WonBye gets calculated by treaten them as the strongest unplayed opponent
    /// </summary>
    public void CalculateWonBye()
    {
        if (!WonByeCalculated)
        {
            foreach (var player in Participants)
            {
                if (player.HasWonBye)
                {
                    player.Enemies.Add(new Enemy(GetStrongestUnplayedEnemy(player).ID, true));
                }
            }
        }

        foreach (var player in Participants)
        {
            CalculateStrengthOfSchedule(player);
        }

        foreach (var player in Participants)
        {
            CalculateExtendedStrengthOfSchedule(player);
        }

        WonByeCalculated = true;
    }

    /// <summary>
    /// This seed allows to give Bonus Points to player
    /// </summary>
    public void GetBonusSeed()
    {
        Pairing.ResetTableNo();
        Round round = new Round(Rounds.Count);
        var Pairings = new ObservableCollection<Pairing>();

        var ListOfPlayers = Enumerable.Where<Player>(Participants, p => !(p.IsDisqualified || p.HasDropped)).ToList();

        foreach (var p in ListOfPlayers)
        {
            Pairings.Add(new Pairing() { Player1 = p, Player2 = Bonus });
        }

        round.RoundText = Literals.Bonus;
        round.Pairings = Pairings;
        Rounds.Add(round);

        DisplayedRound = round.RoundText;
    }

    /// <summary>
    /// Seed the next Round
    /// </summary>
    public void NextRound()
    {
        PrePaired ??= new List<Pairing>();
        SeedPlayers(Rule.IsRandomSeeding ? new Random() : null);
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
                Player enemyPlayer = Enumerable.Where<Player>(Participants, x => x.ID == enemy.ID).First();
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
                Player enemyPlayer = Enumerable.Where<Player>(Participants, x => x.ID == enemy.ID).First();
                esos += (enemyPlayer.StrengthOfSchedule / enemyPlayer.Enemies.Count);
            }
        }

        player.ExtendedStrengthOfSchedule = esos;
    }

    /// <summary>
    /// Gets a new Scenario for the round
    /// </summary>
    /// <returns>the chosen Scenario</returns>
    private string GetScenario()
    {
        if (ChosenScenario == "Random")
        {
            Random random = new Random();
            int temp = random.Next(1, ActiveScenarios.Count);
            ChosenScenario = ActiveScenarios[temp];
        }

        var activeScenario = ChosenScenario;

        ActiveScenarios.Remove(ChosenScenario);

        if (ActiveScenarios.Count == 1)
        {
            ResetScenarios();
        }

        return activeScenario;
    }

    /// <summary>
    /// Pair the players for matches
    /// </summary>
    /// <param name="random">A new Random instance if random pairing ist needed</param>
    private void SeedPlayers(Random? random)
    {
        Pairing.ResetTableNo();
        Round round = new Round(Rounds.Count);
        List<Pairing> Pairings = new List<Pairing>();
        int temp, group = 0;
        bool swappedGroup = false;

        round.Scenario = Rule.UsesScenarios ? GetScenario() : "";

        var ListOfPlayers = Enumerable.Where<Player>(Participants, p => !(p.IsDisqualified || p.HasDropped)).ToList();

        if (PrePaired != null)
        {
            foreach (Pairing p in PrePaired)
            {
                Pairings.Add(p);
                ListOfPlayers.Remove(ListOfPlayers.First(x => x.ID == p.Player1ID));
                if (p.Player2ID >= 0)
                {
                    ListOfPlayers.Remove(ListOfPlayers.First(x => x.ID == p.Player2ID));
                }
                else
                {
                    p.IsResultEdited = true;
                }
            }
        }

        bye = ListOfPlayers.Count % 2 != 0;

        var PointGroup = CountGroups(ListOfPlayers);

        for (int i = 0; i < PointGroup.Length; i++)
        {
            while (PointGroup[i].Count >= 2)
            {
                Pairings.Add(new Pairing());

                //If a pointGroup has an uneven number of player the last player in that group
                //has to swap in the next group and has to be paired first
                if (swappedGroup)
                {
                    temp = PointGroup[i].Count - 1;
                    swappedGroup = false;
                }
                else
                {
                    temp = random?.Next(0, PointGroup[i].Count) ?? 0;
                }

                Pairings[^1].Player1 = PointGroup[i][temp];
                PointGroup[i].RemoveAt(temp);
                temp = random?.Next(0, PointGroup[i].Count) ?? 0;
                group = -1;
                for (int j = i; j < PointGroup.Length; j++)
                {
                    if (!HasPlayedVsWholePointGroup(Enumerable.First<Player>(Participants, x => x.ID == Pairings[^1].Player1ID)
                            , PointGroup[j]))
                    {
                        group = j;
                        break;
                    }
                }

                if (group != -1)
                {
                    temp = random?.Next(0, PointGroup[i].Count) ?? 0;
                    while (Enumerable.First<Player>(Participants, x => x.ID == Pairings[^1].Player1ID)
                           .HasPlayedVS(PointGroup[group][temp].ID))
                    {
                        temp = random?.Next(0, PointGroup[i].Count) ?? temp + 1;
                    }
                }
                else
                {
                    //What happens if a player has played against everyone in his and lower Pointgroups
                    group = i;
                }

                Pairings[^1].Player2 = PointGroup[group][temp];
                PointGroup[group].RemoveAt(temp);
            }

            if (PointGroup[i].Count == 1)
            {
                if (PointGroup.Length == i + 1 && bye)
                {
                    Pairings.Add(new Pairing());
                    Pairings[^1].Player1 = PointGroup[i][0];
                    Pairings[^1].Player2 = Bye;
                    Enumerable.First<Player>(Participants, x => x.ID == Pairings[^1].Player1ID).HasBye = true;
                    Pairings[^1].IsResultEdited = true;
                }
                else
                {
                    PointGroup[i + 1].Add(PointGroup[i][0]);
                    swappedGroup = true;
                }
            }
        }

        //Check if the last 2 have played against each other
        if (!bye)
        {
            var p1 = Enumerable.First<Player>(Participants, x => x.ID == Pairings[^1].Player1ID);
            var p2 = Enumerable.First<Player>(Participants, x => x.ID == Pairings[^1].Player2ID);
            Player enemyP1 = new Player("enemyP1"), enemyP2 = new Player("enemyP2");
            int pairingsNo = -1;

            if (p1.HasPlayedVS(Pairings[^1].Player2ID) &&
                p1.Enemies.Count < Participants.Count)
            {
                for (int i = Pairings.Count - 2; i >= 0; i--)
                {
                    if (!p1.HasPlayedVS(Pairings[i].Player1ID) &&
                        !p2.HasPlayedVS(Pairings[i].Player2ID))
                    {
                        pairingsNo = i;
                        enemyP1 = Enumerable.First<Player>(Participants, x => x.ID == Pairings[i].Player1ID);
                        enemyP2 = Enumerable.First<Player>(Participants, x => x.ID == Pairings[i].Player2ID);
                        break;
                    }

                    if (!p1.HasPlayedVS(Pairings[i].Player2ID) &&
                        !p2.HasPlayedVS(Pairings[i].Player1ID))
                    {
                        pairingsNo = i;
                        enemyP1 = Enumerable.First<Player>(Participants, x => x.ID == Pairings[i].Player2ID);
                        enemyP2 = Enumerable.First<Player>(Participants, x => x.ID == Pairings[i].Player1ID);
                        break;
                    }
                }

                //only if a swappable pair was found
                if (pairingsNo != -1)
                {
                    Pairings[^1].Player1 = p1;
                    Pairings[^1].Player2 = enemyP1;
                    Pairings[pairingsNo].Player1 = p2;
                    Pairings[pairingsNo].Player2 = enemyP2;
                }
            }
        }

        CheckTableNo(Pairings);

        foreach (var pairing in Pairings.OrderBy(x => x.TableNo).ToList())
        {
            round.Pairings.Add(pairing);
        }

        Rounds.Add(round);
        DisplayedRound = Rounds[^1].RoundText;
    }

    /// <summary>
    /// Checks if a player sits at his table, if not the pairings will be swapped
    /// </summary>
    /// <param name="Pairings">The list of pairings which should be checked</param>
    private void CheckTableNo(List<Pairing> Pairings)
    {
        foreach (Pairing p in Pairings)
        {
            var p1 = Enumerable.First<Player>(Participants, x => x.ID == p.Player1ID);
            var p2 = Enumerable.First<Player>(Participants, x => x.ID == p.Player2ID);
            if (p1.TableNo != 0 && p1.HasBye == false && p.TableNo != p1.TableNo)
            {
                if ((p2.TableNo != 0 && p1.TableNo < p2.TableNo) || p2.TableNo == 0)
                {
                    Pairing swapPairing = Pairings.First(x => x.TableNo == p1.TableNo);
                    swapPairing.TableNo = p.TableNo;
                    p.TableNo = p1.TableNo;
                }
                else if (p2.TableNo != 0 && p1.TableNo > p2.TableNo &&
                         p2.TableNo != p.TableNo)
                {
                    Pairing swapPairing = Pairings.First(x => x.TableNo == p2.TableNo);
                    swapPairing.TableNo = p.TableNo;
                    p.TableNo = p2.TableNo;
                }
            }
            else if (p2.TableNo != 0 && p.TableNo != p2.TableNo)
            {
                Pairing swapPairing = Pairings.First(x => x.TableNo == p2.TableNo);
                swapPairing.TableNo = p.TableNo;
                p.TableNo = p2.TableNo;
            }
        }
    }

    /// <summary>
    /// This methods splits the players in groups with the same points
    /// </summary>
    /// <param name="ListOfPlayers">The list of player that should be splitted</param>
    /// <returns>An array of player lists by group</returns>
    private List<Player>[] CountGroups(List<Player> ListOfPlayers)
    {
        List<int> v = new List<int>();
        foreach (Player player in ListOfPlayers)
        {
            if (!v.Contains(player.TournamentPoints))
            {
                v.Add(player.TournamentPoints);
            }
        }

        var PointGroup = new List<Player>[v.Count];
        v.OrderBy(x => x);
        for (int i = 0; i < v.Count; i++)
        {
            PointGroup[i] = new List<Player>();
            foreach (Player player in ListOfPlayers)
            {
                if (v[i] == player.TournamentPoints)
                {
                    PointGroup[i].Add(player);
                }
            }
        }

        return PointGroup;
    }

    /// <summary>
    /// Checks if the player has playeed and won vs all other players in his pointGroup
    /// </summary>
    /// <param name="Player">The player that should be checked</param>
    /// <param name="pointGroup">The whole pointGroup</param>
    /// <returns>True if the player has played and won vs the whole pointGroup otherwise false</returns>
    private bool HasPlayedVsWholePointGroup(Player Player, List<Player> pointGroup)
    {
        for (int i = 0; i < pointGroup.Count; i++)
        {
            var enemy = pointGroup[i];
            if (!Player.HasPlayedVS(enemy.ID) && !Player.Equals(enemy))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns the strongeest unplayed Opponent
    /// </summary>
    /// <param name="player">The player for which the strongest unplayed opponent needs to be find</param>
    /// <returns>Returns the strongest opponnent</returns>
    private Player GetStrongestUnplayedEnemy(Player player)
    {
        return Enumerable.First<Player>(Participants, opponent => player.ID != opponent.ID && !player.HasPlayedVS(opponent.ID));
    }

    /// <summary>
    /// Resets the Scenario List
    /// </summary>
    private void ResetScenarios()
    {
        ActiveScenarios = new List<string>();
        if (Rule.UsesScenarios)
        {
            ActiveScenarios.Add("Random");
            foreach (var s in Rule.Scenarios)
            {
                ActiveScenarios.Add(s);
            }
        }
    }

    #endregion
}