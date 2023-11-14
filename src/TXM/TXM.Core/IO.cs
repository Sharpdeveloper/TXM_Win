﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using TXM.Core.Export.JSON;
using TXM.Core.Logic;

namespace TXM.Core
{
    [Serializable]
    public class IO
    {
        private IFile fileManager;
        private IMessage messageManager;
        public string AutosavePath { get; private set; }
        public string TempPath { get; private set; }
        public string PrintFile { get; private set; }
        private string TextColorFile { get; set; }
        private string TextSizeFile { get; set; }
        private string BGImageFile { get; set; }
        public string SavePath { get; private set; }
        private string imgurl;
        public string TempImgPath { get; private set; }
        private int imgnr = 0;
        private string imgending;


        public IO(IFile filemanager, IMessage messagemanager)
        {
            fileManager = filemanager;
            messageManager = messagemanager;
            SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM");
            AutosavePath = Path.Combine(SavePath, "Autosave");
            TempPath = Path.Combine(SavePath, "Temp");
            PrintFile = Path.Combine(TempPath, "index.html");
            TextColorFile = Path.Combine(SavePath, "textcolor.txt");
            TextSizeFile = Path.Combine(SavePath, "textsize.txt");
            BGImageFile = Path.Combine(SavePath, "bgImage.txt");
            ReadBGImage();
        }

