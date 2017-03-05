using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    [Serializable]
    public class Tournament
    {
        #region Tournament Information
        public List<Player> Participants { get; set; }
        public bool FirstRound { get; set; }
        public List<Pairing> PrePaired { get; set; }
        public string Name { get; set; }
        public Pairing[] Pairings { get; private set; }
        public List<string> Nicknames { get; private set; }
        public int MaxSquadPoints { get; set; }
        public List<Round> Rounds { get; private set; }
        public string FilePath { get; private set; }
        public string AutoSavePath { get; private set; }
        public int DisplayedRound { get; set; }
        public Statistic Statistics { get; set; }
        public TournamentCut Cut { get; set; }
        public bool CutStarted { get; private set; }
        #endregion

        #region GUI_State
        public bool ButtonGetResultState { get; set; }
        public bool ButtonNextRoundState { get; set; }
        public bool ButtonCutState { get; set; }
        #endregion

        #region T3 INformation
        public int T3ID { get; set; }
        public string GOEPPVersion { get; private set; }
        #endregion

        #region Private Fields
        internal int currentStartNr;
        internal static List<int> givenStartNr = new List<int>();
        private Player WonFreeTicket;
        private Player FreeTicket;
        internal List<Player>[] PointGroup;
        internal List<Player> ListOfPlayers;
        internal bool freeticket;
        internal int WonFreetickets;
        internal int currentCountOfPlayer;
        internal List<Player> WinnerLastRound;
        #endregion

        #region Constructors
        public Tournament(string name, int t3ID, string GOEPPversion, bool firstround = true, int maxSquadPoints = 100, TournamentCut cut = TournamentCut.NoCut)
        {
            Name = name;
            T3ID = t3ID;
            GOEPPVersion = GOEPPversion;
            currentStartNr = 0;
            Participants = new List<Player>();
            Nicknames = new List<string>();
            givenStartNr = new List<int>();
            FirstRound = firstround;
            FreeTicket = new Player("Freilos");
            WonFreeTicket = new Player("Gewonnenes Freilos");
            MaxSquadPoints = maxSquadPoints;
            Cut = cut;
            CutStarted = false;
        }
        public Tournament(int t3ID, string name, string GOEPPversion = "")
            : this(name, t3ID, GOEPPversion)
        { }
        public Tournament(string name)
            : this(0, name)
        { }
        public Tournament(string name, int maxSquadPoints, TournamentCut cut)
            : this(name, 0, "", true, maxSquadPoints, cut)
        { }
        #endregion

        #region public functions
        public void AddPlayer(Player player)
        {
            player.Nr = ++currentStartNr;
            Participants.Add(player);
            Nicknames.Add(player.Nickname);
        }
        public void AddInfos(string playerName, int points, string squadlist)
        {
            int playerNr = -1;
            for (int i = 0; i < Participants.Count; i++)
            {
                if (Participants[i].Nickname == playerName)
                {
                    playerNr = i;
                    break;
                }
            }
            if (playerNr >= 0)
            {
                Participants[playerNr].PointOfSquad = points;
                Participants[playerNr].SquadList = squadlist;
            }
        }

        public void Sort()
        {
            List<Player> t = Participants.OrderByDescending(x => x.Points).ThenByDescending(x => x.MarginOfVictory).ThenByDescending(x => x.PointsOfEnemies).ToList<Player>();
            Participants = new List<Player>();
            foreach (Player p in t)
                Participants.Add(p);
            for (int i = 0; i < Participants.Count; i++)
                Participants[i].Rank = i + 1;
        }

        public Pairing[] GetSeed(bool start, bool cut)
        {
            int temp, pos = 0;

            if (Cut != TournamentCut.NoCut && (cut || CutStarted))
            {
                if (cut)
                {
                    Sort();
                    CutStarted = true;
                    if (Cut == TournamentCut.Top8)
                        currentCountOfPlayer = 8;
                    else if (Cut == TournamentCut.Top16)
                        currentCountOfPlayer = 16;
                    else
                        currentCountOfPlayer = 4;

                    ListOfPlayers = new List<Player>();

                    for (int i = 0; i < currentCountOfPlayer; i++)
                        ListOfPlayers.Add(Participants[i]);

                    Pairings = new Pairing[currentCountOfPlayer / 2];

                    while (ListOfPlayers.Count > 0)
                    {
                        Pairings[pos] = new Pairing();
                        Pairings[pos].Player1 = ListOfPlayers[0];
                        Pairings[pos].Player2 = ListOfPlayers[ListOfPlayers.Count - 1];
                        ListOfPlayers.Remove(Pairings[pos].Player1);
                        ListOfPlayers.Remove(Pairings[pos].Player2);
                        pos++;
                    }
                }
                else
                {
                    currentCountOfPlayer /= 2;

                    Pairings = new Pairing[currentCountOfPlayer / 2];

                    for(int i = 0; i < WinnerLastRound.Count/2; i++)
                    {
                        Pairings[i] = new Pairing();
                        Pairings[i].Player1 = WinnerLastRound[i];
                        Pairings[i].Player2 = WinnerLastRound[WinnerLastRound.Count - 1 - i];
                    }
                }
            }
            else
            {
                if (start)
                    Start();
                else
                {
                    Sort();
                    Rounds[Rounds.Count - 1].Participants = Participants;
                }

                ListOfPlayers = new List<Player>();

                foreach (Player p in Participants)
                    ListOfPlayers.Add(p);

                if (start)
                {
                    int tmp = 0;
                    if (Participants.Count % 2 != 0 && WonFreetickets % 2 != 0)
                        tmp = -1;
                    if(PrePaired != null)
                    {
                        foreach(var pairing in PrePaired)
                        {
                            if(pairing.Player1.Freeticket == true)
                                tmp++;
                        }
                    }
                    Pairings = new Pairing[Participants.Count / 2 + Participants.Count % 2 + WonFreetickets / 2 + WonFreetickets % 2 + tmp];
                }
                else
                    Pairings = new Pairing[Participants.Count / 2 + Participants.Count % 2];

                for (int i = 0; i < Pairings.Length; i++)
                    Pairings[i] = new Pairing();

                if (start)
                {
                    if (PrePaired != null)
                    {
                        foreach (Pairing p in PrePaired)
                        {
                            Pairings[pos] = p;
                            ListOfPlayers.Remove(p.Player1);
                            ListOfPlayers.Remove(p.Player2);
                            if (p.Player1.Freeticket)
                                p.ResultEdited = true;
                            pos++;
                        }
                    }
                    List<Player> wonFreeTickets = GetWonFreetickets();
                    for (int i = 0; i < wonFreeTickets.Count; i++)
                    {
                        Pairings[pos] = new Pairing();
                        Pairings[pos].Player1 = wonFreeTickets[i];
                        Pairings[pos].Player2 = WonFreeTicket;
                        Pairings[pos].ResultEdited = true;
                        pos++;
                    }
                    if (ListOfPlayers.Count % 2 == 0)
                        freeticket = false;
                    else
                        freeticket = true;
                }
                else
                {
                    if (Participants.Count % 2 == 0)
                        freeticket = false;
                    else
                        freeticket = true;
                }
                if (start)
                {
                    Random random = new Random();
                    while (ListOfPlayers.Count > 0)
                    {
                        if (ListOfPlayers.Count == 1)
                        {
                            Pairings[pos].Player1 = ListOfPlayers[0];
                            Pairings[pos].Player2 = FreeTicket;
                            Pairings[pos].Player1.Freeticket = true;
                            Pairings[pos].ResultEdited = true;
                            ListOfPlayers.Remove(Pairings[pos].Player1);
                            pos++;
                            break;
                        }
                        Pairings[pos].Player1 = ListOfPlayers[0];
                        for (int i = 0; i < ListOfPlayers.Count; i++)
                        {
                            temp = random.Next(1, ListOfPlayers.Count);
                            if (Pairings[pos].Player1.Team != ListOfPlayers[temp].Team || Pairings[pos].Player1.Team == "")
                            {
                                Pairings[pos].Player2 = ListOfPlayers[temp];
                                break;
                            }
                        }
                        if (Pairings[pos].Player2 == null)
                            Pairings[pos].Player2 = ListOfPlayers[1];

                        ListOfPlayers.Remove(Pairings[pos].Player1);
                        ListOfPlayers.Remove(Pairings[pos].Player2);
                        pos++;
                    }
                }
                else
                {
                    while (ListOfPlayers.Count > 0)
                    {
                        if (ListOfPlayers.Count == 1)
                        {
                            Pairings[pos].Player1 = ListOfPlayers[0];
                            Pairings[pos].Player2 = FreeTicket;
                            Pairings[pos].Player1.Freeticket = true;
                            Pairings[pos].ResultEdited = true;
                            ListOfPlayers.Remove(Pairings[pos].Player1);
                            pos++;
                            break;
                        }
                        Pairings[pos].Player1 = ListOfPlayers[0];
                        for (int i = 1; i < ListOfPlayers.Count; i++)
                        {
                            if (!Pairings[pos].Player1.HasPlayedVS(ListOfPlayers[i]))
                            {
                                Pairings[pos].Player2 = ListOfPlayers[i];
                                break;
                            }
                        }
                        if (Pairings[pos].Player2 == null)
                            Pairings[pos].Player2 = ListOfPlayers[1];

                        ListOfPlayers.Remove(Pairings[pos].Player1);
                        ListOfPlayers.Remove(Pairings[pos].Player2);
                        pos++;
                    }
                }

                //Prüfen, ob die letzten beiden schon gegeneinander gespielt haben
                temp = Pairings.Length - 1;

                if (Pairings[temp] == null)
                    temp--;

                if (Pairings[temp].Player1.HasPlayedVS(Pairings[temp].Player2) && Pairings[temp].Player1.EnemyCount < Participants.Count)
                {
                    for (int i = temp - 1; i >= 0; i--)
                    {
                        if (!Pairings[temp].Player1.HasPlayedVS(Pairings[i].Player1) && !Pairings[temp].Player2.HasPlayedVS(Pairings[i].Player2))
                        {
                            ChangePairing(temp, 0, i, 0);
                            break;
                        }
                        if (!Pairings[temp].Player1.HasPlayedVS(Pairings[i].Player2) && !Pairings[temp].Player2.HasPlayedVS(Pairings[i].Player1))
                        {
                            ChangePairing(temp, 0, i, 1);
                            break;
                        }
                    }
                }
            }

            if (Rounds == null)
                Rounds = new List<Round>();
            Rounds.Add(new Round(Pairings, Participants));
            DisplayedRound = Rounds.Count;

            return Pairings;
        }

        public void GetResults(Pairing[] results, bool update = false)
        {
            Result r;
            bool winner;
            WinnerLastRound = new List<Player>();
            if (!update)
            {
                Pairing.ResetTableNr();
                foreach (Pairing pairing in results)
                {
                    r = new Result(pairing.Player1Score, pairing.Player2Score, pairing.Player2, FirstRound, MaxSquadPoints,1);
                    winner = pairing.Player1.AddResult(r);
                    if (winner)
                        WinnerLastRound.Add(pairing.Player1);
                    if (pairing.Player2 != WonFreeTicket && pairing.Player2 != FreeTicket)
                    {
                        r = new Result(pairing.Player2Score, pairing.Player1Score, pairing.Player1, FirstRound, MaxSquadPoints,1);
                        winner = pairing.Player2.AddResult(r);
                        if (winner)
                            WinnerLastRound.Add(pairing.Player2);
                    }
                }
                FirstRound = false;
            }
            else
            {
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i].ResultEdited)
                    {
                        r = new Result(results[i].Player1Score, results[i].Player2Score, results[i].Player2, FirstRound, MaxSquadPoints,1);
                        results[i].Player1.Update(r, DisplayedRound);
                        r = new Result(results[i].Player2Score, results[i].Player1Score, results[i].Player1, FirstRound, MaxSquadPoints,1);
                        results[i].Player2.Update(r, DisplayedRound);
                    }
                }
            }
            foreach (Player player in Participants)
                player.SumEnemiesStrength();
            for (int i = 0; i < results.Length; i++)
                results[i].ResultEdited = false;
            if (update)
                Rounds[DisplayedRound - 1].Pairings = results;
            else
                Rounds[Rounds.Count - 1].Pairings = results;
        }

        public void ChangePlayer(Player player)
        {
            Participants[GetIndexOf(player)] = (Player)player;
        }

        public Player GetStrongestUnplayedEnemy(Player player)
        {
            for (int i = Participants.Count - 1; i >= 0; i--)
            {
                if (!player.HasPlayedVS(Participants[i]))
                    return Participants[i];
            }
            return null;
        }

        public void Save(string file, bool? buttonGetResults, bool? buttonNextRound, bool? buttonCut, string Autosavetype = "")
        {
            ButtonGetResultState = buttonGetResults == true;
            ButtonNextRoundState = buttonNextRound == true;
            ButtonCutState = buttonCut == true;
            if (file == "")
            {
                file = (new FileInfo(Assembly.GetEntryAssembly().Location)).DirectoryName + "\\Autosave";
                if (!Directory.Exists(file))
                {
                    Directory.CreateDirectory(file);
                }
                file += "\\Autosave_" + DateTime.Now.ToFileTime() + "_" + Name + "_" + Autosavetype + Settings.FILEEXTENSION;
            }

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, this);
            }
        }
        #endregion

        #region private functions
        private int GetIndexOf(Player player)
        {
            for (int i = 0; i < Participants.Count; i++)
            {
                if (Participants[i].Equals(player))
                    return i;
            }
            return -1;
        }

        private Player PlayerWithNr(int nr)
        {
            foreach (Player player in Participants)
            {
                if (player.Nr == nr)
                    return player;
            }
            return null;
        }

        private void Start()
        {
            WonFreetickets = 0;
            foreach (Player p in Participants)
            {
                if (p.WonFreeticket)
                    WonFreetickets++;
            }
        }

        private List<Player> GetWonFreetickets()
        {
            List<Player> result = new List<Player>();
            for (int i = 0; i < ListOfPlayers.Count; i++)
            {
                if (ListOfPlayers[i].WonFreeticket)
                {
                    result.Add(ListOfPlayers[i]);
                    ListOfPlayers.RemoveAt(i);
                    //i--;
                }
            }
            return result;
        }

        private bool HasPlayedVsWholePointGroup(Player Player, int groupNr)
        {
            Player enemy;
            for (int i = 0; i < PointGroup[groupNr].Count; i++)
            {
                enemy = PointGroup[groupNr][i];
                if (!Player.HasPlayedVS(enemy) && !Player.Equals(enemy))
                    return false;
            }
            return true;
        }

        private void ChangePairing(int player1Game, int player1Pos, int player2Game, int player2Pos)
        {
            int player1EnemyPos = (player1Pos == 0) ? 1 : 0;
            if (player1Game == player2Game)
                return;
            Player player2;
            if (player2Pos == 0)
            {
                player2 = Pairings[player2Game].Player1;
                if (player1EnemyPos == 0)
                {
                    Pairings[player2Game].Player1 = Pairings[player1Game].Player1;
                    Pairings[player1Game].Player1 = player2;
                }
                else
                {
                    Pairings[player2Game].Player1 = Pairings[player1Game].Player2;
                    Pairings[player1Game].Player2 = player2;
                }
            }
            else
            {
                player2 = Pairings[player2Game].Player2;
                if (player1EnemyPos == 0)
                {
                    Pairings[player2Game].Player2 = Pairings[player1Game].Player1;
                    Pairings[player1Game].Player1 = player2;
                }
                else
                {
                    Pairings[player2Game].Player2 = Pairings[player1Game].Player2;
                    Pairings[player1Game].Player2 = player2;
                }
            }
        }

        private string BoolToString(bool b)
        {
            return b ? "1" : "0";
        }
        #endregion

        #region static functions
        public static Tournament Load(string file)
        {
            Tournament t = null;
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                t = (Tournament)formatter.Deserialize(stream);
            }
            return t;
        }
        #endregion

        public void RemovePlayer(Player player)
        {
            Participants.Remove(player);
        }
    }
}
