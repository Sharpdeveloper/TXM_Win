using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TXM.Core;

namespace TXMTest
{
    class Program
    {

        static void Main(string[] args)
        {
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.txt");
            int rounds = 4;

            if (!File.Exists(file))
            {
                File.Create(file);
            }

            Pairing p = new Pairing();
            Tournament2 xt = new Tournament2("Test");
            xt.MaxSquadPoints = 100;
            Player xp = new Player("TKundNobody", 100, Faction.Rebels);
            xt.AddPlayer(xp);
            xp = new Player("Tesdeor", 100, Faction.Imperium);
            xt.AddPlayer(xp);
            xp = new Player("Psychohomer", 100, Faction.Imperium);
            xt.AddPlayer(xp);
            xp = new Player("Darth Bane", 98, Faction.Imperium);
            xp.WonFreeticket = true;
            xt.AddPlayer(xp);
            xp = new Player("D.J.", 100, Faction.Rebels);
            xt.AddPlayer(xp);
            xp = new Player("CountGermaine", 98, Faction.Rebels);
            xt.AddPlayer(xp);
            p.Player1 = xp;
            xp = new Player("Hui Buh", 100, Faction.Rebels);
            xt.AddPlayer(xp);
            p.Player2 = xp;
            xp = new Player("TheApprentice", 100, Faction.Imperium);
            xt.AddPlayer(xp);
            xp = new Player("Sarge", 99, Faction.Imperium);
            xp.Team = "MER";
            xt.AddPlayer(xp);
            xp = new Player("Ruskal", 99, Faction.Imperium);
            xp.Team = "MER";
            xt.AddPlayer(xp);
            xp = new Player("Farlander", 96, Faction.Imperium);
            xp.Team = "MER";
            xt.AddPlayer(xp);
            xp = new Player("Quexxes", 100, Faction.Imperium);
            xp.Team = "MER";
            xt.AddPlayer(xp);

            List<Pairing> lp = new List<Pairing>();
            lp.Add(p);
            xt.PrePaired = lp;

            TestTournament(xt, file, rounds);

            Console.WriteLine("Test komplett");
            Console.Read();
        }

        private static void TestTournament(Tournament2 t, string file, int rounds)
        {
            Random rnd = new Random();
            WriteTournamentStart(t, file);
            List<Pairing> p;
            p = t.GetSeed(true, false);
            foreach (Pairing pa in p)
            {
                pa.Player1Score = rnd.Next(12, 101);
                pa.Player2Score = rnd.Next(12, 101);
            }
            WritePairing(p, file, 1);
            t.GetResults(p);
            t.Sort();
            WriteTable(t, file, 1);

            Pairing.ResetTableNr();
            for (int i = 1; i < rounds; i++)
            {
                p = t.GetSeed(false, false);
                foreach (Pairing pa in p)
                {
                    pa.Player1Score = rnd.Next(12, 101);
                    pa.Player2Score = rnd.Next(12, 101);
                }
                WritePairing(p, file, i + 1);
                t.GetResults(p);
                if (i == rounds - 1)
                {
                    foreach (Player player in t.Participants)
                    {
                        if (player.WonFreeticket)
                        {
                            player.AddLastEnemy(t.GetStrongestUnplayedEnemy(player));
                        }
                    }
                }
                t.Sort();
                WriteTable(t, file, i + 1);
            }
            Pairing.ResetTableNr();
        }

        private static void WriteTable(Tournament2 t, string file, int round)
        {
            using (System.IO.StreamWriter f = new System.IO.StreamWriter(file, true))
            {
                f.WriteLine("---Atkuelle Tabelle nach der " + round + ". Runde---");
                f.WriteLine("Nr. Nickname   Punkte S MS U N  HdS Punkte der Gegner");
                foreach (Player player in t.Participants)
                {
                    f.WriteLine(string.Format("{0,2:d}", player.Nr) + "  " + GetName(player.Nickname) + "   " + string.Format("{0,2:d}", player.Points) + "   " + string.Format("{0,1:d}", player.Wins) + " " + string.Format("{0,1:d}", player.ModifiedWins) + "  " + string.Format("{0,1:d}", player.Draws) + " " + string.Format("{0,1:d}", player.Looses) + " " + string.Format("{0,4:d}", ((Player)player).MarginOfVictory) + "    " + string.Format("{0,4:d}", ((Player)player).PointsOfEnemies));
                }
                f.WriteLine();
            }
        }

        private static void WriteTournamentStart(Tournament2 t, string file)
        {
            using (System.IO.StreamWriter f = new System.IO.StreamWriter(file))
            {
                f.WriteLine("---Neues Turnier---");
                f.WriteLine("Nr. Nickname   Squad Team");
                foreach (Player player in t.Participants)
                {
                    f.WriteLine(string.Format("{0,2:d}", player.Nr) + "  " + GetName(player.Nickname) + "   " + string.Format("{0,3:d}", ((Player)player).PointOfSquad) + "  " + player.Team);
                }
                f.WriteLine();
            }

        }

        private static void WritePairing(List<Pairing> p, string file, int count)
        {
            using (System.IO.StreamWriter f = new System.IO.StreamWriter(file, true))
            {
                f.WriteLine("---" + count + ". Paarung---");
                f.WriteLine("Nr. Spieler 1  Spieler 2  P(S1)  P(S2)");
                foreach (Pairing pairing in p)
                {
                    f.WriteLine(string.Format("{0,2:d}", pairing.TableNr) + "  " + GetName(pairing.Player1Name) + " " + GetName(pairing.Player2Name) + " " + string.Format("{0,3:d}", pairing.Player1Score) + "    " + string.Format("{0,3:d}", pairing.Player2Score));
                }
                f.WriteLine();
            }
        }

        private static string GetName(string n)
        {
            if (n.Length != 10)
            {
                if (n.Length < 10)
                {
                    while (n.Length < 10)
                    {
                        n += " ";
                    }
                }
                else
                    n = n.Substring(0, 10);
            }

            return n;
        }

    }
}
