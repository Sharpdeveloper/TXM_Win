using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXM.Core
{
    [Serializable]
    public enum Faction
    {
        Imperium,
        Rebels,
        Scum
    }

    [Serializable]
    public class Player
    {
        #region Player Informations
        public static int DropNr = 0;
        public string Name { get; set; }
        public string Forename { get; set; }
        public string Nickname { get; set; }
        public int TableNr { get; set; }
        public string DisplayName
        {
            get
            {
                if (Nickname != null && Nickname != "")
                    return Forename + " \"" + Nickname + "\"";
                else
                    return Forename + " " + Name.ToCharArray()[0] + "."; 
            }
        }
        public string Team { get; set; }
        public string City { get; private set; }
        public string SquadList { get; set; }
        public bool Disqualified
        {
            get
            {
                if (disqualified == null)
                    disqualified = false;
                return disqualified;
            }                
        }
        private bool disqualified;
        public bool Dropped
        {
            get
            {
                if (dropped == null)
                    dropped = false;
                return dropped;
            }
        }
        private bool dropped;
        public int DropPos
        {
            get
            {
                if (dropPos == 0)
                    dropPos = 999999999;
                return dropPos;
            }
        }
        private int dropPos = 999999999;
        public bool DefeatedAllInGroup
        {
            get
            {
                if (defeatedAllInGroup == null)
                    defeatedAllInGroup = false;
                return defeatedAllInGroup;
            }
            set
            {
                defeatedAllInGroup = value;
            }
        }
        private bool present;
        public bool Present
        {
            get
            {
                if (present == null)
                    present = false;
                return present;
            }
            set
            {
                present = value;
            }
        }
        private bool defeatedAllInGroup;
        private static int currentStartNr = 0;
        public int Order
        {
            get
            {
                Random r = new Random();
                return r.Next(0, 99999);
            }
        }

        #endregion

        #region Game Infomations
        public int Nr { get; set; }
        public int Wins { get;  set; }
        public int ModifiedWins { get;  set; }
        public int Looses { get;  set; }
        public int Draws { get;  set; }
        public int Points { get;  set; }
        public int PointsDestroyed { get;  set; }
        public int PointsLost { get;  set; }
        public int PointsOfEnemies { get;  set; }
        public int PointOfSquad { get; set; }
        public int MarginOfVictory { get;  set; }
        public Faction PlayersFaction { get; set; }
        public List<Result> Results { get; set; }
        #endregion

        #region Tournament Informations
        public List<Player> Enemies { get; private set; }
        public bool Freeticket { get; set; }
        public bool Paired { get; set; }
        public bool WonFreeticket { get; set; }
        #endregion

        #region T3 Informations
        public int T3ID { get; private set; }
        public int Rank { get; set; }
        public int ArmyRank { get; set; }
        public bool Payed { get; set; }
        public bool SquadListGiven { get; set; }
        #endregion

        #region Constructors
        public Player(Player p):this(p.Name, p.Forename, p.Nickname, p.Team,p.City,p.Wins,p.ModifiedWins,p.Looses,p.Draws,p.Points,p.PointsDestroyed,p.PointsLost,p.PointsOfEnemies,p.PointOfSquad,p.MarginOfVictory,p.PlayersFaction,p.Freeticket,p.Paired,p.WonFreeticket,p.T3ID,p.Rank,p.ArmyRank,p.Payed,p.SquadListGiven,p.Nr)
        {

        }

        public Player(string name, string forename, string nickname, string team, string city, int wins, int modifiedWins, int looses, int draws, int points, int pointsDestroyed, int pointsLost, int pointsOfEnemies,  int pointOfSquad, int marginOfVictory, Faction playersFaction, bool freeticket, bool paired, bool wonFreeticket, int t3ID, int rank, int armyRank, bool payed, bool squadListGiven, int nr = -1)
        {
            Name = name;
            Forename = forename;
            Nickname = nickname;
            Team = team;
            City = city;
            Wins = wins;
            ModifiedWins = modifiedWins;
            Looses = looses;
            Draws = draws;
            Points = points;
            PointsDestroyed = pointsDestroyed;
            PointsLost = pointsLost;
            PointsOfEnemies = pointsOfEnemies;
            PointOfSquad = pointOfSquad;
            MarginOfVictory = marginOfVictory;
            PlayersFaction = playersFaction;
            Freeticket = freeticket;
            Paired = paired;
            WonFreeticket = wonFreeticket;
            T3ID = t3ID;
            Rank = rank;
            ArmyRank = armyRank;
            Payed = payed;
            SquadListGiven = squadListGiven;
            if (nr == -1)
                Nr = ++currentStartNr;
            else
                Nr = nr;
            Enemies = new List<Player>();
            Results = new List<Result>();
        }

        public Player(int t3ID, string forename, string name, string nickname, string faction, string city, string team, bool payed, bool armylistgiven, int pointOfSquad = 100)
            : this(name, forename, nickname, team, city, 0, 0, 0, 0, 0, 0, 0, 0, pointOfSquad, 0, StringToFaction(faction), false, false, false, t3ID, 0, 0, payed, armylistgiven)
        { }

        public Player(string nickname, int pointOfSquad, Faction playersFaction)
            : this(0, "", "", nickname, FactionToString(playersFaction), "", "", false, false, pointOfSquad)
        { }

        public Player(string nickname)
            : this(nickname, 100, Faction.Imperium)
        { }
        #endregion

        #region Public Functions
        public bool HasPlayedVS(Player enemy)
        {
            if (Enemies == null)
                return false;
            foreach (Player enemie in Enemies)
            {
                if (enemie.Nr == enemy.Nr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasPlayedAndWonVS(Player enemy)
        {
            if (Enemies == null)
                return false;
            for(int i = 0; i < Enemies.Count; i++)
            {
                var enemie = Enemies[i];
                if (enemie.Nr == enemy.Nr)
                {
                    return Results[i].Destroyed - Results[i].Lost > 0;
                }
            }
            return false;
        }

        public void SumEnemiesStrength()
        {
            PointsOfEnemies = 0;
            for (int i = 0; i < Enemies.Count; i++)
            {
                PointsOfEnemies += GetEnemyStrength(i);
            }
        }

        public bool AddResult(Result result, bool update = false)
        {
            bool winner = false;
            if (Results == null)
                Results = new List<Result>();
            if (Freeticket)
            {
                MarginOfVictory += (int)(1.5 * result.MaxSquadPoints);

                Wins++;
                winner = true;

                Points ++;

                Enemies.Add(result.Enemy);

                Freeticket = false;
            }
            else if (result.FirstRound && WonFreeticket)
            {
                MarginOfVictory += 2 * result.MaxSquadPoints;

                Wins++;
                winner = true;

                Points ++;

            }
            else
            {
                int mov = result.MaxSquadPoints + CalcuateMarginOfVictory(result.Destroyed, result.Lost, result.Enemy.PointOfSquad, result.MaxSquadPoints);
                PointsDestroyed += result.Destroyed;
                PointsLost += result.Lost;
                MarginOfVictory += mov;

                if (result.Destroyed <= result.Lost)
                {
                    if (result.Destroyed == result.Lost && result.WinnerID == this.Nr)
                    {
                        Wins++;
                        Points++;
                    }
                    else
                        Looses++;
                }
                else
                {
                    Points ++;
                    Wins++;
                    winner = true;
                }
            }

            if (!update)
            {
                Results.Add(result);
                Enemies.Add(result.Enemy);
            }

            return winner;
        }

        public void AddLastEnemy(Player enemy)
        {
            Enemies.Add(enemy);

            SumEnemiesStrength();
        }

        public void Update(Result r, int round)
        {
            RemoveRound(round-1);

            //Add new Result:
            AddResult(r, true);
            Results[round - 1] = r;
        }

        public void RemoveLastResult()
        {
            RemoveRound(Results.Count - 1);
            Results.RemoveAt(Results.Count - 1);
        }

        public void RemoveRound(int round)
        {           
            Result result = Results[round];
            int mov = result.MaxSquadPoints + CalcuateMarginOfVictory(result.Destroyed, result.Lost, result.Enemy.PointOfSquad, result.MaxSquadPoints);
            PointsDestroyed -= result.Destroyed;
            PointsLost -= result.Lost;
            MarginOfVictory -= mov;

            if (result.Destroyed <= result.Lost)
            {
                if (result.Destroyed == result.Lost && result.WinnerID == this.Nr)
                {
                    Wins--;
                    Points--;
                }
                else
                    Looses--;
            }
            else
            {
                    Points --;
                    Wins--;
            }
            Enemies.RemoveAt(Enemies.Count - 1);
        }

        public void Disqualify()
        {
            disqualified = true;
            if (Nickname != null)
                Nickname += " <disqualifiziert>";
            else
                Name += " <disqualifiziert>";
        }
        #endregion

        #region Properties
        public int EnemyCount
        {
            get
            {
                return Enemies.Count;
            }
        }

        public string PrintName
        {
            get
            {
                return Forename + " " + Nickname;
            }
        }

        public string PlayersFactionAsString
        {
            get
            {
                if (PlayersFaction == Faction.Imperium)
                    return "Imperium";
                else if (PlayersFaction == Faction.Scum)
                    return "Abschaum und Kriminelle";
                else
                    return "Rebellen";
            }
        }
        #endregion

        #region Static Functions
        public bool Equals(Player other)
        {
            return Nr == other.Nr;
        }

        public static string FactionToString(Faction faction)
        {
            if (faction == Faction.Imperium)
                return "Imperium";
            else if (faction == Faction.Scum)
                return "Scum and Villainy";
            else
                return "Rebels";
        }

        public static Faction StringToFaction(string faction)
        {
            if (faction == "Imperium")
                return Faction.Imperium;
            else if (faction == "Scum and Villainy")
                return Faction.Scum;
            else
                return Faction.Rebels;
        }

        public static Faction StringToFactionLang(string faction, Language lang)
        {
            if (faction == lang.GetTranslation(StaticLanguage.Imperium))
                return Faction.Imperium;
            else if (faction == lang.GetTranslation(StaticLanguage.Scum))
                return Faction.Scum;
            else
                return Faction.Rebels;
        }
        public static string[] GetFactions()
        {
            return new string[] { "Imperium", "Rebels", "Scum and Villainy" };
        }
        #endregion

        #region private Functions
        private int GetEnemyStrength(int enemyNr)
        {
            if (enemyNr < 0)
                return 0;
            else
            {
                return Enemies[enemyNr].Points;
            }
        }

        private int CalcuateMarginOfVictory(int destroyed, int lost, int enemySquadPoints, int maxSquadPoints)
        {
            if (destroyed >= enemySquadPoints)
                return maxSquadPoints - lost;
            else
            {
                if (lost >= PointOfSquad)
                    lost = maxSquadPoints;
                return destroyed - lost;
            }
        }

        public void GetResults(Player p)
        {
            Draws = p.Draws;
            Looses = p.Looses;
            Wins = p.Wins;
            Points = p.Points;
            PointsDestroyed = p.PointsDestroyed;
            PointsLost = p.PointsLost;
            PointsOfEnemies = p.PointsOfEnemies;
            ModifiedWins = p.ModifiedWins;
            MarginOfVictory = p.MarginOfVictory;
        }
        #endregion

        public void Drop()
        {
            DropNr++;
            dropPos = DropNr;
            dropped = true;
            if (Nickname != null)
                Nickname += " <dropped>";
            else
                Name += " <dropped>";
        }
    }
}