        public Tournament GOEPPImport()
        {
            fileManager.AddFilter("*.gip", "GÖPP Import File");
            if (fileManager.Open())
            {
                try
                {
                    List<string> gipFile = new List<string>();
                    using (StreamReader sr = new StreamReader(fileManager.FileName, Encoding.GetEncoding(28591)))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            gipFile.Add(line);
                        }
                    }
                    //Fileformat:
                    //1. Version:   v1.3.3
                    //2. Name:      1. Schlacht um Tatooine
                    //3. T3ID:      12484
                    //4. Number:    30
                    //              0     1          2     3          4         5            6                 7  8  9  10  11
                    //5.+ Player:   ID  ||Firstname||Name||Nickname ||Faction ||City       ||Team            ||A||B||C||D||x
                    //              7619||Dieter   ||Chri||DieChri  ||Rebellen||Leverkusen ||PGF Siegen e. V.||3|| ||1|| ||x
                    //A: ArmyListGiven 3 = Yes, 0 = No
                    //B: ?
                    //C: Paid 1 = Yes, 0 = No
                    //D: Actual T3 Rank
                    //x = End
                    Tournament tournament = new Tournament(Int32.Parse(gipFile[2]), gipFile[1], null, gipFile[0]);
                    for (int i = 4; i < gipFile.Count; i++)
                    {
                        tournament.AddPlayer(ConvertLineToPlayer(gipFile[i]));
                    }
                    //tournament.TeamProtection = false;
                    return tournament;
                }
                catch (Exception e)
                {
                    messageManager.Show("Please choose a valid gip-File.");
                    return null;
                }
            }
            return null;
        }

        private Player ConvertLineToPlayer(string line)
        {
            string[] splitedLine = SplitLine(line);
            string faction = splitedLine[4];
            if (faction == "Imperium")
                faction = "Imperial";
            else if (faction == "Abschaum und Kriminelle")
                faction = "Scum & Villainy";
            else
                faction = "Rebel";
            return new Player(Int32.Parse(splitedLine[0]), splitedLine[1], splitedLine[2], splitedLine[3], faction, splitedLine[5], splitedLine[6], Int32.Parse(splitedLine[9]) == 1, Int32.Parse(splitedLine[7]) == 3);
        }

        private string[] SplitLine(string line)
        {
            string[] splitedLine = new string[11];
            int sepBegin = 0, sepEnd;
            for (int i = 0; i < 11; i++)
            {
                sepEnd = line.IndexOf("|", sepBegin);
                splitedLine[i] = line.Substring(sepBegin, sepEnd - sepBegin);
                sepBegin = sepEnd + 2;
            }
            return splitedLine;
        }

        public void GOEPPExport(Tournament tournament)
        {
            fileManager.AddFilter("*.gep", "GÖPP Export File");
            if (!tournament.Single)
            {
                tournament.SplitTeams();
            }
            if (fileManager.Save())
            {
                string file = fileManager.FileName;
                List<string> temp = new List<string>();
                string lastname, city;
                //1. Version:   #GoePP-Exportdatei, v1.3.3 Export vom 19.08.2014||x
                //2. T3ID:      #TID-12484||x
                //3.+ Spieler:  T3ID ||Fore.||Name     ||Nick  ||Faction ||City          ||Team               ||A ||   B  ||C ||D ||E ||F ||G ||H ||I ||x
                //              22343||Chris||Chrissy  ||NiChri||Rebellen||Colonia       ||Expendable Squadron||15||131   ||32||-2||99||88||77||78||55||x
                //A: Rank
                //B: Points + ArmyRank
                //C: Points
                //D: Difference
                //E: ArmyRank
                //F: Painting
                //G: 
                //H: Fairplay + Fairplay Tournament
                //I: Other

                if (!file.EndsWith(".gep"))
                    file += ".gep";
                string line = "#GoePP-Exportdatei, " + tournament.GOEPPVersion + " Export vom " + DateTime.Now.ToShortDateString();
                string sep = "||", rest = sep + 0 + sep + 0 + sep + 0 + sep + 0;
                using (StreamWriter f = new StreamWriter(file, false, Encoding.GetEncoding(28591)))
                {
                    f.WriteLine(line + "||x");
                }
                line = "#TID-" + tournament.T3ID;
                WriteLine(file, line);
                foreach (Player xwp in tournament.Participants)
                {
                    if (temp.Contains(xwp.Nickname))
                        lastname = xwp.Name + " 1";
                    else
                        lastname = xwp.Name;
                    if (xwp.City.Length > 20)
                        city = xwp.City.Substring(0, 20);
                    else
                        city = xwp.City;
                    line = xwp.T3ID + sep + xwp.Firstname + sep + lastname + sep + xwp.Nickname + sep + xwp.Faction + sep + city + sep + xwp.Team + sep + xwp.Rank + sep + (xwp.TournamentPoints + xwp.ArmyRank) + sep + xwp.TournamentPoints + sep + xwp.MarginOfVictory + sep + xwp.ArmyRank + rest;
                    temp.Add(xwp.Nickname);
                    WriteLine(file, line);
                }
            }
        }

        public Tournament CSVImport()
        {
            fileManager.AddFilter("*.csv", "Excel File (CSV)");
            if (fileManager.Open())
            {
                try
                {
                    List<string> csvFile = new List<string>();
                    using (StreamReader sr = new StreamReader(fileManager.FileName))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            csvFile.Add(line);
                        }
                    }
                    //Fileformat:
                    //0          1         2        3    4       5    6          7       8  
                    //First Name;Last Name;Nickname;Team;Faction;Paid;List given;Won Bye;Squadlist
                    Tournament tournament = new Tournament(fileManager.FileName.Split(Path.DirectorySeparatorChar)[fileManager.FileName.Split(Path.DirectorySeparatorChar).Length - 1].Split('.')[0], null);
                    for (int i = 1; i < csvFile.Count; i++)
                    {
                        tournament.AddPlayer(ConvertCSVToPlayer(csvFile[i]));
                    }
                    tournament.TeamProtection = false;
                    return tournament;
                }
                catch (Exception)
                {
                    messageManager.Show("Please chosse a valid csv-File.");
                    return null;
                }
            }
            return null;
        }

        public void CSVImportAdd(Tournament tournament)
        {
            fileManager.AddFilter("*.csv", "Excel File (CSV)");
            if (fileManager.Open())
            {
                try
                {
                    List<string> csvFile = new List<string>();
                    using (StreamReader sr = new StreamReader(fileManager.FileName))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            csvFile.Add(line);
                        }
                    }
                    //Fileformat:
                    //0          1         2        3    4       5    6          7       8  
                    //First Name;Last Name;Nickname;Team;Faction;Paid;List given;Won Bye;Squadlist
                    for (int i = 1; i < csvFile.Count; i++)
                    {
                        var player = ConvertCSVToPlayer(csvFile[i]);
                        var p = (Player) tournament.Participants.Where(x => x.Nickname == player.Nickname).FirstOrDefault(); ;
                        if (p == null)
                        {
                            p = (Player) tournament.Participants.Where(x => x.Name == player.Name).Where(x => x.Firstname == player.Firstname).FirstOrDefault();
                        }
                        if(p == null)
                        {
                            tournament.AddPlayer(player);
                        }
                        else
                        {
                            p.WonBye = player.WonBye;
                            p.SquadList = player.SquadList;
                            p.ListGiven = player.ListGiven;
                            if(!p.ListGiven && p.SquadList != String.Empty)
                            {
                                p.ListGiven = true;
                            }
                        }
                        
                    }
                }
                catch (Exception)
                {
                    messageManager.Show("Please chosse a valid csv-File.");
                    return;
                }
            }
        }

        private Player ConvertCSVToPlayer(string line)
        {
            string[] splitedLine = line.Split(';');
            try
            {
                splitedLine[8].ToString();
            }
            catch (Exception)
            {
                splitedLine = line.Split(',');
            }
            return new Player(0, splitedLine[0], splitedLine[1], splitedLine[2], splitedLine[4], "", splitedLine[3], splitedLine[5].ToUpper() == "X", splitedLine[6].ToUpper() == "X") { WonBye = splitedLine[7].ToUpper() == "X", SquadList = splitedLine[8] };
        }

        private void WriteLine(string file, string line)
        {
            using (StreamWriter f = new StreamWriter(file, true, Encoding.GetEncoding(28591)))
            {
                f.WriteLine(line + "||x");
            }
        }

        private string WriteTable(Tournament tournament, bool bbcode, bool onlyNicknames, bool lists)
        {
            StringBuilder sb = new StringBuilder();
            string title = tournament.Name + " - Table - Round " + tournament.DisplayedRound;

            string head = "<!DOCTYPE html><meta charset=\"UTF-8\"><html><head><title>" + title + "</title></head><body><h2>" + title + "</h2> <br />";
            string tb = "<table>"; //Table begin
            string te = "</table>"; //Table end
            string rb = "<tr>"; //Table row begin
            string re = "</tr>"; //Table row end
            string db = "<td>"; //Table data begin
            string de = "</td>"; //Table data end
            string end = "</body></html>";
            string ab1 = "<a href=\"";
            string ab2 = "\">";
            string ae = "</a>";
            string nl = Environment.NewLine;

            if (bbcode)
            {
                head = "[b]" + title + "[/b]";
                tb = "[table]";
                te = "[/table]";
                rb = "[tr]";
                re = "[/tr]";
                db = "[td]";
                de = "[/td]";
                ab1 = "[url=";
                ab2 = "]";
                ae = "[/url]";
                end = nl;
            }

            sb.Append(head);

            //Added marquee to see all Pairings without manually scrolling
            //if(!bbcode)
            //{
            //    sb.Append("<marquee direction=\"up\" behavior=\"alternate\">");
            //}

            sb.Append(tb);
            sb.Append(rb);
            sb.Append(db);
            sb.Append("#"); //Rank
            sb.Append(de);
            if(!onlyNicknames)
            {
                sb.Append(db);
                sb.Append("Firstname"); //Firstname
                sb.Append(de);
            }
            sb.Append(db);
            sb.Append("Nickname"); //Nickname
            sb.Append(de);
            sb.Append(db);
            sb.Append("Team"); //Team
            sb.Append(de);
            sb.Append(db);
            sb.Append("Faction"); //Faction
            sb.Append(de);
            sb.Append(db);
            sb.Append("TP"); //Tournamentpoints
            sb.Append(de);
            sb.Append(db);
            sb.Append("W"); //Wins
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains("ModWins"))
            {
                sb.Append(db);
                sb.Append("MW"); //Modified Wins
                sb.Append(de);
            }
            if (tournament.Rule.OptionalFields.Contains("Draws"))
            {
                sb.Append(db);
                sb.Append("D"); //Draws
                sb.Append(de);
            }
            if (tournament.Rule.OptionalFields.Contains("ModLoss"))
            {
                sb.Append(db);
                sb.Append("ML"); //Modified Loss
                sb.Append(de);
            }
            sb.Append(db);
            sb.Append("L"); //Losses
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains("MoV"))
            {
                sb.Append(db);
                sb.Append("MoV"); //Margin of Victory
                sb.Append(de);
            }
            sb.Append(db);
            sb.Append("SoS"); //Strength of Schedule
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains("eSoS"))
            {
                sb.Append(db);
                sb.Append("eSoS"); //extended Strength of Schedule
                sb.Append(de);
            }

            if (lists)
            {
                sb.Append(db);
                sb.Append("Lists");
                sb.Append(de);
            }
            sb.Append(re);
            sb.Append(nl);

            var participants = /* tournament.Rounds.Count >= 1 ? tournament.Rounds[tournament.Rounds.Count - 1].Participants : */tournament.Participants;

            foreach (Player p in participants)
            {
                sb.Append(rb);
                sb.Append(db);
                sb.Append(p.Rank); //Rank
                sb.Append(de);
                if(!onlyNicknames)
                {
                    sb.Append(db);
                    sb.Append(p.Firstname); //Firstname
                    sb.Append(de);
                }
                sb.Append(db);
                sb.Append(p.Nickname); //Nickname
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.Team); //Team
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.Faction); //Faction
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.TournamentPoints); //Tournamentpoints
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.Wins); //Wins
                sb.Append(de);
                if (tournament.Rule.OptionalFields.Contains("ModWins"))
                {
                    sb.Append(db);
                    sb.Append(p.ModifiedWins); //Modified Wins
                    sb.Append(de);
                }
                if (tournament.Rule.OptionalFields.Contains("Draws"))
                {
                    sb.Append(db);
                    sb.Append(p.Draws); //Draws
                    sb.Append(de);
                }
                if (tournament.Rule.OptionalFields.Contains("ModLoss"))
                {
                    sb.Append(db);
                    sb.Append(p.ModifiedLosses); //Modified Loss
                    sb.Append(de);
                }
                sb.Append(db);
                sb.Append(p.Losses); //Losses
                sb.Append(de);
                if (tournament.Rule.OptionalFields.Contains("MoV"))
                {
                    sb.Append(db);
                    sb.Append(p.MarginOfVictory); //Margin of Victory
                    sb.Append(de);
                }
                sb.Append(db);
                sb.Append(p.StrengthOfSchedule); //Strength of Schedule
                sb.Append(de);
                if (tournament.Rule.OptionalFields.Contains("eSoS"))
                {
                    sb.Append(db);
                    sb.Append(p.ExtendedStrengthOfSchedule); //extended Strength of Schedule
                    sb.Append(de);
                }

                if (lists)
                {
                    sb.Append(db);
                    if (p.SquadList.Contains("http"))
                    {
                        sb.Append(ab1);
                        sb.Append(p.SquadList);
                        sb.Append(ab2);
                        sb.Append(p.SquadList);
                        sb.Append(ae);
                    }
                    else
                    {
                        sb.Append(p.SquadList);
                    }
                    sb.Append(de);
                }
                sb.Append(re);
                sb.Append(nl);
            }

            sb.Append(te);

            //end marquee tag
            //if (!bbcode)
            //{
            //    sb.Append("</marquee>");
            //}

            sb.Append(end);
            return sb.ToString();
        }

        private string WritePairings(Tournament tournament, bool bbcode, bool result, bool onlyNicknames)
        {
            if(tournament.Rounds.Count == 0)
            {
                if (bbcode)
                {
                    return "[b]No Round started => No Pairings yet[/b]";
                }
                else
                {
                    return "<html><body><h1>No Round started => No Pairings yet</h1></body></html>";
                }
            }
            StringBuilder sb = new StringBuilder();
            string title = "";
            if (result)
                title = tournament.Name + " - Results - Round " + (tournament.DisplayedRound - 1);
            else
                title = tournament.Name + " - Pairings - Round " + tournament.DisplayedRound;

            string head = "<!DOCTYPE html><meta charset=\"UTF-8\"><html><head><title>" + title + "</title></head><body><h2>" + title + "</h2> <br />";
            if (tournament.Rule.UsesScenarios)
            {
                head += "<h3>Scenario: " + tournament.ActiveScenario + "</h3><br />";
            }
            string tb = "<table>"; //Table begin
            string te = "</table>"; //Table end
            string rb = "<tr>"; //Table row begin
            string re = "</tr>"; //Table row end
            string db = "<td>"; //Table data begin
            string de = "</td>"; //Table data end
            string end = "</body></html>";
            string nl = Environment.NewLine;

            if (bbcode)
            {
                head = "[b]" + title + "[/b]";
                if (tournament.Rule.UsesScenarios)
                {
                    head += " [u]Scenario: " + tournament.ActiveScenario + "[/u]";
                }
                tb = "[table]";
                te = "[/table]";
                rb = "[tr]";
                re = "[/tr]";
                db = "[td]";
                de = "[/td]";
                end = nl;
            }

            sb.Append(head);

            ////Added marquee to see all Pairings without manually scrolling
            //         if (!bbcode)
            //{
            //	sb.Append("<marquee direction=\"up\" behavior=\"alternate\">");
            //}

            if (result)
            {
                sb.Append(tb);
                sb.Append(rb);
                sb.Append(db);
                sb.Append("T#"); //Table Number
                sb.Append(de);
                sb.Append(db);
                sb.Append("Player 1"); //Player 1
                sb.Append(de);
                sb.Append(db);
                sb.Append("Player 2"); //Player 2
                sb.Append(de);

                    sb.Append(db);
                    sb.Append("Score"); //Score
                    sb.Append(de);
                

                if (tournament.Rule.IsDrawPossible)
                {
                    sb.Append(db);
                    sb.Append("Winner"); //Winner
                    sb.Append(de);
                }

                sb.Append(re);
                sb.Append(nl);

                int round = tournament.Rounds.Count - 1;
                if (result)
                {
                    round--;
                }

                if (round < 0)
                {
                    return "";
                }

                foreach (Pairing p in tournament.Rounds[round].Pairings)
                {
                    sb.Append(rb);
                    sb.Append(db);
                    sb.Append(p.TableNr); //Table Number
                    sb.Append(de);
                    sb.Append(db);
                    sb.Append(onlyNicknames ? p.Player1.Nickname : p.Player1Name); //Player 1
                    sb.Append(de);
                    sb.Append(db);
                    sb.Append(onlyNicknames ? p.Player2.Nickname : p.Player2Name); //Player 2
                    sb.Append(de);
                    
                        sb.Append(db);
                        sb.Append(p.Player1Score + ":" + p.Player2Score); //Score
                        sb.Append(de);

                    if (tournament.Rule.IsDrawPossible)
                    {
                        sb.Append(db);
                        sb.Append(p.Winner); //Winner
                        sb.Append(de);
                    }

                    sb.Append(re);
                    sb.Append(nl);
                }
            }
            else
            {
                 sb.Append(tb);
                sb.Append(rb);
                sb.Append(db);
                sb.Append("Player 1"); //Player 1
                sb.Append(de);
                sb.Append(db);
                sb.Append("Player 2"); //Player 2
                sb.Append(de);
                sb.Append(db);
                sb.Append("T#"); //Table Number
                sb.Append(de);
                
                sb.Append(re);
                sb.Append(nl);

                int round = tournament.Rounds.Count - 1;
                if (result)
                {
                    round--;
                }

                if (round < 0)
                {
                    return "";
                }

                var doublePairings = new List<Pairing>();
                foreach (var p in tournament.Rounds[round].Pairings)
                {
                    doublePairings.Add(new Pairing(p));
                    if (doublePairings[^1].Player2.ID >= 0)
                    {
                        doublePairings.Add(new Pairing(p));
                        (doublePairings[^1].Player1, doublePairings[^1].Player2) =
                            (doublePairings[^1].Player2, doublePairings[^1].Player1);
                    }
                }

                var sorted = doublePairings.OrderBy(x => (onlyNicknames ? x.Player1.Nickname : x.Player1Name)).ToList();

                foreach (Pairing p in sorted)
                {
                    sb.Append(rb);
                    sb.Append(db);
                    sb.Append(onlyNicknames ? p.Player1.Nickname : p.Player1Name); //Player 1
                    sb.Append(de);
                    sb.Append(db);
                    sb.Append(onlyNicknames ? p.Player2.Nickname : p.Player2Name); //Player 2
                    sb.Append(de);
                    sb.Append(db);
                    sb.Append(p.TableNr); //Table Number
                    sb.Append(de);

                    sb.Append(re);
                    sb.Append(nl);
                }
            }

            sb.Append(te);

            //end marquee tag
            //if (!bbcode)
            //{
            //	sb.Append("</marquee>");
            //}

            sb.Append(end);
            return sb.ToString();
        }

        public string GetBBCode(Tournament tournament, bool table, bool result, bool onlyNicknames, bool lists)
        {
            if (table)
            {
                return WriteTable(tournament, true, onlyNicknames, lists);
            }
            else
            {
                return WritePairings(tournament, true, result, onlyNicknames);
            }
        }

        public (string, string) GetJSON(Tournament tournament)
        {
            (string json, string file) r = ("", "");
            r.json = JSONCreator.TournamentToListFortress(tournament);
            r.file = Path.Combine(SavePath, $"{tournament.Name}.json");
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
            using (StreamWriter sw = new StreamWriter(r.file, false))
            {
                sw.Write(r.json);
            }
            return r;
        }

        public string Print(Tournament tournament, bool onlyNicknames, bool lists)
        {
            string title = tournament.Name + " - Table - Round " + tournament.DisplayedRound;

            string print = WriteTable(tournament, false, onlyNicknames, lists);
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            using (StreamWriter sw = new StreamWriter(PrintFile, false))
            {
                sw.Write(print);
            }

            return PrintFile;
        }

        public string PrintPairings(Tournament tournament, bool result, bool onlyNicknames)
        {
            string title = "";
            if (result)
                title = tournament.Name + " - Results - Round " + (tournament.DisplayedRound - 1);
            else
                title = tournament.Name + " - Pairings - Round " + tournament.DisplayedRound;

            string print = WritePairings(tournament, false, result, onlyNicknames);
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            using (StreamWriter sw = new StreamWriter(PrintFile, false))
            {
                sw.Write(print);
            }

            return PrintFile;
        }

        public void Save(Tournament tournament, bool autosave, string getResultsText, bool? buttonCut, string Autosavetype = "")
        {
            string file = "";
            tournament.ButtonGetResultsText = getResultsText;
            tournament.ButtonCutState = buttonCut == true;
            if (autosave)
            {
                file = AutosavePath;
                if (!Directory.Exists(file))
                {
                    Directory.CreateDirectory(file);
                }
                string name = tournament.Name;
                name = name.Replace(':', ' ');
                name = name.Replace('\\', ' ');
                name = name.Replace('/', ' ');
                name = name.Replace('_', ' ');
                file = Path.Combine(file, "Autosave_" + DateTime.Now.ToFileTime() + "_" + name + "_" + Autosavetype + "." + Settings.FILEEXTENSION);
            }
            else
            {
                fileManager.AddFilter("*." + Settings.FILEEXTENSION, Settings.FILEEXTENSIONSNAME);
                if (fileManager.Save())
                {
                    file = fileManager.FileName;
                }
                else
                    return;
            }
            if (!file.EndsWith(Settings.FILEEXTENSION))
                file += Settings.FILEEXTENSION;

            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, tournament);
                }
            }
            catch (Exception)
            {
                file = AutosavePath;
                file += Path.PathSeparator + "Autosave_" + DateTime.Now.ToFileTime() + "_A_Tournament_" + Autosavetype + "." + Settings.FILEEXTENSION;
                using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, tournament);
                }
            }
        }

        public Tournament Load(string filename = "")
        {
            string file;
            if (filename != "")
            {
                file = filename;
            }
            else
            {
                fileManager.AddFilter("*." + Settings.FILEEXTENSION, Settings.FILEEXTENSIONSNAME);
                if (fileManager.Open())
                    file = fileManager.FileName;
                else
                    return null;
            }
            try
            {
                Tournament t = null;
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    t = (Tournament)formatter.Deserialize(stream);
                }
                return t;
            }
            catch (Exception)
            {
                messageManager.Show("Choose a valid file.");
                return null;
            }
        }

        public bool ShowMessageWithOKCancel(string text)
        {
            return messageManager.ShowWithOKCancel(text);
        }

        public void ShowMessage(string text)
        {
            messageManager.Show(text);
        }

        public bool AutosavePathExists
        {
            get
            {
                return Directory.Exists(AutosavePath);
            }
        }

        public void OpenAutosaveFolder()
        {
            if (AutosavePathExists)
                Process.Start("file://" + AutosavePath);
            else
                messageManager.Show("There is no autosave folder.");
        }

        public void DeleteAutosaveFolder()
        {
            if (AutosavePathExists)
            {
                Directory.Delete(AutosavePath, true);
                messageManager.Show("Autosave fodler was deleted.");
            }
            else
                messageManager.Show("There is no autosave folder.");
        }

        public bool GetColor()
        {
            if (File.Exists(TextColorFile))
            {
                string line;
                using (StreamReader sr = new StreamReader(TextColorFile))
                {
                    line = sr.ReadLine();
                }
                return line == "White";
            }
            else
                return false;
        }

        public void WriteColor(bool whiteText)
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            using (StreamWriter f = new StreamWriter(TextColorFile))
            {
                f.WriteLine(whiteText ? "White" : "Black");
            }
        }

        public string ReadBGImage()
        {
            if (File.Exists(BGImageFile))
            {
                string line;
                using (StreamReader sr = new StreamReader(BGImageFile))
                {
                    line = sr.ReadLine();
                }
                return line;
            }
            else
                return "";
        }

        public void WriteBGImage(string path)
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            using (StreamWriter f = new StreamWriter(BGImageFile))
            {
                f.WriteLine(path);
            }
        }

        public double GetSize()
        {
            if (File.Exists(TextSizeFile))
            {
                string line;
                using (StreamReader sr = new StreamReader(TextSizeFile))
                {
                    line = sr.ReadLine();
                }
                return Double.Parse(line);
            }
            else
                return 50.0;
        }

        public void WriteSize(double size)
        {
            using (StreamWriter f = new StreamWriter(TextSizeFile))
            {
                f.WriteLine(size);
            }
        }

        public string GetImagePath()
        {
            string img = Path.Combine(SavePath, "background.png");
            imgurl = null;
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".png";
            }
            img = Path.Combine(SavePath, "background.jpg");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".jpg";
            }
            img = Path.Combine(SavePath, "background.jpeg");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".jpeg";
            }
            img = Path.Combine(SavePath, "background.tif");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".tif";
            }
            img = Path.Combine(SavePath, "background.tiff");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".tiff";
            }
            TempImgPath = ReadBGImage();
            if (imgurl == null)
                imgurl = TempImgPath;
            if (TempImgPath != "" && (imgurl != TempImgPath))
            {
                File.Delete(imgurl);
                imgending = TempImgPath.Remove(0, TempImgPath.LastIndexOf('.'));
                imgurl = Path.Combine(SavePath, "background" + imgending);
                File.Copy(TempImgPath, imgurl, true);
                WriteBGImage(imgurl);
            }
            return imgurl;
        }

        public bool NewImage()
        {
            fileManager.AddFilter("*.jpg;*.jpeg;*.png;*.tif;*.tiff", "All Images");
            if (fileManager.Open())
            {
                imgurl = fileManager.FileName;
                imgending = imgurl.Remove(0, imgurl.LastIndexOf("."));
                if (!Directory.Exists(TempPath))
                    Directory.CreateDirectory(TempPath);
                TempImgPath = Path.Combine(TempPath, "background" + imgnr + imgending);
                imgnr++;
                File.Copy(imgurl, TempImgPath, true);
                WriteBGImage(TempImgPath);
                return true;
            }
            return false;
        }

        public string PrintScoreSheet(Tournament tournament)
        {
            string title = tournament.Name + " - Pairings - Round " + tournament.DisplayedRound;
            List<string> print = new List<string>();
            string temp = "<!DOCTYPE html><html><head><title>" + title + "</title></head><body>";
            title = "Round " + tournament.DisplayedRound + " - Table ";
            foreach (Pairing p in tournament.Rounds[tournament.Rounds.Count - 1].Pairings)
            {
                temp = "<table width=100%><tr><td><h4>" + title + p.TableNr + "</h4></td><td><h4>" + p.Player1Name + "</h4></td><td><h4>" + p.Player2Name + "</h4></td></tr>";
                print.Add(temp);
                temp = "<tr><td></td><td>Points destroyed    _________</td><td>Points destroyed    _________</td></tr>";
                print.Add(temp);
                if (false && tournament.FirstRound)
                {
                    if (tournament.PrintDDENG)
                    {
                        temp = "<tr><td colspan=\"3\"><b>Blue Deck:</b> 2x Shaken Pilot, 2x Blinded Pilot, 2x Damaged Cockpit, 2x Stunned Pilot, 2x Console Fire, 2x Damaged Engine, 2x Major Explosion, 2x Weapons Failure, 2x Major Hull Breach, 2x Structural Damage, 2x Damaged Sensor Array, 2x Loose Stabilizer, 2x Thrust Control Fire, 7x Direct Hit!</td></tr>";
                        print.Add(temp);
                        temp = "<tr><td colspan=\"3\"><b>Red Deck:</b> 2x Injured Pilot, 2x Blinded Pilot, 2x Damaged Cockpit, 2x Stunned Pilot, 2x Console Fire, 2x Damaged Engine, 2x Minor Explosion, 2x Weapon Malfunction, 2x Minor Hull Breach, 2x Structural Damage, 2x Damaged Sensor Array, 2x Munitions Failure, 2x Thrust Control Fire, 7x Direct Hit!</td></tr>";
                        print.Add(temp);
                    }
                    else if (tournament.PrintDDGER)
                    {
                        temp = "<tr><td colspan=\"3\"><b>Blaues Deck:</b> 2x Verstörter Pilot, 2x Geblendeter Pilot, 2x Cockpitschaden, 2x Benommener Pilot, 2x Konsolenbrand, 2x Triebwerksschaden, 2x Schwere Explosion, 2x Waffenausfall, 2x Schwerer Hüllenbruch, 2x Strukturschäden, 2x Sensorenausfall, 2x Lockerer Stabilisator, 2x Schubreglerbrand, 7x Volltreffer!</td></tr>";
                        print.Add(temp);
                        temp = "<tr><td colspan=\"3\"><b>Rotes Deck:</b> 2x Verletzter Pilot, 2x Geblendeter Pilot, 2x Cockpitschaden, 2x Benommener Pilot, 2x Konsolenbrand, 2x Triebwerksschaden, 2x Schwache Explosion, 2x Waffenfehlfunktion, 2x Hüllenbruch, 2x Strukturschäden, 2x Sensorenausfall, 2x Ladehemmung, 2x Schubreglerbrand, 7x Volltreffer!</td></tr>";
                        print.Add(temp);
                    }
                }
                temp = "</table><hr/>";
                print.Add(temp);

            }
            temp = "</body></html>";
            print.Add(temp);
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            using (StreamWriter sw = new StreamWriter(PrintFile, false, Encoding.UTF8))
            {
                foreach (string s in print)
                    sw.WriteLine(s);
            }

            return PrintFile;
        }

        private void SetTempPath()
        {
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
        }

        public string[] GetAutosaveFiles()
        {
            return Directory.GetFiles(AutosavePath, "*" + Settings.FILEEXTENSION);
        }

        public string PrintBestInFaction(Tournament tournament, bool onlyNicknames)
        {
            string print = WriteFactions(tournament, false, onlyNicknames);
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            using (StreamWriter sw = new StreamWriter(PrintFile, false))
            {
                sw.Write(print);
            }

            return PrintFile;
        }

        private string WriteFactions(Tournament tournament, bool bbcode, bool onlyNicknames)
        {
            StringBuilder sb = new StringBuilder();
            string title = tournament.Name + " - Best in Factions";
            
            string head = "<!DOCTYPE html><meta charset=\"UTF-8\"><html><head><title>" + title + "</title></head><body><h2>" + title + "</h2> <br />";
            string hs = "<h3>";
            string he = "</h3>";
            string tb = "<table>"; //Table begin
            string te = "</table>"; //Table end
            string rb = "<tr>"; //Table row begin
            string re = "</tr>"; //Table row end
            string db = "<td>"; //Table data begin
            string de = "</td>"; //Table data end
            string end = "</body></html>";
            string blank = "<br />";
            string nl = Environment.NewLine;

            if (bbcode)
            {
                head = "[b]" + title + "[/b]";
                tb = "[table]";
                te = "[/table]";
                rb = "[tr]";
                re = "[/tr]";
                db = "[td]";
                de = "[/td]";
                blank = nl;
                end = nl;
            }

            sb.Append(head);

            string[] factions = tournament.Rule.Factions;

            foreach (var f in factions)
            {
                sb.Append(blank);
                sb.Append(WriteFaction(tournament, f, tb, te, rb, re, db, de, end, nl, hs, he, blank, onlyNicknames));
            }
            
            return sb.ToString();
        }

        private string WriteFaction(Tournament tournament
            , string faction
            , string tb
            , string te
            , string rb
            , string re
            , string db
            , string de
            , string end
            , string nl,
            string hs, string he, string blank, bool onlyNicknames)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(hs + "Best in " + faction + he);
            //sb.Append(blank);
            
            //Tableheader
            sb.Append(tb);
            sb.Append(rb);
            sb.Append(db);
            sb.Append("#"); //Rank
            sb.Append(de);
            if (!onlyNicknames)
            {
                sb.Append(db);
                sb.Append("Firstname"); //Firstname
                sb.Append(de);
            }
            sb.Append(db);
            sb.Append("Nickname"); //Nickname
            sb.Append(de);
            sb.Append(db);
            sb.Append("Team"); //Team
            sb.Append(de);
            sb.Append(db);
            sb.Append("Faction"); //Faction
            sb.Append(de);
            sb.Append(db);
            sb.Append("TP"); //Tournamentpoints
            sb.Append(de);
            sb.Append(db);
            sb.Append("W"); //Wins
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains("ModWins"))
            {
                sb.Append(db);
                sb.Append("MW"); //Modified Wins
                sb.Append(de);
            }
            if (tournament.Rule.OptionalFields.Contains("Draws"))
            {
                sb.Append(db);
                sb.Append("D"); //Draws
                sb.Append(de);
            }
            if (tournament.Rule.OptionalFields.Contains("ModLoss"))
            {
                sb.Append(db);
                sb.Append("ML"); //Modified Loss
                sb.Append(de);
            }
            sb.Append(db);
            sb.Append("L"); //Losses
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains("MoV"))
            {
                sb.Append(db);
                sb.Append("MoV"); //Margin of Victory
                sb.Append(de);
            }
            sb.Append(db);
            sb.Append("SoS"); //Strength of Schedule
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains("eSoS"))
            {
                sb.Append(db);
                sb.Append("eSoS"); //extended Strength of Schedule
                sb.Append(de);
            }
            sb.Append(re);
            sb.Append(nl);
            
            var participants =  tournament.Participants.Where(x => x.Faction == faction).ToList();
            List<Player> printList = new List<Player>();
            for (int i = 0; i < participants.Count; i++)
            {
                printList.Add(new Player(participants[i]));
                printList[i].Rank = i + 1;
            }
            
             foreach (Player p in printList)
            {
                sb.Append(rb);
                sb.Append(db);
                sb.Append(p.Rank); //Rank
                sb.Append(de);
                if (!onlyNicknames)
                {
                    sb.Append(db);
                    sb.Append(p.Firstname); //Firstname
                    sb.Append(de);
                }
                sb.Append(db);
                sb.Append(p.Nickname); //Nickname
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.Team); //Team
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.Faction); //Faction
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.TournamentPoints); //Tournamentpoints
                sb.Append(de);
                sb.Append(db);
                sb.Append(p.Wins); //Wins
                sb.Append(de);
                if (tournament.Rule.OptionalFields.Contains("ModWins"))
                {
                    sb.Append(db);
                    sb.Append(p.ModifiedWins); //Modified Wins
                    sb.Append(de);
                }
                if (tournament.Rule.OptionalFields.Contains("Draws"))
                {
                    sb.Append(db);
                    sb.Append(p.Draws); //Draws
                    sb.Append(de);
                }
                if (tournament.Rule.OptionalFields.Contains("ModLoss"))
                {
                    sb.Append(db);
                    sb.Append(p.ModifiedLosses); //Modified Loss
                    sb.Append(de);
                }
                sb.Append(db);
                sb.Append(p.Losses); //Losses
                sb.Append(de);
                if (tournament.Rule.OptionalFields.Contains("MoV"))
                {
                    sb.Append(db);
                    sb.Append(p.MarginOfVictory); //Margin of Victory
                    sb.Append(de);
                }
                sb.Append(db);
                sb.Append(p.StrengthOfSchedule); //Strength of Schedule
                sb.Append(de);
                if (tournament.Rule.OptionalFields.Contains("eSoS"))
                {
                    sb.Append(db);
                    sb.Append(p.ExtendedStrengthOfSchedule); //extended Strength of Schedule
                    sb.Append(de);
                }
                sb.Append(re);
                sb.Append(nl);
            }

            sb.Append(te);
            
            sb.Append(end);
            return sb.ToString();
        }

    }
}
