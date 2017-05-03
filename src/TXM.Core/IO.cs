using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace TXM.Core
{
	[Serializable]
    public class IO
    {
        private IFile fileManager;
        private IMessage messageManager;
        public string AutosavePath { get; private set; }
        public string TempPath { get; private set; }
        public string LanguagePath { get; private set; }
        public string BinLanguagePath { get; private set; }
        private string iconPath;
        public Dictionary<string, string> IconFiles { get; private set; }
        public string PrintFile { get; private set; }
        public string TextColorFile { get; private set; }
        public string SavePath { get; private set; }
        private string imgurl;
        private string languageList;
        private string langseturl;
        private string languageURL = "http://apps.piratesoftatooine.de/Languages/";
        public string TempImgPath { get; private set; }
        private int imgnr = 0;
        private string imgending;
        private string cards;
        private string translations;
        private Statistic stats;


        public IO(IFile filemanager, IMessage messagemanager)
        {
            fileManager = filemanager;
            messageManager = messagemanager;
            SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM");
            AutosavePath = Path.Combine(SavePath, "Autosave");
            TempPath = Path.Combine(SavePath, "Temp");
            PrintFile = Path.Combine(TempPath, "print.html");
            TextColorFile = Path.Combine(SavePath, "textcolor.txt");
            LanguagePath = Path.Combine(SavePath, "Language");
            BinLanguagePath = Path.Combine(LanguagePath, "Bin");
            langseturl = Path.Combine(SavePath, "language.txt");
            languageList = Path.Combine(TempPath, "LanguageList.txt");
            iconPath = Path.Combine(SavePath, "Icons");
            IconFiles = new Dictionary<string, string>();
            if (!Directory.Exists(iconPath))
            {
                Directory.CreateDirectory(iconPath);
                SaveIcons(true);
            }
            else
                SaveIcons(false);
        }


        public Tournament2 GOEPPImport()
        {
            fileManager.AddFilter("*.gip", "GÖPP Import Datei");
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
                    //5.+ Player:   ID  ||Forename ||Name||Nickname ||Faction ||City       ||Team            ||A||B||C||D||x
                    //              7619||Dieter   ||Chri||DieChri  ||Rebellen||Leverkusen ||PGF Siegen e. V.||3|| ||1|| ||x
                    //A: ArmyListGiven 3 = Yes, 0 = No
                    //B: ?
                    //C: Paid 1 = Yes, 0 = No
                    //D: Actual T3 Rank
                    //x = End
                    Tournament2 tournament = new Tournament2(Int32.Parse(gipFile[2]), gipFile[1], gipFile[0]);
                    for (int i = 4; i < gipFile.Count; i++)
                    {
                        tournament.AddPlayer(ConvertLineToPlayer(gipFile[i]));
                    }
                    //tournament.TeamProtection = false;
                    return tournament;
                }
                catch (Exception e)
                {
                    messageManager.Show("Bitte eine gültige GÖPP Import Datei auswählen.");
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
                faction = "Scum and Villainy";
            else
                faction = "Rebels";
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

        public void GOEPPExport(Tournament2 tournament)
        {
            fileManager.AddFilter("*.gep", "GÖPP Export Datei");
            if(!tournament.Single)
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
                    line = xwp.T3ID + sep + xwp.Forename + sep + lastname + sep + xwp.Nickname + sep + xwp.PlayersFactionAsString + sep + city + sep + xwp.Team + sep + xwp.Rank + sep + (xwp.Points + xwp.ArmyRank) + sep + xwp.Points + sep + xwp.MarginOfVictory + sep + xwp.ArmyRank + rest;
                    temp.Add(xwp.Nickname);
                    WriteLine(file, line);
                }
            }
        }

        private void WriteLine(string file, string line)
        {
            using (StreamWriter f = new StreamWriter(file, true, Encoding.GetEncoding(28591)))
            {
                f.WriteLine(line + "||x");
            }
        }

        public List<string> GetLanguages()
        {
            List<string> l = new List<string>();
            if (Directory.Exists(LanguagePath))
            {
                string[] invis = Directory.GetFiles(LanguagePath, ".*");
                foreach (var s in Directory.GetFiles(LanguagePath))
                {
                    if (!invis.Contains(s))
                    {
                        l.Add(s.Substring(s.LastIndexOf(Path.DirectorySeparatorChar) + 1, s.Length - s.LastIndexOf('.') + 2));
                    }
                }
            }
            return l;
        }

        public static string TableForForum(Tournament2 tournament, List<Player> players)
        {
			string output = tournament.Name + " - Table - Round " + tournament.DisplayedRound + "\n[table][tr][td]#[/td][td]Name[/td][td]Points[/td][td]Wins[/td][td]Looses[/td][td]MoV[/td][td]SoS[/td][/tr]\n";
            for (int i = 1; i <= players.Count; i++)
            {
                Player p = players[i - 1];
				output += "[tr][td]" + i + "[/td][td]" + p.DisplayName + "[/td][td]" + p.Points + "[/td][td]" + p.Wins + "[/td][td]" + p.Looses + "[/td][td]" + p.MarginOfVictory + "[/td][td]" + p.PointsOfEnemies + "[/td][/tr]\n";
            }
            output += "[/table]\n";
            return output;
        }

        public static string PairingForForum(Tournament2 tournament, List<Pairing> pairing)
        {
			string output = tournament.Name + " - Table - Round " + tournament.DisplayedRound + "\n[table][tr][td]Table No.[/td][td]Player 1[/td][td]Player 2[/td][td]Result[/td][/tr]\n";
            for (int i = 1; i <= pairing.Count; i++)
            {
                Pairing p = pairing[i - 1];
                output += "[tr][td]" + p.TableNr + "[/td][td]" + p.Player1Name + "[/td][td]" + p.Player2Name + "[/td][td]" + p.Player1Score + ":" + p.Player2Score + "[/td][/tr]\n";
            }
            output += "[/table]\n";
            return output;
        }

        public void Print(Tournament2 tournament)
        {
			string title = tournament.Name + " - Table - Round " + tournament.DisplayedRound; 

            List<string> print = new List<string>();
            string temp = "<!DOCTYPE html><html><head><title>" + title + "</title></head><body><table>";
            print.Add(temp);
            temp = "<h2>" + title + "</h2></br>";
            print.Add(temp);
            temp = "<tr><td>#</td><td>Name</td><td>Points</td><td>W</td><td>L</td><td>MoV</td><td>SoS</td></tr>";
            print.Add(temp);
            foreach (Player p in tournament.Rounds[tournament.Rounds.Count - 1].Participants)
            {
                temp = "<tr><td>" + p.Rank + "</td><td>" + p.DisplayName + "</td><td>" + p.Points + "</td><td>" + p.Wins + "</td><td>" + p.Looses + "</td><td>" + p.MarginOfVictory + "</td><td>" + p.PointsOfEnemies + "</td></tr>";
                print.Add(temp);
            }
            temp = "</table></body></html>";
            print.Add(temp);
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            using (StreamWriter sw = new StreamWriter(PrintFile, false))
            {
                foreach (string s in print)
                    sw.WriteLine(s);
            }

            Process.Start("file://" + PrintFile);
        }

        public void Print(Tournament2 tournament, bool result)
        {
            string title;
            if (result)
				title = tournament.Name + " - Results - Round " + tournament.DisplayedRound;
            else
				title = tournament.Name + " - Pairings - Round " + tournament.DisplayedRound;
            List<string> print = new List<string>();
            string temp = "<!DOCTYPE html><html><head><title>" + title + "</title></head><body><table>";
            print.Add(temp);
            temp = "<h2>" + title + "</h2></br>";
            print.Add(temp);
			temp = "<tr><td>Table-No</td><td>Player 1</td><td>Player 2</td>";
            if (result)
                temp += "<td>Result</td>";
            temp += "</tr>";
            print.Add(temp);
            tournament.Sort();
            foreach (Pairing p in tournament.Rounds[tournament.Rounds.Count - 1].Pairings)
            {
                temp = "<tr><td>" + p.TableNr + "</td><td>" + p.Player1Name + "</td><td>" + p.Player2Name + "</td>";
                if (result)
                    temp += "<td>" + p.Player1Score + ":" + p.Player2Score + "</td>";
                temp += "</tr>";
                print.Add(temp);
            }
            temp = "</table></body></html>";
            print.Add(temp);
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            using (StreamWriter sw = new StreamWriter(PrintFile, false))
            {
                foreach (string s in print)
                    sw.WriteLine(s);
            }

            Process.Start("file://" + PrintFile);
        }

        public void Save(Tournament2 tournament, bool autosave, bool? buttonGetResults, bool? buttonNextRound, bool? buttonCut, string Autosavetype = "")
        {
            string file = "";
            tournament.ButtonGetResultState = buttonGetResults == true;
            tournament.ButtonNextRoundState = buttonNextRound == true;
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
                file += "\\Autosave_" + DateTime.Now.ToFileTime() + "_" + name + "_" + Autosavetype + Settings.FILEEXTENSION;
            }
            else
            {
                fileManager.AddFilter("*" + Settings.FILEEXTENSION, Settings.FILEEXTENSIONSNAME);
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
				file += "\\Autosave_" + DateTime.Now.ToFileTime() + "_A_Tournament_" + Autosavetype + Settings.FILEEXTENSION;
				using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					formatter.Serialize(stream, tournament);
				}
			}
            if (tournament.Statistics != null)
            {
                tournament.Statistics.Path = file.Substring(0, file.LastIndexOf('.')) + ".txmstats";
                SaveStatistic(tournament.Statistics, true);
            }
        }

        public void SaveStatistic(Statistic stats, bool save = false)
        {
            string path = "";
            if (save)
            {
                path = stats.Path;
                if (path == "" || path == null)
                {
                    fileManager.AddFilter("*.txmstats", "TXM Statistik Datei");
                    if (fileManager.Save())
                    {
                        path = fileManager.FileName;
                    }
                    else
                        return;
                }
            }
            else
                path = Path.Combine(TempPath, Settings.BINFILE);

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, stats);
            }
        }

        public Tournament2 Load(string filename = "")
        {
            string file;
            if (filename != "")
            {
                file = filename;
            }
            else
            {
                fileManager.AddFilter("*" + Settings.FILEEXTENSION, Settings.FILEEXTENSIONSNAME);
                foreach (string s in Settings.OLDEXTENSIONS)
                    fileManager.AddFilter("*" + s, Settings.FILEEXTENSIONSNAME + " alt", true);
                if (fileManager.Open())
                    file = fileManager.FileName;
                else
                    return null;
            }
                try
                {
                    Tournament2 t = null;
                    if (file.EndsWith(".txmb"))
                    {
                        Tournament told = Tournament.Load(file);
                        t = new Tournament2(told.Name);
                        t.Participants = told.Participants;
                        t.FirstRound = told.FirstRound;
                        t.PrePaired = told.PrePaired;
                        t.Nicknames = told.Nicknames;
                        t.MaxSquadPoints = told.MaxSquadPoints;
                        if (told.Rounds != null)
                        {
                            t.Rounds = new List<Round2>();
                            foreach (var r in told.Rounds)
                                t.Rounds.Add(new Round2(r.Pairings.ToList<Pairing>(), r.Participants));
                        }
                        t.FilePath = told.FilePath;
                        t.AutoSavePath = told.AutoSavePath;
                        t.DisplayedRound = told.DisplayedRound;
                        t.Statistics = told.Statistics;
                        t.Cut = told.Cut;
                        t.CutStarted = told.CutStarted;
                        t.WonFreeticketsCalculated = false;
                        if (told.Pairings != null)
                        {
                            t.Pairings = new List<Pairing>();
                            foreach (var p in told.Pairings)
                                t.Pairings.Add(p);
                        }
                        t.ButtonGetResultState = told.ButtonGetResultState;
                        t.ButtonNextRoundState = told.ButtonNextRoundState;
                        t.ButtonCutState = told.ButtonCutState;
                        t.T3ID = told.T3ID;
                        t.GOEPPVersion = told.GOEPPVersion;
                        t.currentStartNr = told.currentStartNr;
                        foreach (var i in Tournament.givenStartNr)
                            Tournament.givenStartNr.Add(i);
                        t.ListOfPlayers = told.ListOfPlayers;
                        t.WonFreetickets = told.WonFreetickets;
                        t.currentCountOfPlayer = told.currentCountOfPlayer;
                        t.WinnerLastRound = told.WinnerLastRound;
                        t.freeticket = told.freeticket;
                        t.WonFreeticketsCalculated = false;
                    }
                    else
                    {
                        IFormatter formatter = new BinaryFormatter();
                        using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            t = (Tournament2)formatter.Deserialize(stream);
                        }
                    }
                    return t;
                }
                catch (Exception e)
                {
                    messageManager.Show("Choose a valid file.");
                    return null;
                }
            return null;
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

        public void ShowAutosaveFolder()
        {
            if (AutosavePathExists)
                Process.Start("file://" + AutosavePath);
            else
                messageManager.Show("Es exisitiert kein Autosave Ordner.");
        }

        public void DeleteAutosaveFolder()
        {
            if (AutosavePathExists)
            {
                Directory.Delete(AutosavePath, true);
                messageManager.Show("Autosave Ordner wurde gelöscht.");
            }
            else
                messageManager.Show("Es exisitiert kein Autosave Ordner.");
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
            using (StreamWriter f = new StreamWriter(TextColorFile))
            {
                f.WriteLine(whiteText ? "White" : "Black");
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
            img = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM", "background.jpg");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".jpg";
            }
            img = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM", "background.jpeg");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".jpeg";
            }
            img = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM", "background.tif");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".tif";
            }
            img = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM", "background.tiff");
            if (File.Exists(img))
            {
                imgurl = img;
                imgending = ".tiff";
            }
            return imgurl;
        }

        public void CopyImage()
        {
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            TempImgPath = Path.Combine(TempPath, "background" + imgnr + imgending);
            imgnr++;
            File.Copy(imgurl, TempImgPath, true);
        }

        public void NewImage()
        {
            fileManager.AddFilter("*.jpg;*.jpeg;*.png;*.tif;*.tiff", "Alle Bilderformate");
            if (fileManager.Open())
            {
                string imageSrc = fileManager.FileName;
                string newImageSrc = Path.Combine(SavePath, "background" + imageSrc.Remove(0, imageSrc.LastIndexOf(".")));
                File.Copy(imageSrc, newImageSrc, true);
                imgurl = newImageSrc;
                CopyImage();
            }
        }

        public void LoadYASBFiles()
        {
            SetTempPath();
            WebClient wc = new WebClient();
            wc.DownloadFile(Settings.SQUADFILECARDS, cards);
            wc.DownloadFile(Settings.SQUADFILETRANSLATION, translations);
        }

        private void Translate(string data, char what)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> lines = new List<string>();
            do
            {
                lines.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains("name: "))
                {
                    int a = lines[i - 1].Contains("ship") ? i - 2 : i - 1;
                    TranslatePilot(lines[a], lines[i], what);
                }
            }
        }
        private void TranslatePilot(string uk, string de, char what)
        {
            int s = uk.IndexOf("\"", 0) + 1;
            int e = uk.IndexOf("\"", s);
            uk = uk.Substring(s, e - s);
            s = de.IndexOf("\"", 0) + 1;
            e = de.IndexOf("\"", s);
            de = de.Substring(s, e - s);
            if (what == 'p')
            {
                foreach (Pilot p in stats.Pilots)
                {
                    if (p.Name == uk)
                    {
                        p.Gername = de;
                        break;
                    }
                }
            }
            else if (what == 'u')
            {
                foreach (Upgrade p in stats.Upgrades)
                {
                    if (p.Name == uk)
                    {
                        p.Gername = de;
                        break;
                    }
                }
            }
            else if (what == 'm')
            {
                foreach (Modification p in stats.Modifications)
                {
                    if (p.Name == uk)
                    {
                        p.Gername = de;
                        break;
                    }
                }
            }
            else if (what == 't')
            {
                foreach (Title p in stats.Titles)
                {
                    if (p.Name == uk)
                    {
                        p.Gername = de;
                        break;
                    }
                }
            }
        }
        private List<Ship> GetShips(string data)
        {
            List<Ship> ships = new List<Ship>();
            int start = data.IndexOf(" name:", 0);
            int end;

            do
            {
                end = data.IndexOf(" name:", start + 1);
                if (end <= 0)
                    end = data.Length;
                ships.Add(ConvertToShip(data.Substring(start, end - start)));
                start = end;
            } while (start != data.Length);

            return ships;
        }
        private Ship ConvertToShip(string data)
        {
            int s = 0;
            int e;
            List<int[]> maneuvers = new List<int[]>() ;
            List<XWingAction> actions = new List<XWingAction>() ;
            s = data.IndexOf("\"", s) + 1;
            e = data.IndexOf("\"", s);
            string name = data.Substring(s, e - s);
            int shields = 0, hull = 0, agility = 0, attack = 0;

            s = data.IndexOf("[", s) + 1;
            e = data.IndexOf("]", s);
            List<Faction> factions = GetFactions(data.Substring(s, e - s));

            s = data.IndexOf("attack", s);
            if (s > -1)
            {
                s = data.IndexOf(":", s) + 2;
                 attack = Int32.Parse(data.Substring(s, 1));

                s = data.IndexOf(":", s) + 2;
                 agility = Int32.Parse(data.Substring(s, 1));

                s = data.IndexOf(":", s) + 2;
                 hull = Int32.Parse(data.Substring(s, 1));

                s = data.IndexOf(":", s) + 2;
                 shields = Int32.Parse(data.Substring(s, 1));

                s = data.IndexOf("[", s);
                e = data.IndexOf("]", s);
                actions = GetActions(data.Substring(s, e - s));

                s = data.IndexOf("[", e);
                e = s + 1;
                for (int i = 0; i < 5; i++)
                    e = data.IndexOf("]", e) + 2;
                if (s > 0)
                    maneuvers = GetManeuvers(data.Substring(s, e - s));
                else
                {
                    maneuvers = new List<int[]>();
                    maneuvers.Add(new int[6]);
                }
            }

                return new Ship(name, factions, attack, agility, hull, shields, actions, maneuvers);
        }
        private List<int[]> GetManeuvers(string data)
        {
            List<int[]> maneuvers = new List<int[]>();
            int start = data.IndexOf("[", 0) + 1;
            start = data.IndexOf("[", start);
            int end;
            while (start > 0)
            {
                end = data.IndexOf("]", start);
                int[] row = GetSpeeds(data.Substring(start, end - start));
                maneuvers.Add(row);
                start = data.IndexOf("[", end + 1) + 1;
            }
            return maneuvers;
        }
        private int[] GetSpeeds(string data)
        {
            int[] speeds = new int[6];
            int pos = 0;
            int oldpos = 0;
            for (int i = 0; i < 5; i++)
            {
                pos = data.IndexOf(",", pos + 1);
                try
                {
                    speeds[i] = Int32.Parse(data.Substring(pos - 1, 1));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    speeds[i] = Int32.Parse(data.Substring(oldpos + 2, 1));
                }
                oldpos = pos;
            }
            if (pos != -1)
                speeds[5] = Int32.Parse(data.Substring(pos + 2, 1));
            else
                speeds[5] = 0;
            return speeds;
        }
        private List<XWingAction> GetActions(string data)
        {
            List<XWingAction> actions = new List<XWingAction>();
            int start = data.IndexOf("\"", 0) + 1;
            int end;
            string f;
            do
            {
                end = data.IndexOf("\"", start);
                f = data.Substring(start, end - start);
                if (f == "Target Lock")
                    actions.Add(XWingAction.TargetLock);
                else if (f == "Boost")
                    actions.Add(XWingAction.Boost);
                else if (f == "Evade")
                    actions.Add(XWingAction.Evade);
                else if (f == "Barrel Roll")
                    actions.Add(XWingAction.BarrelRoll);
                else if (f == "Recover")
                    actions.Add(XWingAction.Recover);
                else if (f == "Reinforce")
                    actions.Add(XWingAction.Reinforce);
                else if (f == "Coordinate")
                    actions.Add(XWingAction.Coordinate);
                else if (f == "Jam")
                    actions.Add(XWingAction.Jam);
                else if (f == "Cloak")
                    actions.Add(XWingAction.Cloak);
                else
                    actions.Add(XWingAction.Focus);
                start = data.IndexOf("\"", end + 1) + 1;
            } while (start > 0);

            return actions;
        }
        private List<Faction> GetFactions(string data)
        {
            List<Faction> factions = new List<Faction>();
            int start = data.IndexOf("\"", 0) + 1;
            int end;
            string f;
            do
            {
                end = data.IndexOf("\"", start);
                f = data.Substring(start, end - start);
                if (f == "Rebel Alliance")
                    factions.Add(Faction.Rebels);
                else if (f == "Galactic Empire")
                    factions.Add(Faction.Imperial);
                else
                    factions.Add(Faction.Scum);
                start = data.IndexOf("\"", end + 1) + 1;
            } while (start > 0);

            return factions;
        }
        private List<Pilot> GetPilots(string data)
        {
            List<Pilot> pilots = new List<Pilot>();
            int start = data.IndexOf(" name:", 0);
            int end;

            do
            {
                end = data.IndexOf(" name:", start + 1);
                if (end <= 0)
                    end = data.Length;
                pilots.Add(ConvertToPilot(data.Substring(start, end - start)));
                start = end;
            } while (start != data.Length);

            return pilots;
        }
        private Pilot ConvertToPilot(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            Faction faction = Faction.Scum;
            int id = -1;
            List<string> sources = new List<string>();
            bool unique = false;
            Ship ship = null;
            int skill = -1;
            int points = -1;
            List<Slot> slots = new List<Slot>();

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name") && !(str.Contains("canonical")))
                {
                    if (str.Contains("\""))
                    {
                        s = str.IndexOf("\"", 0) + 1;
                        e = str.IndexOf("\"", s);
                    }
                    else
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                    }
                    name = str.Substring(s, e - s);
                }
                else if (str.Contains("faction"))
                {
                    s = str.IndexOf("\"", 0);
                    e = str.IndexOf("\"", s + 1) + 1;
                    faction = GetFactions(str.Substring(s, e - s))[0];
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("unique"))
                {
                    unique = true;
                }
                else if (str.Contains("ship"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    ship = GetShip(str.Substring(s, e - s));
                }
                else if (str.Contains("skill"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    skill = Int32.Parse(str.Substring(s, 1));
                }
                else if (str.Contains("points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
                else if (str.Contains("slots"))
                {
                    for (int j = i; j < pilotvalues.Count; j++)
                        str += pilotvalues[j];
                    slots = GetSlots(str);
                    break;
                }
            }

            return new Pilot(name, faction, id, sources, unique, ship, skill, points, slots);
        }
        private List<string> GetSources(string data)
        {
            List<string> sources = new List<string>();
            int start = data.IndexOf("\"", 0) + 1;
            int end;
            string f;
            do
            {
                end = data.IndexOf("\"", start);
                sources.Add(data.Substring(start, end - start));
                start = data.IndexOf("\"", end + 1) + 1;
            } while (start > 0);

            return sources;
        }
        private List<Slot> GetSlots(string data)
        {
            List<Slot> slots = new List<Slot>();
            int start = data.IndexOf("\"", 0) + 1;
            if (start <= 0)
            {
                slots.Add(Slot.None);
                return slots;
            }
            int end;
            string f;
            do
            {
                end = data.IndexOf("\"", start);
                f = data.Substring(start, end - start);
                if (f == "Elite")
                    slots.Add(Slot.Elite);
                else if (f == "Torpedo")
                    slots.Add(Slot.Torpedo);
                else if (f == "Astromech")
                    slots.Add(Slot.Astromech);
                else if (f == "Turret")
                    slots.Add(Slot.Turret);
                else if (f == "Missile")
                    slots.Add(Slot.Missile);
                else if (f == "Crew")
                    slots.Add(Slot.Crew);
                else if (f == "Cannon")
                    slots.Add(Slot.Cannon);
                else if (f == "Bomb")
                    slots.Add(Slot.Bomb);
                else if (f == "System")
                    slots.Add(Slot.System);
                else if (f == "Hardpoint")
                    slots.Add(Slot.Hardpoint);
                else if (f == "Team")
                    slots.Add(Slot.Team);
                else if (f == "Illicit")
                    slots.Add(Slot.Illicit);
                else if (f == "Salvaged Astromech")
                    slots.Add(Slot.SalvagedAstromech);
                else
                    slots.Add(Slot.None);
                start = data.IndexOf("\"", end + 1) + 1;
            } while (start > 0);

            return slots;
        }
        private Ship GetShip(string shipName)
        {
            foreach (Ship s in stats.Ships)
            {
                if (s.Name == shipName)
                    return s;
            }
            return null;
        }
        private List<Upgrade> GetUpgrades(string data)
        {
            List<Upgrade> upgrades = new List<Upgrade>();
            int start = data.IndexOf(" name:", 0);
            int end;

            do
            {
                end = data.IndexOf(" name:", start + 1);
                if (end <= 0)
                    end = data.Length;
                upgrades.Add(ConvertToUpgrade(data.Substring(start, end - start)));
                start = end;
            } while (start != data.Length);

            return upgrades;
        }
        private Upgrade ConvertToUpgrade(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            int id = -1;
            Slot slot = Slot.None;
            List<string> sources = new List<string>();
            int attack = -1;
            string range = "";
            bool unique = false;
            int points = -1;

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    if (s == 0 && e == -1)
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                    }
                    name = str.Substring(s, e - s);
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("attack"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    attack = Int32.Parse(str.Substring(s, 1));
                }
                else if (str.Contains("range"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    try
                    {
                        range = str.Substring(s, e - s);
                    }
                    catch(ArgumentException ae)
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                        range = str.Substring(s, e - s);
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("unique"))
                {
                    unique = true;
                }
                else if (str.Contains("points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
                else if (str.Contains("slot"))
                {
                    for (int j = i; j < pilotvalues.Count; j++)
                        str += pilotvalues[j];
                    slot = GetSlots(str)[0];
                }
            }

            return new Upgrade(name, id, slot, sources, attack, range, unique, points);
        }
        private List<Modification> GetModifications(string data)
        {
            List<Modification> modifications = new List<Modification>();
            int start = data.IndexOf(" name:", 0);
            int end;

            do
            {
                end = data.IndexOf(" name:", start + 1);
                if (end <= 0)
                    end = data.Length;
                modifications.Add(ConvertToModification(data.Substring(start, end - start)));
                start = end;
            } while (start != data.Length);

            return modifications;
        }
        private Modification ConvertToModification(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            int id = -1;
            List<string> sources = new List<string>();
            int points = -1;
            Ship onlyFor = null;

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    name = str.Substring(s, e - s);
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("ship") && !str.Contains("ship.data.") && !str.Contains("restriction_func"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    onlyFor = GetShip(str.Substring(s, e - s));
                }
                else if (str.Contains("points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
            }

            return new Modification(name, id, points, sources, onlyFor);
        }

        private void SaveIcons(bool save)
        {
            string s = Path.Combine(iconPath, "AddUser.png");
            if (save)
                Icons.Add_User_100.Save(s);
            IconFiles.Add("AddUser", s);
            s = Path.Combine(iconPath, "Randomizer.png");
            if (save)
                Icons.Clover_100.Save(s);
            IconFiles.Add("Randomizer", s);
            s = Path.Combine(iconPath, "EditUser.png");
            if (save)
                Icons.Edit_User_100.Save(s);
            IconFiles.Add("EditUser", s);
            s = Path.Combine(iconPath, "RemoveUser.png");
            if (save)
                Icons.Remove_User_100.Save(s);
            IconFiles.Add("RemoveUser", s);
            s = Path.Combine(iconPath, "Reset.png");
            if (save)
                Icons.Rotate_Left_100.Save(s);
            IconFiles.Add("Reset", s);
            s = Path.Combine(iconPath, "Save.png");
            if (save)
                Icons.Save_100.Save(s);
            IconFiles.Add("Save", s);
            s = Path.Combine(iconPath, "Disqualify.png");
            if (save)
                Icons.Unfriend_Male_100.Save(s);
            IconFiles.Add("Disqualify", s);
            s = Path.Combine(iconPath, "ChangePairings.png");
            if (save)
                Icons.User_Group_100.Save(s);
            IconFiles.Add("ChangePairings", s);
            s = Path.Combine(iconPath, "Timer.png");
            if (save)
                Icons.Watch_100.Save(s);
            IconFiles.Add("Timer", s);
        }

        public Bitmap GetIcon(string name)
        {
            Bitmap b = null;
            switch(name)
            {
                case  "UserAdd":
                    b = Icons.Add_User_100;
                    break;
                case "Randomizer":
                    b = Icons.Clover_100;
                    break;
                case "UserEdit":
                    b = Icons.Edit_User_100;
                    break;
                case "UserRemove":
                    b = Icons.Remove_User_100;
                    break;
                case "Undo":
                    b = Icons.Rotate_Left_100;
                    break;
                case "Save":
                    b = Icons.Save_100;
                    break;
                case "Disqualify":
                    b = Icons.Unfriend_Male_100;
                    break;
                case "ChangePairings":
                    b = Icons.User_Group_100;
                    break;
                case "Timer":
                    b = Icons.Watch_100;
                    break;                   
            }
            return b;
        }

        public void PrintScoreSheet(Tournament2 tournament)
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
                if (tournament.FirstRound)
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

            Process.Start("file://" + PrintFile);
        }

        private List<Title> GetTitles(string data)
        {
            List<Title> titles = new List<Title>();
            int start = data.IndexOf(" name:", 0);
            int end;

            do
            {
                end = data.IndexOf(" name:", start + 1);
                if (end <= 0)
                    end = data.Length;
                titles.Add(ConvertToTitles(data.Substring(start, end - start)));
                start = end;
            } while (start != data.Length);

            return titles;
        }
        private Title ConvertToTitles(string data)
        {
            int s = 0;
            int e = data.IndexOf("\n", s); ;
            List<string> pilotvalues = new List<string>();
            do
            {
                pilotvalues.Add(data.Substring(s, e - s));
                s = e + 2;
                e = data.IndexOf("\n", s);
            } while (e > 0);

            string name = "";
            int id = -1;
            List<string> sources = new List<string>();
            int points = -1;
            Ship onlyFor = null;
            bool unique = false;

            for (int i = 0; i < pilotvalues.Count; i++)
            {
                string str = pilotvalues[i];
                if (str.Contains(" name"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    try
                    {
                        name = str.Substring(s, e - s);
                    }
                    catch (ArgumentOutOfRangeException exc)
                    {
                        s = str.IndexOf("\'", 0) + 1;
                        e = str.IndexOf("\'", s);
                        name = str.Substring(s, e - s);
                    }
                }
                else if (str.Contains(" id"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        id = Int32.Parse(str.Substring(s, 3));
                    }
                    catch (ArgumentOutOfRangeException exce)
                    {
                        try
                        {
                            id = Int32.Parse(str.Substring(s, 2));
                        }
                        catch (ArgumentOutOfRangeException excep)
                        {
                            id = Int32.Parse(str.Substring(s, 1));
                        }
                    }
                }
                else if (str.Contains("sources"))
                {
                    //s = str.IndexOf("[", 0);
                    //e = str.IndexOf("]", s);
                    //sources = GetSources(str.Substring(s, e - s));
                }
                else if (str.Contains("ship") && !str.Contains("ship.") && !str.Contains("restriction_func") && !str.Contains("validation_func"))
                {
                    s = str.IndexOf("\"", 0) + 1;
                    e = str.IndexOf("\"", s);
                    onlyFor = GetShip(str.Substring(s, e - s));
                }
                else if (str.Contains("unique"))
                {
                    unique = true;
                }
                else if (str.Contains(" points"))
                {
                    s = str.IndexOf(":", 0) + 2;
                    try
                    {
                        points = Int32.Parse(str.Substring(s, 2));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        points = Int32.Parse(str.Substring(s, 1));
                    }
                }
            }

            return new Title(name, id, sources, unique, onlyFor, points);
        }

        private void SetTempPath()
        {
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            cards = Path.Combine(TempPath, "cards.txt");
            translations = Path.Combine(TempPath, "gertrans.txt");
        }

        public Statistic LoadContents(bool update = false, bool silence = false)
        {
            SetTempPath();
            if (!File.Exists(cards))
            {
                if (silence)
                    LoadYASBFiles();
                else
                {
                    messageManager.Show("Bitte erst die Daten aktualisieren.");
                    return null;
                }
            }
            if (File.Exists(Path.Combine(TempPath, Settings.BINFILE)) && !update && new FileInfo(Path.Combine(TempPath, Settings.BINFILE)).Length != 0)
                return LoadStatistic();
            else
            {
                string text;
                int posStart = 0;
                int posEnd;
                stats = new Statistic();

                using (StreamReader sr = new StreamReader(cards, Encoding.UTF8))
                {
                    text = sr.ReadToEnd();
                }

                posStart = text.IndexOf("ships:", posStart);
                posEnd = text.IndexOf("pilotsById:", posStart);
                stats.Ships = GetShips(text.Substring(posStart, posEnd - posStart));

                posStart = posEnd;
                posEnd = text.IndexOf("upgradesById:", posStart);
                stats.Pilots = GetPilots(text.Substring(posStart, posEnd - posStart));

                posStart = posEnd;
                posEnd = text.IndexOf("modificationsById:", posStart);
                stats.Upgrades = GetUpgrades(text.Substring(posStart, posEnd - posStart));

                posStart = posEnd;
                posEnd = text.IndexOf("titlesById:", posStart);
                stats.Modifications = GetModifications(text.Substring(posStart, posEnd - posStart));

                posStart = posEnd;
                posEnd = text.IndexOf("exportObj.setupCardData = (basic_cards", posStart);
                stats.Titles = GetTitles(text.Substring(posStart, posEnd - posStart));

                text = "";
                posStart = 0;
                posEnd = 0;

                using (StreamReader sr = new StreamReader(translations))
                {
                    text = sr.ReadToEnd();
                }

                posStart = text.IndexOf("pilot_translations", posStart);
                posEnd = text.IndexOf("upgrade_translations", posStart);
                Translate(text.Substring(posStart, posEnd - posStart), 'p');

                posStart = posEnd;
                posEnd = text.IndexOf("modification_translations", posStart);
                Translate(text.Substring(posStart, posEnd - posStart), 'u');

                posStart = posEnd;
                posEnd = text.IndexOf("title_translations", posStart);
                Translate(text.Substring(posStart, posEnd - posStart), 'm');

                posStart = posEnd;
                posEnd = text.IndexOf("exportObj.setupCardData", posStart);
                Translate(text.Substring(posStart, posEnd - posStart), 't');

                SaveStatistic(stats);
                return stats;
            }
        }
        public Statistic LoadStatistic(bool load = false)
        {
            string path = "";
            if (load)
            {
                fileManager.AddFilter("*.txmstats", "TXM Statistik Datei");
                if (fileManager.Open())
                {
                    path = fileManager.FileName;
                }
                else
                    return null;
            }
            else
            {
                SetTempPath();
                path = Path.Combine(TempPath, Settings.BINFILE);
            }
            Statistic stats = null;
            IFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    stats = (Statistic)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                messageManager.Show("Die ausgewählte Datei ist keine gültige txmstats-Datei.");
            }
            return stats;
        }
        public void Export(Statistic stats, bool csv, bool info = true, string path = "")
        {
            List<string> lines = new List<string>();
            stats.Sort();
            int length = stats.IPilots.Count > stats.IShips.Count ? stats.IPilots.Count : stats.IShips.Count;
            length = length > stats.IUpgrades.Count ? length : stats.IUpgrades.Count;
            if (csv)
            {
                if (path == "")
                {
                    fileManager.AddFilter("*.csv", "CSV-Datei");
                    if (fileManager.Save())
                        path = fileManager.FileName;
                }
                if (path != "")
                {
                    string line = "=SUMME(A2:A" + length + ");Schiffe;MERCode;;=SUMME(E2:E" + length + ");Piloten;Welle;;=SUMME(I2:I" + length + ");Aufwertungskarten;;=SUMME(L2:L" + length + ");Punktzahlen;;;=SUMME(P2:P" + length + ");Fraktionen;Prozent;Schiffe pro Fraktion;Schiffepro Liste;;Schiffe pro Squad;=E1/P1;;";
                    lines.Add(line);
                    List<string> PointFormulas = new List<string>(); ;
                    List<string> FactionFormulas = new List<string>();
                    List<string> ShipsPerFactionFormulas = new List<string>();
                    List<string> WaveFormulas = new List<string>();
                    for (int i = 0; i <= stats.Points.Count; i++)
                    {
                        line = "=L" + (i + 2) + "/L1";
                        PointFormulas.Add(line);
                    }
                    for (int i = 0; i <= stats.FPoints.Count; i++)
                    {
                        line = "=P" + (i + 2) + "/P1";
                        FactionFormulas.Add(line);
                    }
                    for (int i = 0; i <= stats.FPoints.Count; i++)
                    {
                        line = "=S" + (i + 2) + "/P" + (i + 2);
                        ShipsPerFactionFormulas.Add(line);
                    }
                    for (int i = 0; i <= stats.Waves.Count; i++)
                    {
                        line = "=W" + (i + 2) + "/E1";
                        WaveFormulas.Add(line);
                    }
                    for (int i = 0; i < length; i++)
                    {
                        line = "";
                        if (i < stats.IShips.Count)
                            line += stats.IShips[i].Count + ";" + stats.IShips[i].Gername + ";" + stats.IShips[i].MERCode + ";;";
                        else
                            line += ";;;;";
                        if (i < stats.IPilots.Count)
                            line += stats.IPilots[i].Count + ";" + stats.IPilots[i].Gername + ";" + stats.IPilots[i].Wave + ";;";
                        else
                            line += ";;;;";
                        if (i < stats.IUpgrades.Count)
                            line += stats.IUpgrades[i].Count + ";" + stats.IUpgrades[i].Gername + ";;";
                        else
                            line += ";;;";
                        if (i < stats.Points.Count)
                            line += stats.Points[i][1] + ";" + stats.Points[i][0] + ";" + PointFormulas[i] + ";;";
                        else
                            line += ";;;;";
                        if (i < stats.FPoints.Count)
                            line += stats.FPoints[i].Count + ";" + Player.FactionToString(stats.FPoints[i].PointsFaction) + ";" + FactionFormulas[i] + ";" + stats.ShipsPerFaction[i] + ";" + ShipsPerFactionFormulas[i] + ";;";
                        else
                            line += ";;;;;;";
                        if (i < stats.Waves.Count)
                            line += "Welle " + stats.Waves[i].Name + ";" + stats.Waves[i].Count + ";" + WaveFormulas[i] + ";;";
                        else
                            line += ";;;;";
                        lines.Add(line);
                    }

                    using (StreamWriter f = new StreamWriter(path))
                    {
                        foreach (string s in lines)
                            f.WriteLine(s);
                    }

                    if (info)
                    {
                        messageManager.Show("Datei erfolgreich exportiert.");
                    }
                }
            }
        }
        public void LoadCSV(Tournament2 tournament)
        {
            LoadCSV(tournament.Statistics, true, tournament);
        }
        public void LoadCSV(Statistic stats, bool overview = false, Tournament2 tournament = null)
        {
            string path, file, overviewList, line;
            string[] url, lines;
            int countOfLists;
            bool win = true;
            List<Statistic> statistics;
            List<string> lists, players, output;
            lists = new List<string>();
            players = new List<string>();
            statistics = new List<Statistic>();
            fileManager.AddFilter("*.txt", "TXT-Datei");
            if (fileManager.Open())
            {
                path = fileManager.FileName;
            }
            else
                return;
            using (StreamReader sr = new StreamReader(path))
            {
                file = sr.ReadToEnd();
            }
            if (file.Contains('\n'))
            {
                lines = file.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] != "")
                    {
                        lines[i] = lines[i].Remove(lines[i].LastIndexOf('\r'));
                    }
                }
            }
            else
                lines = file.Split('\r');

            countOfLists = lines[1].Split('\t').Length - 1;

            statistics.Add(stats);
            for (int i = 1; i < countOfLists; i++)
                statistics.Add(LoadContents());

            for (int j = 1; j < lines.Length; j++)
            {
                if (lines[j] != "")
                {
                    url = lines[j].Split('\t');
                    players.Add(url[0]);
                    for (int i = 1; i <= countOfLists; i++)
                    {
                        if (url[i] != "")
                        {
                            if (url[i].StartsWith("\"") && url[i].EndsWith("\""))
                                url[i] = url[i].Substring(url[i].IndexOf("\"") + 1, url[i].LastIndexOf("\"") - 1);
                            else if (url[i].StartsWith("\""))
                                url[i] = url[i].Substring(url[i].IndexOf("\"") + 1);
                            else if (url[i].EndsWith("\""))
                                url[i] = url[i].Substring(0, url[i].LastIndexOf("\"") - 2);
                        }
                        if (url[i] == "")
                            overviewList = "";
                        else
                            overviewList = statistics[i - 1].Parse(url[i], true, overview);
                        lists.Add(overviewList);
                        //Aktuell nur für die erste Liste, da die Turnierverwaltung noch nicht
                        //mit Escalation klar kommt.
                        if (tournament != null && overviewList != "" && i == 1)
                        {
                            int trenner = overviewList.IndexOf(';');
                            int trenner2 = overviewList.IndexOf(';', trenner + 1);
                            tournament.AddInfos(url[0], Int32.Parse(overviewList.Substring(trenner + 1, trenner2 - trenner - 1)), url[i]);
                        }
                    }
                }
            }

            if (countOfLists > 1)
            {
                messageManager.Show("Es wurden mehrere Listen pro Spieler ermittelt.\nWähle einen Speicherort aus.\nAnschließend wird die erste Liste angezeigt.");
                fileManager.AddFilter("*.txmstats", "TXM Statistik Datei");
                if (fileManager.Save())
                {
                    path = fileManager.FileName;
                }
                else
                    return;
                for (int i = 0; i < countOfLists; i++)
                {
                    statistics[i].Path = path.Substring(0, path.LastIndexOf('.')) + " " + (i + 1) + path.Substring(path.LastIndexOf('.'), path.Length - path.LastIndexOf('.'));
                    SaveStatistic(statistics[i], true);
                }
            }

            if (overview)
            {
                messageManager.Show("Gib bitte den Speicherort für die\nÜbersichtsliste(n) an.");
                fileManager.AddFilter("*.csv", "CSV-Datei");
                if (fileManager.Save())
                {
                    path = fileManager.FileName;
                }
                else
                    return;
                output = new List<string>();
                line = "Spieler;Fraktion;Punkte;Liste";
                output.Add(line);
                for (int i = 0; i < players.Count; i++)
                {
                    url = lines[i + 1].Split('\t');
                    line = players[i] + ";";
                    for (int j = 0; j < countOfLists; j++)
                    {
                        line += lists[i * countOfLists + j] + ";";
                    }
                    output.Add(line);
                }
                using (StreamWriter f = new StreamWriter(path))
                {
                    foreach (string s in output)
                        f.WriteLine(s);
                }
                if (countOfLists == 1)
                {
                    path = path.Substring(0, path.LastIndexOf('.')) + "_statistik.csv";
                    Export(statistics[0], true, false, path);
                }
                else
                {
                    for (int i = 0; i < countOfLists; i++)
                    {
                        string p = statistics[i].Path.Substring(0, path.LastIndexOf('.') + 2) + ".csv";
                        Export(statistics[i], true, false, p);
                    }
                }
            }
        }

        public string LoadCSV()
        {
            fileManager.AddFilter("*.csv", "CSV-Datei");
            if (fileManager.Open())
            {
                return fileManager.FileName;
            }
            return null;
        }

        public void ConvertCSV(string separator, string fileIn, string fileOut)
        {
            List<string> linesOld = new List<string>();
            List<string> linesNew = new List<string>();
            string line;
            using (StreamReader sr = new StreamReader(fileIn))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    linesOld.Add(line);
                }
            }
            linesNew.Add("[table]");
            foreach (string s in linesOld)
            {
                string[] temp = s.Split(separator.ToCharArray()[0]);
                string newLine = "[tr]";
                foreach (string t in temp)
                {
                    newLine += "[td]" + t + "[/td]";
                }
                newLine += "[/tr]";
                linesNew.Add(newLine);
            }
            linesNew.Add("[/table]");
            using (StreamWriter sw = new StreamWriter(fileOut))
            {
                foreach (string s in linesNew)
                {
                    sw.WriteLine(s);
                }
            }
            messageManager.Show("Wurde konvertiert.");
        }


        public string[] GetAutosaveFiles()
        {
            return Directory.GetFiles(AutosavePath, "*" + Settings.FILEEXTENSION);
        }
    }
}
