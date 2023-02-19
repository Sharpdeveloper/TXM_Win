using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace TXM.Core
{
    public partial class Tournament
    {
        #region Constructors
        public Tournament(string name, int t3ID, string GOEPPversion, AbstractRules rules, bool firstround = true, int maxPoints = 100, TournamentCut cut = TournamentCut.NoCut)
        {
            Player.ResetID();
            Name = name;
            T3ID = t3ID;
            GOEPPVersion = GOEPPversion;
            Participants = new ObservableCollection<Player>();
            Nicknames = new List<string>();
            givenStartNo = new List<int>();
            FirstRound = firstround;
            MaxPoints = maxPoints;
            Cut = cut;
            CutStarted = false;
            WonByeCalculated = false;
            Pairings = new ObservableCollection<Pairing>();
            Rounds = new List<Round>();
            Rule = rules;
            Single = true;
            bonus = false;
        }
        public Tournament(int t3ID, string name, AbstractRules rules, string GOEPPversion = "")
            : this(name, t3ID, GOEPPversion, rules)
        { }
        public Tournament(string name, AbstractRules rules)
            : this(0, name, rules)
        { }

        public Tournament(string name, int maxPoints, TournamentCut cut, AbstractRules rules)
            : this(name, 0, "", rules, true, maxPoints, cut)
        { }
        #endregion

        #region public functions
        public Player GetPlayer(int id) => Participants.Where(x => x.ID == id).First();

        public void AddPlayer(Player player)
        {
            Participants.Add(player);
            Nicknames.Add(player.Nickname);
            if (Pairings.Count > 0)
            {
                if (Pairings[Pairings.Count - 1].Player2ID == Bye.ID)
                {
                    Pairings[Pairings.Count - 1].Player2 = player;
                    Pairings[Pairings.Count - 1].IsResultEdited = false;
                }
                else
                    Pairings.Add(new Pairing() { Player1 = player, IsResultEdited = true, Player2 = Bye });
            }
        }

        public void Sort()
        {
            Participants = Rule.SortTable(Participants);
        }

        internal void SplitTeams()
        {
            foreach (var tp in Teamplayer)
            {
                int a = 0;
                for (int i = 0; i < Participants.Count; i++)
                {
                    if (Participants[i].Team == tp.Team)
                    {
                        a = i;
                        break;
                    }
                }
                tp.Wins = Participants[a].Wins;
                tp.Rank = Participants[a].Rank;
                tp.ModifiedWins = Participants[a].ModifiedWins;
                tp.Losses = Participants[a].Losses;
                tp.Draws = Participants[a].Draws;
                tp.TournamentPoints = Participants[a].TournamentPoints;
                tp.DestroyedPoints = Participants[a].DestroyedPoints;
                tp.LostPoints = Participants[a].LostPoints;
                tp.StrengthOfSchedule = Participants[a].StrengthOfSchedule;
                tp.MarginOfVictory = Participants[a].MarginOfVictory;
            }
            Participants = Teamplayer;
        }

        public List<Pairing> GetSeed(bool start, bool cut)
        {
            if (Rule.UsesScenarios)
            {
                if (ChoosenScenario == "Random")
                {
                    Random random = new Random();
                    int temp = random.Next(1, ActiveScenarios.Count);
                    ChoosenScenario = ActiveScenarios[temp];
                    ActiveScenario = ChoosenScenario;
                }

                ActiveScenarios.Remove(ChoosenScenario);

                if (ActiveScenarios == null || ActiveScenarios.Count == 1)
                {
                    ResetScenarios();
                }
            }
            //if (Rule.IsRandomSeeding)
            //{
            //    return GetSeedRandom(start, cut);
            //}
            //else
            //{
            //    return GetSeedNonRandom(start, cut);
            //}
            return new List<Pairing>();
        }

        
        //public List<Pairing> GetSeedRandom(bool start, bool cut)
        //{
        //    Pairing.ResetTableNo();
        //    int temp, pos = 0;
        //    bool swappedGroup = false;

        //    #region Cut
        //    if (Cut != TournamentCut.NoCut && (cut || CutStarted))
        //    {
        //        CalculateWonBye();
        //        if (cut)
        //        {
        //            Sort();
        //            CutStarted = true;
        //            if (Cut == TournamentCut.Top8)
        //                currentCountOfPlayer = 8;
        //            else if (Cut == TournamentCut.Top16)
        //                currentCountOfPlayer = 16;
        //            else if (Cut == TournamentCut.Top32)
        //                currentCountOfPlayer = 32;
        //            else if (Cut == TournamentCut.Top64)
        //                currentCountOfPlayer = 64;
        //            else
        //                currentCountOfPlayer = 4;

        //            ListOfPlayers = new List<Player>();

        //            for (int i = 0; i < currentCountOfPlayer; i++)
        //                ListOfPlayers.Add(Participants[i]);

        //            Pairings = new List<Pairing>();

        //            while (ListOfPlayers.Count > 0)
        //            {
        //                Pairings.Add(new Pairing());
        //                Pairings[pos].Player1 = ListOfPlayers[0];
        //                Pairings[pos].Player2 = ListOfPlayers[ListOfPlayers.Count - 1];
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player2ID).First());
        //                pos++;
        //            }
        //        }
        //        else
        //        {
        //            Pairings = new List<Pairing>();

        //            for (int i = 0; i < WinnerLastRound.Count / 2; i++)
        //            {
        //                Pairings.Add(new Pairing());
        //                Pairings[i].Player1 = WinnerLastRound[i];
        //                Pairings[i].Player2 = WinnerLastRound[WinnerLastRound.Count - 1 - i];
        //            }
        //        }
        //    }
        //    #endregion
        //    else
        //    {
        //        if (start)
        //            Start();
        //        else
        //        {
        //            Sort();
        //            Rounds[Rounds.Count - 1].Participants = new List<Player>();
        //            foreach (Player p in Participants)
        //                Rounds[Rounds.Count - 1].Participants.Add(new Player(p));

        //        }

        //        ListOfPlayers = new List<Player>();
        //        Random random = new Random();

        //        foreach (Player p in Participants)
        //        {
        //            if (!(p.IsDisqualified || p.HasDropped))
        //                ListOfPlayers.Add(p);
        //        }

        //        Pairings = new List<Pairing>();

        //        if (start)
        //        {
        //            if (PrePaired != null)
        //            {
        //                foreach (Pairing p in PrePaired)
        //                {
        //                    Pairings.Add(p);
        //                    ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == p.Player1ID).First());
        //                    ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == p.Player2ID).First());
        //                    if (Participants.Where(x => x.ID == p.Player1ID).First().HasBye)
        //                    {
        //                        p.ResultEdited = true;
        //                    }
        //                    pos++;
        //                }
        //            }
        //            List<Player> wonByes = GetWonByes();
        //            for (int i = 0; i < wonByes.Count; i++)
        //            {
        //                Pairings.Add(new Pairing());
        //                Pairings[pos].Player1 = wonByes[i];
        //                Pairings[pos].Player2 = WonBye;
        //                Pairings[pos].ResultEdited = true;
        //                pos++;
        //            }
        //            if (ListOfPlayers.Count % 2 == 0)
        //                bye = false;
        //            else
        //                bye = true;
        //        }
        //        else
        //        {
        //            if (ListOfPlayers.Count % 2 == 0)
        //                bye = false;
        //            else
        //                bye = true;
        //        }
        //        if (start)
        //        {
        //            while (ListOfPlayers.Count > 0)
        //            {
        //                Pairings.Add(new Pairing());
        //                if (ListOfPlayers.Count == 1)
        //                {
        //                    Pairings[pos].Player1 = ListOfPlayers[0];
        //                    Pairings[pos].Player2 = Bye;
        //                    Participants.Where(x => x.ID == Pairings[pos].Player1ID).First().HasBye = true;
        //                    Pairings[pos].ResultEdited = true;
        //                    ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                    pos++;
        //                    break;
        //                }
        //                Pairings[pos].Player1 = ListOfPlayers[0];
        //                for (int i = 0; i < ListOfPlayers.Count; i++)
        //                {
        //                    temp = random.Next(1, ListOfPlayers.Count);
        //                    if (TeamProtection && (Pairings[pos].Player1.Team != ListOfPlayers[temp].Team || Pairings[pos].Player1.Team == ""))
        //                    {
        //                        Pairings[pos].Player2 = ListOfPlayers[temp];
        //                        break;
        //                    }
        //                    else
        //                        Pairings[pos].Player2 = ListOfPlayers[temp];
        //                }
        //                if (Participants.Where(x => x.ID == (Pairings[pos].Player2ID) == null)
        //                    Pairings[pos].Player2 = ListOfPlayers[1];

        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player2ID).First());
        //                pos++;
        //            }
        //        }
        //        else
        //        {
        //            CountGroups();

        //            int group = 0;
        //            for (int i = 0; i < PointGroup.Length; i++)
        //            {
        //                while (PointGroup[i].Count >= 2)
        //                {
        //                    Pairings.Add(new Pairing());
        //                    if (swappedGroup)
        //                    {
        //                        temp = PointGroup[i].Count - 1;
        //                        swappedGroup = false;
        //                    }
        //                    else
        //                    {
        //                        temp = random.Next(0, PointGroup[i].Count);
        //                    }
        //                    Pairings[pos].Player1 = PointGroup[i][temp];
        //                    PointGroup[i].RemoveAt(temp);
        //                    temp = random.Next(0, PointGroup[i].Count);
        //                    group = -1;
        //                    for (int j = i; j < PointGroup.Length; j++)
        //                    {
        //                        if (!HasPlayedVsWholePointGroup((Player)Pairings[pos].Player1, j))
        //                        {
        //                            group = j;
        //                            break;
        //                        }
        //                    }
        //                    if (group != -1)
        //                    {
        //                        temp = random.Next(0, PointGroup[i].Count);
        //                        while (Pairings[pos].Player1.HasPlayedVS(PointGroup[group][temp].ID))
        //                        {
        //                            temp = random.Next(0, PointGroup[i].Count);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Was passiert wenn ein Spieler schon gegen alle gespielt hat?
        //                        group = i;
        //                    }
        //                    Pairings[pos].Player2 = PointGroup[group][temp];
        //                    PointGroup[group].RemoveAt(temp);
        //                    pos++;
        //                }
        //                if (PointGroup[i].Count == 1)
        //                {
        //                    if (PointGroup.Length == i + 1 && bye)
        //                    {
        //                        Pairings.Add(new Pairing());
        //                        Pairings[pos].Player1 = PointGroup[i][0];
        //                        Pairings[pos].Player2 = Bye;
        //                        Pairings[pos].Player1.HasBye = true;
        //                        Pairings[pos].ResultEdited = true;
        //                    }
        //                    else
        //                    {
        //                        PointGroup[i + 1].Add(PointGroup[i][0]);
        //                        swappedGroup = true;
        //                    }
        //                }
        //            }
        //        }

        //        //Prüfen, ob die letzten beiden schon gegeneinander gespielt haben
        //        temp = Pairings.Count - 1;

        //        if (Pairings[temp] == null)
        //            temp--;

        //        if (Pairings[temp].Player1.HasPlayedVS(Pairings[temp].Player2.ID) && Pairings[temp].Player1.Enemies.Count < Participants.Count)
        //        {
        //            for (int i = temp - 1; i >= 0; i--)
        //            {
        //                if (!Pairings[temp].Player1.HasPlayedVS(Pairings[i].Player1.ID) && !Pairings[temp].Player2.HasPlayedVS(Pairings[i].Player2.ID))
        //                {
        //                    ChangePairing(temp, 0, i, 0);
        //                    break;
        //                }
        //                if (!Pairings[temp].Player1.HasPlayedVS(Pairings[i].Player2.ID) && !Pairings[temp].Player2.HasPlayedVS(Pairings[i].Player1.ID))
        //                {
        //                    ChangePairing(temp, 0, i, 1);
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    if (Rounds == null)
        //        Rounds = new List<Round>();
        //    Rounds.Add(new Round(Rounds.Count + 1, Pairings, Participants, ChoosenScenario));
        //    DisplayedRound = Rounds.Count;
        //    CheckTableNr();

        //    return Pairings;
        //}

        //public List<Pairing> GetSeedNonRandom(bool start, bool cut)
        //{
        //    Pairing.ResetTableNr();
        //    int temp, pos = 0;

        //    if (Cut != TournamentCut.NoCut && (cut || CutStarted))
        //    {
        //        CalculateWonBye();
        //        if (cut)
        //        {
        //            Sort();
        //            CutStarted = true;
        //            if (Cut == TournamentCut.Top8)
        //                currentCountOfPlayer = 8;
        //            else if (Cut == TournamentCut.Top16)
        //                currentCountOfPlayer = 16;
        //            else
        //                currentCountOfPlayer = 4;

        //            ListOfPlayers = new List<Player>();

        //            for (int i = 0; i < currentCountOfPlayer; i++)
        //                ListOfPlayers.Add(Participants[i]);

        //            Pairings = new List<Pairing>();

        //            while (ListOfPlayers.Count > 0)
        //            {
        //                Pairings.Add(new Pairing());
        //                Pairings[pos].Player1 = ListOfPlayers[0];
        //                Pairings[pos].Player2 = ListOfPlayers[ListOfPlayers.Count - 1];
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player2ID).First());
        //                pos++;
        //            }
        //        }
        //        else
        //        {
        //            Pairings = new List<Pairing>();

        //            for (int i = 0; i < WinnerLastRound.Count / 2; i++)
        //            {
        //                Pairings.Add(new Pairing());
        //                Pairings[i].Player1 = WinnerLastRound[i];
        //                Pairings[i].Player2 = WinnerLastRound[WinnerLastRound.Count - 1 - i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (start)
        //            Start();
        //        else
        //        {
        //            Sort();
        //            Rounds[Rounds.Count - 1].Participants = new List<Player>();
        //            foreach (Player p in Participants)
        //                Rounds[Rounds.Count - 1].Participants.Add(new Player(p));

        //        }

        //        ListOfPlayers = new List<Player>();

        //        foreach (Player p in Participants)
        //        {
        //            if (!p.IsDisqualified)
        //                ListOfPlayers.Add(p);
        //        }

        //        Pairings = new List<Pairing>();

        //        if (start)
        //        {
        //            if (PrePaired != null)
        //            {
        //                foreach (Pairing p in PrePaired)
        //                {
        //                    Pairings.Add(p);
        //                    ListOfPlayers.Remove(p.Player1);
        //                    ListOfPlayers.Remove(p.Player2);
        //                    if (p.Player1.HasBye)
        //                        p.ResultEdited = true;
        //                    pos++;
        //                }
        //            }
        //            List<Player> wonFreeTickets = GetWonByes();
        //            for (int i = 0; i < wonFreeTickets.Count; i++)
        //            {
        //                Pairings.Add(new Pairing());
        //                Pairings[pos].Player1 = wonFreeTickets[i];
        //                Pairings[pos].Player2 = WonBye;
        //                Pairings[pos].ResultEdited = true;
        //                pos++;
        //            }
        //            if (ListOfPlayers.Count % 2 == 0)
        //                bye = false;
        //            else
        //                bye = true;
        //        }
        //        else
        //        {
        //            if (Participants.Count % 2 == 0)
        //                bye = false;
        //            else
        //                bye = true;
        //        }
        //        if (start)
        //        {
        //            Random random = new Random();
        //            while (ListOfPlayers.Count > 0)
        //            {
        //                Pairings.Add(new Pairing());
        //                if (ListOfPlayers.Count == 1)
        //                {
        //                    Pairings[pos].Player1 = ListOfPlayers[0];
        //                    Pairings[pos].Player2 = Bye;
        //                    Pairings[pos].Player1.HasBye = true;
        //                    Pairings[pos].ResultEdited = true;
        //                    ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                    pos++;
        //                    break;
        //                }
        //                Pairings[pos].Player1 = ListOfPlayers[0];

        //                for (int i = 0; i < ListOfPlayers.Count; i++)
        //                {
        //                    temp = random.Next(1, ListOfPlayers.Count);
        //                    if (TeamProtection && (Pairings[pos].Player1.Team != ListOfPlayers[temp].Team || Pairings[pos].Player1.Team == ""))
        //                    {
        //                        Pairings[pos].Player2 = ListOfPlayers[temp];
        //                        break;
        //                    }

        //                }

        //                if Participants.Where(x => x.ID == (Pairings[pos].Player2ID) == null)
        //                    Pairings[pos].Player2 = ListOfPlayers[1];

        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player2ID).First());
        //                pos++;
        //            }
        //        }
        //        else
        //        {
        //            while (ListOfPlayers.Count > 0)
        //            {
        //                Pairings.Add(new Pairing());
        //                if (ListOfPlayers.Count == 1)
        //                {
        //                    Pairings[pos].Player1 = ListOfPlayers[0];
        //                    Pairings[pos].Player2 = Bye;
        //                    Pairings[pos].Player1.HasBye = true;
        //                    Pairings[pos].ResultEdited = true;
        //                    ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                    pos++;
        //                    break;
        //                }
        //                Pairings[pos].Player1 = ListOfPlayers[0];
        //                for (int i = 1; i < ListOfPlayers.Count; i++)
        //                {
        //                    if (!Pairings[pos].Player1.HasPlayedVS(ListOfPlayers[i].ID))
        //                    {
        //                        Pairings[pos].Player2 = ListOfPlayers[i];
        //                        break;
        //                    }
        //                }
        //                if Participants.Where(x => x.ID == (Pairings[pos].Player2ID) == null)
        //                    Pairings[pos].Player2 = ListOfPlayers[1];

        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player1ID).First());
        //                ListOfPlayers.Remove(ListOfPlayers.Where(x => x.ID == Pairings[pos].Player2ID).First());
        //                pos++;
        //            }
        //        }

        //        //Prüfen, ob die letzten beiden schon gegeneinander gespielt haben
        //        temp = Pairings.Count - 1;

        //        if (Pairings[temp] == null)
        //            temp--;

        //        if (Pairings[temp].Player1.HasPlayedVS(Pairings[temp].Player2.ID) && Pairings[temp].Player1.Enemies.Count < Participants.Count)
        //        {
        //            for (int i = temp - 1; i >= 0; i--)
        //            {
        //                if (!Pairings[temp].Player1.HasPlayedVS(Pairings[i].Player1.ID) && !Pairings[temp].Player2.HasPlayedVS(Pairings[i].Player2.ID))
        //                {
        //                    ChangePairing(temp, 0, i, 0);
        //                    break;
        //                }
        //                if (!Pairings[temp].Player1.HasPlayedVS(Pairings[i].Player2.ID) && !Pairings[temp].Player2.HasPlayedVS(Pairings[i].Player1.ID))
        //                {
        //                    ChangePairing(temp, 0, i, 1);
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    if (Rounds == null)
        //        Rounds = new List<Round>();
        //    Rounds.Add(new Round(Rounds.Count + 1, Pairings, Participants, ChoosenScenario));
        //    DisplayedRound = Rounds.Count;

        //    return Pairings;
        //}

        //private void CheckTableNr()
        //{
        //    foreach (Pairing p in Pairings)
        //    {
        //        if (p.Player1.TableNo != 0 && p.Player1.HasBye == false && p.TableNo != p.Player1.TableNo)
        //        {
        //            if ((p.Player2.TableNo != 0 && p.Player1.TableNo < p.Player2.TableNo) || p.Player2.TableNo == 0)
        //            {
        //                ChangePairings(p.Player1.TableNo, p.TableNo);
        //            }
        //            else if (p.Player2.TableNo != 0 && p.Player1.TableNo > p.Player2.TableNo && p.Player2.TableNo != p.TableNo)
        //            {
        //                ChangePairings(p.Player2.TableNo, p.TableNo);
        //            }
        //        }
        //        else if (p.Player2.TableNo != 0)
        //        {
        //            ChangePairings(p.Player2.TableNo, p.TableNo);
        //        }
        //    }
        //}

        //private void ChangePairings(int a, int b)
        //{
        //    int tablenra = a;
        //    int tablenrb = b;
        //    a--;
        //    b--;
        //    if (a >= Pairings.Count)
        //    {
        //        a = Pairings.Count - 1;
        //    }
        //    if (b >= Pairings.Count)
        //    {
        //        b = Pairings.Count - 1;
        //    }

        //    Player a1 = Pairings[a].Player1;
        //    Player a2 = Pairings[a].Player2;
        //    bool result = Pairings[a].ResultEdited;
        //    Pairings[a].Player1 = Pairings[b].Player1;
        //    Pairings[a].Player2 = Pairings[b].Player2;
        //    Pairings[a].ResultEdited = Pairings[b].ResultEdited;
        //    Pairings[b].Player1 = a1;
        //    Pairings[b].Player2 = a2;
        //    Pairings[b].ResultEdited = result;
        //    Pairings[a].TableNo = tablenra;
        //    Pairings[b].TableNo = tablenrb;
        //}

        //public void GetResults(List<Pairing> results, bool update = false)
        //{
        //    Result r;
        //    int winnerID = 0;
        //    bool winner;
        //    WinnerLastRound = new List<Player>();
        //    if (bonus)
        //    {
        //        foreach (Pairing pairing in results)
        //        {
        //            r = new Result(0, 0, Bonus.ID, MaxPoints, 1, pairing.Player1Score);
        //            Rule.AddBonus(pairing.Player1, r);
        //        }
        //        return;
        //    }
        //    if (!update)
        //    {
        //        foreach (Pairing pairing in results)
        //        {
        //            if (pairing.Winner == "Player 1")
        //                winnerID = pairing.Player1.ID;
        //            else if (pairing.Winner == "Player 2")
        //                winnerID = pairing.Player2.ID;
        //            else
        //                winnerID = (pairing.Player1Score > pairing.Player2Score) ? pairing.Player1.ID : pairing.Player2.ID;
        //            r = new Result(pairing.Player1Score, pairing.Player2Score, pairing.Player2.ID, MaxPoints, winnerID, pairing.Player1Points);
        //            winner = Rule.AddResult(pairing.Player1, r);
        //            if (winner)
        //                WinnerLastRound.Add(pairing.Player1);
        //            if (pairing.Player2 != WonBye && pairing.Player2 != Bye)
        //            {
        //                r = new Result(pairing.Player2Score, pairing.Player1Score, pairing.Player1.ID, MaxPoints, winnerID, pairing.Player2Points);
        //                winner = Rule.AddResult(pairing.Player2, r);
        //                if (winner)
        //                    WinnerLastRound.Add(pairing.Player2);
        //            }
        //        }
        //        FirstRound = false;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < results.Count; i++)
        //        {
        //            if (results[i].ResultEdited)
        //            {
        //                if (results[i].Winner == "Player 1")
        //                    winnerID = results[i].Player1.ID;
        //                else if (results[i].Winner == "Player 2")
        //                    winnerID = results[i].Player2.ID;
        //                else
        //                    winnerID = (results[i].Player1Score > results[i].Player2Score) ? results[i].Player1.ID : results[i].Player2.ID;
        //                r = new Result(results[i].Player1Score, results[i].Player2Score, results[i].Player2.ID, MaxPoints, winnerID, results[i].Player1Points);
        //                Rule.Update(results[i].Player1, r, DisplayedRound);
        //                r = new Result(results[i].Player2Score, results[i].Player1Score, results[i].Player1.ID, MaxPoints, winnerID, results[i].Player2Points);
        //                Rule.Update(results[i].Player2, r, DisplayedRound);
        //            }
        //        }
        //    }
        //    foreach (Player player in Participants)
        //        CalculateStrengthOfSchedule(player);
        //    foreach (Player player in Participants)
        //        CalculateExtendedStrengthOfSchedule(player);
        //    for (int i = 0; i < results.Count; i++)
        //        results[i].ResultEdited = false;
        //    if (update)
        //        Rounds[DisplayedRound - 1].Pairings = results;
        //    else
        //        Rounds[Rounds.Count - 1].Pairings = results;
        //    PrePaired = new List<Pairing>();
        //    Sort();
        //}

        public void ChangePlayer(Player player)
        {
            Participants[GetIndexOf(player)] = (Player)player;
        }

        public Player GetStrongestUnplayedEnemy(Player player)
        {
            for (int i = Participants.Count - 1; i >= 0; i--)
            {
                if (!player.HasPlayedVS(Participants[i].ID))
                    return Participants[i];
            }
            return null;
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

        private int GetIndexOfInRound(Player player, int round)
        {
            for (int i = 0; i < Rounds[round].Participants.Count; i++)
            {
                if (Rounds[round].Participants[i].Equals(player))
                    return i;
            }
            return -1;
        }

        private void CountGroups()
        {
            List<int> v = new List<int>();
            foreach (Player player in ListOfPlayers)
            {
                if (!v.Contains(player.TournamentPoints))
                {
                    v.Add(player.TournamentPoints);
                }
            }
            PointGroup = new List<Player>[v.Count];
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
        }

        private bool HasPlayedVsWholePointGroup(Player Player, int groupNr)
        {
            Player enemy;
            for (int i = 0; i < PointGroup[groupNr].Count; i++)
            {
                enemy = PointGroup[groupNr][i];
                if (!Player.HasPlayedVS(enemy.ID) && !Player.Equals(enemy))
                    return false;
            }
            return true;
        }

        private void Start()
        {
            if (!Single)
            {
                Teamplayer = Participants;
                Participants = new ObservableCollection<Player>();
                foreach (var tp in Teamplayer)
                {
                    int a = -1;
                    for (int i = 0; i < Participants.Count; i++)
                    {
                        if (Participants[i].Team == tp.Team)
                        {
                            a = i;
                            break;
                        }
                    }
                    if (a == -1)
                    {
                        Participants.Add(new Player(tp.Team) { Team = tp.Team, Firstname = tp.Firstname, Faction = tp.Faction });
                    }
                    else
                        Participants[a].Firstname += ", " + tp.Firstname;
                }
            }
            WonByes = 0;
            foreach (Player p in Participants)
            {
                if (p.HasWonBye)
                    WonByes++;
            }
        }

        private List<Player> GetWonByes()
        {
            List<Player> result = new List<Player>();
            for (int i = 0; i < ListOfPlayers.Count; i++)
            {
                if (ListOfPlayers[i].HasWonBye)
                {
                    result.Add(ListOfPlayers[i]);
                    ListOfPlayers.RemoveAt(i);
                    //i--;
                }
            }
            return result;
        }

        //private void ChangePairing(int player1Game, int player1Pos, int player2Game, int player2Pos)
        //{
        //    int player1EnemyPos = (player1Pos == 0) ? 1 : 0;
        //    if (player1Game == player2Game)
        //        return;
        //    Player player2;
        //    if (player2Pos == 0)
        //    {
        //        player2 = Pairings[player2Game].Player1;
        //        if (player1EnemyPos == 0)
        //        {
        //            Pairings[player2Game].Player1 = Pairings[player1Game].Player1;
        //            Pairings[player1Game].Player1 = player2;
        //        }
        //        else
        //        {
        //            Pairings[player2Game].Player1 = Pairings[player1Game].Player2;
        //            Pairings[player1Game].Player2 = player2;
        //        }
        //    }
        //    else
        //    {
        //        player2 = Pairings[player2Game].Player2;
        //        if (player1EnemyPos == 0)
        //        {
        //            Pairings[player2Game].Player2 = Pairings[player1Game].Player1;
        //            Pairings[player1Game].Player1 = player2;
        //        }
        //        else
        //        {
        //            Pairings[player2Game].Player2 = Pairings[player1Game].Player2;
        //            Pairings[player1Game].Player2 = player2;
        //        }
        //    }
        //    Pairings[player1Game].Player1.HasBye = false;
        //    Pairings[player1Game].Player2.HasBye = false;
        //    Pairings[player2Game].Player1.HasBye = false;
        //    Pairings[player2Game].Player2.HasBye = false;
        //    Pairings[player1Game].ResultEdited = false;
        //    Pairings[player2Game].ResultEdited = false;
        //    if (Pairings[player1Game].Player1 == Bye)
        //    {
        //        Pairings[player1Game].Player2.HasBye = true;
        //        Pairings[player1Game].ResultEdited = true;
        //    }
        //    else if (Pairings[player1Game].Player2 == Bye)
        //    {
        //        Pairings[player1Game].Player1.HasBye = true;
        //        Pairings[player1Game].ResultEdited = true;
        //    }
        //    else if (Pairings[player2Game].Player1 == Bye)
        //    {
        //        Pairings[player2Game].Player2.HasBye = true;
        //        Pairings[player2Game].ResultEdited = true;
        //    }
        //    else if (Pairings[player2Game].Player2 == Bye)
        //    {
        //        Pairings[player2Game].Player1.HasBye = true;
        //        Pairings[player2Game].ResultEdited = true;
        //    }
        //}

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

        public void RemovePlayer(Player player)
        {
            Participants.Remove(player);
        }

        //TODO: Refactor Calculate Won Bye
        public void CalculateWonBye()
        {
            if (!WonByeCalculated)
            {
                foreach (Player player in (Participants))
                {
                    if (player.HasWonBye)
                    {
                        int enemyID = 0;
//             player.AddLastEnemy(GetStrongestUnplayedEnemy(player));
                        player.Enemies.Add(new Enemy(enemyID, true));
                        CalculateStrengthOfSchedule(player);
                        CalculateExtendedStrengthOfSchedule(player);
                    }
                }
            }
            WonByeCalculated = true;
        }

        public int GetIndexOfPlayer(Player p)
        {
            for (int i = 0; i < Participants.Count; i++)
            {
                if (Participants[i].ID == p.ID)
                    return i;
            }
            return 0;
        }

        public ObservableCollection<Pairing> GetBonusSeed()
        {
            Pairings = new ObservableCollection<Pairing>();

            foreach (var p in Participants)
            {
                Pairings.Add(new Pairing() { Player1 = p, Player2 = Bonus });
            }

            if (Rounds == null)
                Rounds = new List<Round>();
            Rounds.Add(new Round(Rounds.Count + 1, Pairings, Participants, ChoosenScenario));
            DisplayedRound = Rounds.Count;
            bonus = true;
            return Pairings;
        }

    }
}
