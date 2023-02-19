using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using TXM.Core.Export.JSON;

namespace TXM.Core;

public class IO
{
    private IFile fileManager;
    private IMessage messageManager;
    public string AutosavePath { get; private set; }
    public string TempPath { get; private set; }
    public string PrintFile { get; private set; }
    private string SettingsFile { get; set; }
    public string SavePath { get; private set; }
    public string TempImgPath { get; private set; }
    private int imgnr = 0;
    private string imgending;
    private JsonSerializerOptions options;
    public Settings ActiveSettings { get; set; }

    private bool AutosavePathExists => Directory.Exists(AutosavePath);


    public IO(IFile filemanager, IMessage messagemanager)
    {
        fileManager = filemanager;
        messageManager = messagemanager;
        SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM");
        AutosavePath = Path.Combine(SavePath, "Autosave");
        TempPath = Path.Combine(SavePath, "Temp");
        PrintFile = Path.Combine(TempPath, "print.html");
        SettingsFile = Path.Combine(SavePath, "settings.conf");
        options = new JsonSerializerOptions { WriteIndented = true };
    }

    #region internal methods - import / load

    /// <summary>
    /// GOEPP Import File from tabletoptournaments.net (T3)
    /// </summary>
    /// <returns>A Tournament Object from the Date in the file or null if the file is invalid or the file chooser is canceled</returns>
    internal Tournament? GOEPPImport()
    {
        fileManager.AddFilter("*.gip", $"GÖPP {Texts.ImportFile}");
        if (fileManager.Open())
        {
            try
            {
                List<string> gipFile = new List<string>();
                using (StreamReader sr = new StreamReader(fileManager.FileName, Encoding.GetEncoding(28591)))
                {
                    string? line;
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
                    tournament.AddPlayer(ConvertGOEPPLineToPlayer(gipFile[i]));
                }

                //tournament.TeamProtection = false;
                return tournament;
            }
            catch (Exception)
            {
                messageManager.Show(Texts.InvalidFile.Replace("<filetype>", "gip"));
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Imports a tournament from a csv file
    /// </summary>
    /// <returns>A tournamnet or null if the file is invalid or the file chooser was canceld</returns>
    internal Tournament? CSVImport()
    {
        fileManager.AddFilter("*.csv", $"{Texts.ExcelFile} (CSV)");
        if (fileManager.Open())
        {
            try
            {
                List<string> csvFile = new List<string>();
                using (StreamReader sr = new StreamReader(fileManager.FileName))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        csvFile.Add(line);
                    }
                }

                //Fileformat:
                //0          1         2        3    4       5    6          7       8  
                //First Name;Last Name;Nickname;Team;Faction;Paid;List given;Won Bye;Squadlist
                Tournament tournament =
                    new Tournament(
                        fileManager.FileName.Split(Path.DirectorySeparatorChar)[
                            fileManager.FileName.Split(Path.DirectorySeparatorChar).Length - 1].Split('.')[0], null);
                for (int i = 1; i < csvFile.Count; i++)
                {
                    tournament.AddPlayer(ConvertCSVToPlayer(csvFile[i]));
                }

                tournament.TeamProtection = false;
                return tournament;
            }
            catch (Exception)
            {
                messageManager.Show(Texts.InvalidFile.Replace("<filetype>", "csv"));
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Adds Data from a csv file to an already imported / created tournament
    /// </summary>
    /// <param name="tournament">The tournament where the new data should be added</param>
    internal void CSVImportAdd(Tournament tournament)
    {
        fileManager.AddFilter("*.csv", $"{Texts.ExcelFile} (CSV)");
        if (fileManager.Open())
        {
            try
            {
                List<string> csvFile = new List<string>();
                using (StreamReader sr = new StreamReader(fileManager.FileName))
                {
                    string? line;
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
                    var p = (Player)tournament.Participants.Where(x => x.Nickname == player.Nickname).First();
                    ;
                    if (p == null)
                    {
                        p = (Player)tournament.Participants.Where(x => x.Name == player.Name)
                            .Where(x => x.Firstname == player.Firstname).First();
                    }

                    if (p == null)
                    {
                        tournament.AddPlayer(player);
                    }
                    else
                    {
                        p.HasWonBye = player.HasWonBye;
                        p.SquadList = player.SquadList;
                        p.HasListGiven = player.HasListGiven;
                        if (!p.HasListGiven && p.SquadList != String.Empty)
                        {
                            p.HasListGiven = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                messageManager.Show(Texts.InvalidFile.Replace("<filetype>", "csv"));
                return;
            }
        }
    }

    /// <summary>
    /// Load a tournament from the filesystem
    /// </summary>
    /// <param name="filename">Optional filename</param>
    /// <returns>A tournament object or null</returns>
    public Tournament? Load(string filename = "")
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
            {
                file = fileManager.FileName;
            }
            else
            {
                return null;
            }
        }

        try
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(file))
            {
                sb.Append(sr.ReadLine());
            }

            return JsonSerializer.Deserialize<Tournament>(sb.ToString());
        }
        catch (Exception)
        {
            messageManager.Show(Texts.InvalidFile.Replace("<filetype>", Settings.FILEEXTENSION));
            return null;
        }
    }

    /// <summary>
    /// Load the settings from the filesystem
    /// </summary>
    public void LoadSettings()
    {
        StringBuilder sb = new StringBuilder();
        using (StreamReader sr = new StreamReader(SettingsFile))
        {
            sb.Append(sr.ReadLine());
        }

        ActiveSettings = JsonSerializer.Deserialize<Settings>(sb.ToString());
    }

    /// <summary>
    /// Let the User choose a new background image for the timer window
    /// </summary>
    /// <returns>True if it's a new image</returns>
    internal bool NewImage()
    {
        fileManager.AddFilter("*.jpg;*.jpeg;*.png;*.tif;*.tiff", Texts.ImageFiles);
        if (fileManager.Open())
        {
            string imgurl = fileManager.FileName;
            imgending = imgurl.Remove(0, imgurl.LastIndexOf("."));
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            TempImgPath = Path.Combine(TempPath, "background" + imgnr + imgending);
            imgnr++;
            File.Copy(imgurl, TempImgPath, true);
            ActiveSettings.BGImagePath = TempImgPath;
            SaveSettings();
            return true;
        }

        return false;
    }

    #endregion

    #region internal methods - export / print / Save

    /// <summary>
    /// Saves the current tournement in a GOEPP Export file for upload on tabletoptournaments.net (T3)
    /// </summary>
    /// <param name="tournament">The tournament that should be exported</param>
    internal void GOEPPExport(Tournament tournament)
    {
        fileManager.AddFilter("*.gep", $"GÖPP {Texts.ExportFile}");
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
            string line = "#GoePP-Exportdatei, " + tournament.GOEPPVersion + " Export vom " +
                          DateTime.Now.ToShortDateString();
            string sep = "||", rest = sep + 0 + sep + 0 + sep + 0 + sep + 0;
            using (StreamWriter f = new StreamWriter(file, false, Encoding.GetEncoding(28591)))
            {
                f.WriteLine(line + "||x");
            }

            line = "#TID-" + tournament.T3ID;
            WriteGOEPPLine(file, line);
            foreach (Player p in tournament.Participants)
            {
                if (temp.Contains(p.Nickname))
                {
                    lastname = p.Name + " 1";
                }
                else
                {
                    lastname = p.Name;
                }

                if (p.City.Length > 20)
                {
                    city = p.City.Substring(0, 20);
                }
                else
                {
                    city = p.City;
                }

                line = p.T3ID + sep + p.Firstname + sep + lastname + sep + p.Nickname + sep + p.Faction + sep + city +
                       sep + p.Team + sep + p.Rank + sep + (p.TournamentPoints + p.ArmyRank) + sep +
                       p.TournamentPoints + sep + p.MarginOfVictory + sep + p.ArmyRank + rest;
                temp.Add(p.Nickname);
                WriteGOEPPLine(file, line);
            }
        }
    }


    /// <summary>
    /// Creates the Output of the ranking table for HTML or BBCODE (forums)
    /// </summary>
    /// <param name="tournament">The Tournament which should be prepared for output</param>
    /// <param name="bbcode">True for BBCODE, false for HTML</param>
    /// <returns>A string with the whole html/bbcode</returns>
    internal string CreateOutputForTable(Tournament tournament, bool bbcode)
    {
        StringBuilder sb = new StringBuilder();
        string title = tournament.Name + $" - {Texts.Table} - {Texts.Round} {tournament.DisplayedRound}";

        string head, tb, te, rb, re, db, de, end, nl = Environment.NewLine;

        if (bbcode)
        {
            head = $"[b]{title}[/b]";
            tb = "[table]";
            te = "[/table]";
            rb = "[tr]";
            re = "[/tr]";
            db = "[td]";
            de = "[/td]";
            end = nl;
        }
        else
        {
            head = $"<!DOCTYPE html><meta charset=\"UTF-8\"><html><head><title>{title}</title></head><body><h2>" +
                   title + "</h2> <br />";
            tb = "<table>";
            te = "</table>";
            rb = "<tr>";
            re = "</tr>";
            db = "<td>";
            de = "</td>";
            end = "</body></html>";
        }

        sb.Append(head);

        sb.Append(tb);
        sb.Append(rb);
        sb.Append(db);
        sb.Append("#"); //Rank
        sb.Append(de);
        sb.Append(db);
        sb.Append(Texts.Firstname); //Firstname
        sb.Append(de);
        sb.Append(db);
        sb.Append(Texts.Nickname); //Nickname
        sb.Append(de);
        sb.Append(db);
        sb.Append(Texts.Team); //Team
        sb.Append(de);
        sb.Append(db);
        sb.Append(Texts.Faction); //Faction
        sb.Append(de);
        sb.Append(db);
        sb.Append(Texts.TournamentPointsShort); //Tournamentpoints
        sb.Append(de);
        sb.Append(db);
        sb.Append(Texts.WinsShort); //Wins
        sb.Append(de);
        if (tournament.Rule.OptionalFields.Contains(Literals.ModWins))
        {
            sb.Append(db);
            sb.Append(Texts.ModifiedWinsShort); //Modified Wins
            sb.Append(de);
        }

        if (tournament.Rule.OptionalFields.Contains(Literals.Draws))
        {
            sb.Append(db);
            sb.Append(Texts.DrawsShort); //Draws
            sb.Append(de);
        }

        if (tournament.Rule.OptionalFields.Contains(Literals.ModLoss))
        {
            sb.Append(db);
            sb.Append(Texts.ModifiedLossesShort); //Modified Loss
            sb.Append(de);
        }

        sb.Append(db);
        sb.Append(Texts.Losses); //Losses
        sb.Append(de);
        if (tournament.Rule.OptionalFields.Contains(Literals.MoV))
        {
            sb.Append(db);
            sb.Append(Texts.MarginOfVictoryShort); //Margin of Victory
            sb.Append(de);
        }

        sb.Append(db);
        sb.Append(Texts.StrengthOfScheduleShort); //Strength of Schedule
        sb.Append(de);
        if (tournament.Rule.OptionalFields.Contains(Literals.eSoS))
        {
            sb.Append(db);
            sb.Append(Texts.ExtendedStrengthOfScheduleShort); //extended Strength of Schedule
            sb.Append(de);
        }

        sb.Append(re);
        sb.Append(nl);

        var participants = tournament.Rounds.Count >= 1
            ? tournament.Rounds[tournament.Rounds.Count - 1].Participants
            : tournament.Participants;

        foreach (Player p in participants)
        {
            sb.Append(rb);
            sb.Append(db);
            sb.Append(p.Rank); //Rank
            sb.Append(de);
            sb.Append(db);
            sb.Append(p.Firstname); //Firstname
            sb.Append(de);
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
            if (tournament.Rule.OptionalFields.Contains(Literals.ModWins))
            {
                sb.Append(db);
                sb.Append(p.ModifiedWins); //Modified Wins
                sb.Append(de);
            }

            if (tournament.Rule.OptionalFields.Contains(Literals.Draws))
            {
                sb.Append(db);
                sb.Append(p.Draws); //Draws
                sb.Append(de);
            }

            if (tournament.Rule.OptionalFields.Contains(Literals.ModLoss))
            {
                sb.Append(db);
                sb.Append(p.ModifiedLosses); //Modified Loss
                sb.Append(de);
            }

            sb.Append(db);
            sb.Append(p.Losses); //Losses
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains(Literals.MoV))
            {
                sb.Append(db);
                sb.Append(p.MarginOfVictory); //Margin of Victory
                sb.Append(de);
            }

            sb.Append(db);
            sb.Append(p.StrengthOfSchedule); //Strength of Schedule
            sb.Append(de);
            if (tournament.Rule.OptionalFields.Contains(Literals.eSoS))
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

    /// <summary>
    /// Creates the Output for HTML or BBCODE (forums)
    /// </summary>
    /// <param name="tournament">The Tournament which should be prepared for output</param>
    /// <param name="bbcode">True for BBCODE, false for HTML</param>
    /// <param name="result">True if the results of the last round should be printed. </param>
    /// <returns>A string with the whole html/bbcode</returns>
    internal string CreateOutputForPairings(Tournament tournament, bool bbcode, bool result)
    {
        if (tournament.Rounds.Count == 0)
        {
            if (bbcode)
            {
                return $"[b]{Texts.NoRoundStarted} => {Texts.NoPairingsYet}[/b]";
            }
            else
            {
                return $"<html><body><h1>{Texts.NoRoundStarted} => {Texts.NoPairingsYet}</h1></body></html>";
            }
        }

        StringBuilder sb = new StringBuilder();
        string title = "", head, tb, te, rb, re, db, de, end, nl = Environment.NewLine;

        if (result)
        {
            title = tournament.Name + $" - {Texts.Results} - {Texts.Round} {(tournament.DisplayedRound - 1)}";
        }
        else
        {
            title = tournament.Name + $" - {Texts.Pairings} - {Texts.Round} {tournament.DisplayedRound}";
        }

        if (bbcode)
        {
            head = $"[b]{title}[/b]";
            if (tournament.Rule.UsesScenarios)
            {
                head += $" [u]Scenario: {tournament.ActiveScenario}[/u]";
            }

            tb = "[table]";
            te = "[/table]";
            rb = "[tr]";
            re = "[/tr]";
            db = "[td]";
            de = "[/td]";
            end = nl;
        }
        else
        {
            head = $"<!DOCTYPE html><meta charset=\"UTF-8\"><html><head><title>{title}</title></head><body><h2>" +
                   title + "</h2> <br />";
            if (tournament.Rule.UsesScenarios)
            {
                head += $"<h3>Scenario: {tournament.ActiveScenario}</h3><br />";
            }

            tb = "<table>";
            te = "</table>";
            rb = "<tr>";
            re = "</tr>";
            db = "<td>";
            de = "</td>";
            end = "</body></html>";
        }

        sb.Append(head);

        sb.Append(tb);
        sb.Append(rb);
        sb.Append(db);
        sb.Append($"{Texts.TableShort}#"); //Table Number
        sb.Append(de);
        sb.Append(db);
        sb.Append($"{Texts.Player} 1"); //Player 1
        sb.Append(de);
        sb.Append(db);
        sb.Append($"{Texts.Player}  2"); //Player 2
        sb.Append(de);
        if (result)
        {
            sb.Append(db);
            sb.Append(Texts.Score); //Score
            sb.Append(de);
        }

        if (result && tournament.Rule.IsDrawPossible)
        {
            sb.Append(db);
            sb.Append(Texts.Winner); //Winner
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
            sb.Append(p.TableNo); //Table Number
            sb.Append(de);
            sb.Append(db);
            sb.Append(p.Player1Name); //Player 1
            sb.Append(de);
            sb.Append(db);
            sb.Append(p.Player2Name); //Player 2
            sb.Append(de);
            if (result)
            {
                sb.Append(db);
                sb.Append($"{p.Player1Score}:{p.Player2Score}"); //Score
                sb.Append(de);
            }

            if (result && tournament.Rule.IsDrawPossible)
            {
                sb.Append(db);
                sb.Append(p.Winner); //Winner
                sb.Append(de);
            }

            sb.Append(re);
            sb.Append(nl);
        }

        sb.Append(te);

        sb.Append(end);
        return sb.ToString();
    }

    /// <summary>
    /// Exports the Tournament to JSON for listfotress.com
    /// </summary>
    /// <param name="tournament">The tournament which should be exported</param>
    /// <returns>(the JSON for the export, the file name)</returns>
    internal (string, string) GetJsonForListfortress(Tournament tournament)
    {
        (string json, string file) r = ("", "");
        r.json = JSONCreator.TournamentToListFortress(tournament);
        r.file = Path.Combine(SavePath, $"{tournament.Name}.json");
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        using (var sw = new StreamWriter(r.file, false))
        {
            sw.Write(r.json);
        }

        return r;
    }

    /// <summary>
    /// Print the players of a tournament as a HTML file
    /// </summary>
    /// <param name="tournament">The tournament which players should be printed</param>
    /// <returns>The file name which contains the HTML</returns>
    internal string PrintPlayerList(Tournament tournament)
    {
        string print = CreateOutputForTable(tournament, false);
        if (!Directory.Exists(TempPath))
        {
            Directory.CreateDirectory(TempPath);
        }

        using (StreamWriter sw = new StreamWriter(PrintFile, false))
        {
            sw.Write(print);
        }

        return PrintFile;
    }

    /// <summary>
    /// Print the current pairings or the results of the last round
    /// </summary>
    /// <param name="tournament">The tournament which contains the pairings</param>
    /// <param name="result">True if the results should be printed.</param>
    /// <returns>The file name which contains the HTML</returns>
    internal string PrintPairings(Tournament tournament, bool result)
    {
        string print = CreateOutputForPairings(tournament, false, result);
        if (!Directory.Exists(TempPath))
        {
            Directory.CreateDirectory(TempPath);
        }

        using (StreamWriter sw = new StreamWriter(PrintFile, false))
        {
            sw.Write(print);
        }

        return PrintFile;
    }

    /// <summary>
    /// Save the the tournament to a file
    /// </summary>
    /// <param name="tournament">The tournament that should be saved</param>
    /// <param name="autosave">true if it's an autosave</param>
    /// <param name="getResultsText">Text of the getResultsButton</param>
    /// <param name="buttonCut">State of the Cut Button</param>
    /// <param name="Autosavetype">Type of autosave, default ""</param>
    internal void Save(Tournament tournament, bool autosave, string getResultsText, bool? buttonCut,
        string Autosavetype = "")
    {
        StringBuilder sb = new StringBuilder();
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
            file = Path.Combine(file,
                $"{Literals.Autosave}_{DateTime.Now.ToFileTime()}_{name}_{Autosavetype}.{Settings.FILEEXTENSION}");
        }
        else
        {
            fileManager.AddFilter($"*.{Settings.FILEEXTENSION}", Settings.FILEEXTENSIONSNAME);
            if (fileManager.Save())
            {
                file = fileManager.FileName;
            }
            else
                return;
        }

        if (!file.EndsWith(Settings.FILEEXTENSION))
        {
            sb.Clear();
            sb.Append(file);
            sb.Append(Settings.FILEEXTENSION);
            file = sb.ToString();
        }

        try
        {
            using (StreamWriter stream = new StreamWriter(file,
                       new FileStreamOptions()
                           { Mode = FileMode.Create, Access = FileAccess.Write, Share = FileShare.None }))
            {
                stream.WriteLine(JsonSerializer.Serialize(tournament, options));
            }
        }
        catch (Exception)
        {
            sb.Clear();
            sb.Append(AutosavePath);
            sb.Append(
                $"{Path.PathSeparator}{Literals.Autosave}_{DateTime.Now.ToFileTime()}_{Literals.ATournament}_{Autosavetype}.{Settings.FILEEXTENSION}");
            file = sb.ToString();
            using (StreamWriter stream = new StreamWriter(file,
                       new FileStreamOptions()
                           { Mode = FileMode.Create, Access = FileAccess.Write, Share = FileShare.None }))
            {
                stream.WriteLine(JsonSerializer.Serialize(tournament, options));
            }
        }
    }

    /// <summary>
    /// Print a ScoreSheet
    /// </summary>
    /// <param name="tournament">The tournament for what a score sheet should be printed</param>
    /// <returns>A string to the Path of the file with the Scoresheets</returns>
    internal string PrintScoreSheet(Tournament tournament)
    {
        string title = $"{tournament.Name} - {Texts.Pairings} - {Texts.Round} {tournament.DisplayedRound}";
        List<string> print = new List<string>();
        string temp = $"<!DOCTYPE html><html><head><title>{title}</title></head><body>";
        print.Add(temp);
        title = $"Round {tournament.DisplayedRound} - {Texts.Table}";
        foreach (Pairing p in tournament.Rounds[tournament.Rounds.Count - 1].Pairings)
        {
            temp =
                $"<table width=100%><tr><td><h4>{title} {p.TableNo}</h4></td><td><h4>{p.Player1Name}</h4></td><td><h4>{p.Player2Name}</h4></td></tr>";
            print.Add(temp);
            temp = "<tr><td></td><td>Points destroyed    _________</td><td>Points destroyed    _________</td></tr>";
            print.Add(temp);
            temp = "</table><hr/>";
            print.Add(temp);
        }

        temp = "</body></html>";
        print.Add(temp);
        if (!Directory.Exists(TempPath))
        {
            Directory.CreateDirectory(TempPath);
        }

        using (StreamWriter sw = new StreamWriter(PrintFile, false, Encoding.UTF8))
        {
            foreach (string s in print)
            {
                sw.WriteLine(s);
            }
        }

        return PrintFile;
    }

    /// <summary>
    /// Saves the current settings
    /// </summary>
    internal void SaveSettings()
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        using (var sw = new StreamWriter(SettingsFile, false))
        {
            sw.Write(JsonSerializer.Serialize(ActiveSettings));
        }
    }

    #endregion

    #region internal methods

    /// <summary>
    /// Gets all save files in the auto save folder
    /// </summary>
    /// <returns>Returns all save files as string</returns>
    internal string[] GetAutosaveFiles()
    {
        return Directory.GetFiles(AutosavePath, "*" + Settings.FILEEXTENSION);
    }

    /// <summary>
    /// Shows a Message Box with OK and Cancel Options
    /// </summary>
    /// <param name="text">The text that should be shown</param>
    /// <returns>True if the User clicked OK and false if not.</returns>
    internal bool ShowMessageWithOKCancel(string text)
    {
        return messageManager.ShowWithOKCancel(text);
    }

    /// <summary>
    /// Shows a Message Box
    /// </summary>
    /// <param name="text">The Text that should be shown</param>
    internal void ShowMessage(string text)
    {
        messageManager.Show(text);
    }

    /// <summary>
    /// Opens the autosave folder in the default file manager
    /// </summary>
    internal void OpenAutosaveFolder()
    {
        if (AutosavePathExists)
        {
            Process.Start("file://" + AutosavePath);
        }
        else
        {
            messageManager.Show(Texts.NoAutosaveFolder);
        }
    }

    /// <summary>
    /// deletes the autosave folder
    /// </summary>
    internal void DeleteAutosaveFolder()
    {
        if (AutosavePathExists)
        {
            Directory.Delete(AutosavePath, true);
            messageManager.Show(Texts.AutoSaveFolderDeleted);
        }
        else
        {
            messageManager.Show(Texts.NoAutosaveFolder);
        }
    }

    #endregion

    #region private methods

    /// <summary>
    /// Splits a line from the GOEPP file and creates a Player Object
    /// </summary>
    /// <param name="line">The line which should be converted</param>
    /// <returns>A player object with the data from the line</returns>
    private Player ConvertGOEPPLineToPlayer(string line)
    {
        string[] splitedLine = new string[11];
        int sepBegin = 0, sepEnd;
        for (int i = 0; i < 11; i++)
        {
            sepEnd = line.IndexOf("|", sepBegin);
            splitedLine[i] = line.Substring(sepBegin, sepEnd - sepBegin);
            sepBegin = sepEnd + 2;
        }

        return new Player(Int32.Parse(splitedLine[0]), splitedLine[1], splitedLine[2], splitedLine[3], splitedLine[4],
            splitedLine[5], splitedLine[6], Int32.Parse(splitedLine[9]) == 1, Int32.Parse(splitedLine[7]) == 3);
    }

    /// <summary>
    /// Splits a line from the csv file and creates a Player Object
    /// </summary>
    /// <param name="line">The line which should be converted</param>
    /// <returns>A player object with the data from the line</returns>
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

        return new Player(0, splitedLine[0], splitedLine[1], splitedLine[2], splitedLine[4], "", splitedLine[3],
                splitedLine[5].ToUpper() == "X", splitedLine[6].ToUpper() == "X")
            { HasWonBye = splitedLine[7].ToUpper() == "X", SquadList = splitedLine[8] };
    }

    /// <summary>
    /// Writes a line to the GOEPP Export file
    /// </summary>
    /// <param name="file">The file name where to write the line</param>
    /// <param name="line">The line which should be written</param>
    private void WriteGOEPPLine(string file, string line)
    {
        using (StreamWriter f = new StreamWriter(file, true, Encoding.GetEncoding(28591)))
        {
            f.WriteLine(line + "||x");
        }
    }

    #endregion
}