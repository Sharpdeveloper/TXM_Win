using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

using Octokit;

using TXM.Core.Export.JSON;
using TXM.Core.Models;

namespace TXM.Core.Global;

public sealed class InputOutput
{
    private InputOutput()
    {
        SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TXM");
        AutosavePath = Path.Combine(SavePath, "Autosave");
        LanguagePath = Path.Combine(SavePath, "Languages");
        TempPath = Path.Combine(SavePath, "Temp");
        PrintFile = Path.Combine(TempPath, "print.html");
        SettingsFile = Path.Combine(SavePath, "settings.json");
        options = new JsonSerializerOptions { WriteIndented = true };
    }

    private static InputOutput _instance = new InputOutput();
    public static InputOutput GetInstance() => _instance;
    public IFile FileManager { get; set; }
    public IMessage MessageManager { get; set; }
    public string AutosavePath { get; private set; }
    public string TempPath { get; private set; }
    public string LanguagePath { get; private set; }
    public string PrintFile { get; private set; }
    private string SettingsFile { get; set; }
    public string SavePath { get; private set; }
    public string TempImgPath { get; private set; }
    private int imgnr = 0;
    private string imgending;
    private JsonSerializerOptions options;
    public Settings ActiveSettings { get; set; }

    public bool AutosavePathExists => Directory.Exists(AutosavePath);

    #region internal methods - import / load

    /// <summary>
    /// GOEPP Import File from tabletoptournaments.net (T3)
    /// </summary>
    /// <returns>A Tournament Object from the Date in the file or null if the file is invalid or the file chooser is canceled</returns>
    internal Logic.Tournament? GOEPPImport()
    {
        FileManager.AddFilter("*.gip", $"GÖPP {State.Text.ImportFile}");
        if (FileManager.Open())
        {
            try
            {
                List<string> gipFile = new List<string>();
                using (StreamReader sr = new StreamReader(FileManager.FileName, Encoding.GetEncoding(28591)))
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
                Logic.Tournament tournament =
                    new Logic.Tournament(Int32.Parse(gipFile[2]), gipFile[1], null, gipFile[0]);
                for (int i = 4; i < gipFile.Count; i++)
                {
                    tournament.AddPlayer(ConvertGOEPPLineToPlayer(gipFile[i]));
                }

                //tournament.TeamProtection = false;
                return tournament;
            }
            catch (Exception)
            {
                MessageManager.Show(State.Text.InvalidFile.Replace("<filetype>", "gip"));
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Imports a tournament from a csv file
    /// </summary>
    /// <returns>A tournamnet or null if the file is invalid or the file chooser was canceld</returns>
    internal Logic.Tournament? CSVImport()
    {
        FileManager.AddFilter("*.csv", $"{State.Text.ExcelFile} (CSV)");
        if (FileManager.Open())
        {
            try
            {
                List<string> csvFile = new List<string>();
                using (StreamReader sr = new StreamReader(FileManager.FileName))
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
                Logic.Tournament tournament =
                    new Logic.Tournament(
                        FileManager.FileName.Split(Path.DirectorySeparatorChar)[
                            FileManager.FileName.Split(Path.DirectorySeparatorChar).Length - 1].Split('.')[0], null);
                for (int i = 1; i < csvFile.Count; i++)
                {
                    tournament.AddPlayer(ConvertCSVToPlayer(csvFile[i]));
                }

                tournament.TeamProtection = false;
                return tournament;
            }
            catch (Exception)
            {
                MessageManager.Show(State.Text.InvalidFile.Replace("<filetype>", "csv"));
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Adds Data from a csv file to an already imported / created tournament
    /// </summary>
    internal void CSVImportAdd()
    {
        var tournament = State.Controller.ActiveTournament;
        FileManager.AddFilter("*.csv", $"{State.Text.ExcelFile} (CSV)");
        if (FileManager.Open())
        {
            try
            {
                List<string> csvFile = new List<string>();
                using (StreamReader sr = new StreamReader(FileManager.FileName))
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
                    var p = tournament.Participants.Where(x => x.Nickname == player.Nickname).First();
                    ;
                    if (p == null)
                    {
                        p = tournament.Participants.Where(x => x.Name == player.Name)
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
                MessageManager.Show(State.Text.InvalidFile.Replace("<filetype>", "csv"));
            }
        }
    }

    /// <summary>
    /// Load a tournament from the filesystem
    /// </summary>
    /// <param name="filename">Optional filename</param>
    /// <returns>A tournament object or null</returns>
    public Logic.Tournament? Load(string filename = "")
    {
        string file;
        if (filename != "")
        {
            file = filename;
        }
        else
        {
            FileManager.AddFilter("*." + Settings.FileExtension, Settings.FileExtensionsName);
            if (FileManager.Open())
            {
                file = FileManager.FileName;
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
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line);
                }
            }

            return JsonSerializer.Deserialize<Logic.Tournament>(sb.ToString());
        }
        catch (Exception)
        {
            MessageManager.Show(State.Text.InvalidFile.Replace("<filetype>", Settings.FileExtension));
            return null;
        }
    }

    /// <summary>
    /// Load the settings from the filesystem
    /// </summary>
    public Settings LoadSettings()
    {
        if (!File.Exists(SettingsFile))
        {
            return Settings.GetInstance();
        }
        var sb = new StringBuilder();
        using (StreamReader sr = new StreamReader(SettingsFile))
        {
            while (sr.ReadLine() is { } line)
            {
                sb.Append(line);
            }
        }
        return JsonSerializer.Deserialize<Settings>(sb.ToString(), options);
    }
    
    /// <summary>
    /// Load the language from the filesystem
    /// </summary>
    public Texts LoadLanguage(string language)
    {
        var file = Path.Combine(LanguagePath, $"{language}.json");
        //var file = Path.Combine(LanguagePath, $"{language}.json");
        if (!File.Exists(file))
        {
            return Texts.GetInstance();
        }
        var sb = new StringBuilder();
        using (StreamReader sr = new StreamReader(file))
        {
            while (sr.ReadLine() is { } line)
            {
                sb.Append(line);
            }
        }

        return (Texts) JsonSerializer.Deserialize<Texts>(sb.ToString(), options);
        //Texts.SetInstance(JsonSerializer.Deserialize<Texts>(sb.ToString(), options));
        //State.Text = Texts.GetInstance();
    }

    /// <summary>
    /// Let the User choose a new background image for the timer window
    /// </summary>
    /// <returns>True if it's a new image</returns>
    public (bool NewImage, string Path) NewImage()
    {
        FileManager.AddFilter("*.jpg;*.jpeg;*.png;*.tif;*.tiff", State.Text.ImageFiles);
        return FileManager.Open() ? (true, FileManager.FileName) : (false, "");
    }

    #endregion

    #region internal methods - export / print / Save

    /// <summary>
    /// Saves the current tournement in a GOEPP Export file for upload on tabletoptournaments.net (T3)
    /// </summary>
    /// <param name="tournament">The tournament that should be exported</param>
    internal void GOEPPExport(Logic.Tournament tournament)
    {
        FileManager.AddFilter("*.gep", $"GÖPP {State.Text.ExportFile}");

        if (FileManager.Save())
        {
            string file = FileManager.FileName;
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
            foreach (Models.Player p in tournament.Participants)
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
    internal string CreateOutputForTable(Logic.Tournament tournament, bool bbcode)
    {
        StringBuilder sb = new StringBuilder();
        string title = tournament.Name + $" - {State.Text.Table} - {State.Text.Round} {tournament.DisplayedRound}";

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
        sb.Append(State.Text.Firstname); //Firstname
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.Nickname); //Nickname
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.Team); //Team
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.Faction); //Faction
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.TournamentPointsShort); //Tournamentpoints
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.WinsShort); //Wins
        sb.Append(de);
        if (tournament.Rule.OptionalFields.Contains(Literals.ModWins))
        {
            sb.Append(db);
            sb.Append(State.Text.ModifiedWinsShort); //Modified Wins
            sb.Append(de);
        }

        if (tournament.Rule.OptionalFields.Contains(Literals.Draws))
        {
            sb.Append(db);
            sb.Append(State.Text.DrawsShort); //Draws
            sb.Append(de);
        }

        if (tournament.Rule.OptionalFields.Contains(Literals.ModLoss))
        {
            sb.Append(db);
            sb.Append(State.Text.ModifiedLossesShort); //Modified Loss
            sb.Append(de);
        }

        sb.Append(db);
        sb.Append(State.Text.Losses); //Losses
        sb.Append(de);
        if (tournament.Rule.OptionalFields.Contains(Literals.MoV))
        {
            sb.Append(db);
            sb.Append(State.Text.MarginOfVictoryShort); //Margin of Victory
            sb.Append(de);
        }

        sb.Append(db);
        sb.Append(State.Text.StrengthOfScheduleShort); //Strength of Schedule
        sb.Append(de);
        if (tournament.Rule.OptionalFields.Contains(Literals.ESoS))
        {
            sb.Append(db);
            sb.Append(State.Text.ExtendedStrengthOfScheduleShort); //extended Strength of Schedule
            sb.Append(de);
        }

        sb.Append(re);
        sb.Append(nl);

        var participants = tournament.Participants;

        foreach (Models.Player p in participants)
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
            if (tournament.Rule.OptionalFields.Contains(Literals.ESoS))
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
    internal string CreateOutputForPairings(Logic.Tournament tournament, bool bbcode, bool result)
    {
        if (tournament.Rounds.Count == 0)
        {
            if (bbcode)
            {
                return $"[b]{State.Text.NoRoundStarted} => {State.Text.NoPairingsYet}[/b]";
            }
            else
            {
                return $"<html><body><h1>{State.Text.NoRoundStarted} => {State.Text.NoPairingsYet}</h1></body></html>";
            }
        }

        StringBuilder sb = new StringBuilder();
        string head, tb, te, rb, re, db, de, end, nl = Environment.NewLine;
        string title = tournament.Name +
                       $" - {(result ? State.Text.Results : State.Text.Pairings)} - {State.Text.Round} {tournament.DisplayedRound}";


        if (bbcode)
        {
            head = $"[b]{title}[/b]";
            if (tournament.Rule.UsesScenarios)
            {
                head += $" [u]Scenario: {tournament.ChosenScenario}[/u]";
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
                head += $"<h3>Scenario: {tournament.ChosenScenario}</h3><br />";
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
        sb.Append(State.Text.TableNoShort); //Table Number
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.Player1); //Player 1
        sb.Append(de);
        sb.Append(db);
        sb.Append(State.Text.Player2); //Player 2
        sb.Append(de);
        if (result)
        {
            sb.Append(db);
            sb.Append(State.Text.Score); //Score
            sb.Append(de);
        }

        if (result && tournament.Rule.IsDrawPossible)
        {
            sb.Append(db);
            sb.Append(State.Text.Winner); //Winner
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
    internal (string, string) GetJsonForListfortress()
    {
        var tournament = State.Controller.ActiveTournament;
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
    internal string PrintPlayerList()
    {
        var tournament = State.Controller.ActiveTournament;
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
    internal string PrintPairings(bool result)
    {
        var tournament = State.Controller.ActiveTournament;
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
    /// <param name="autosave">true if it's an autosave</param>
    /// <param name="Autosavetype">Type of autosave, default ""</param>
    internal void Save(bool autosave, string Autosavetype)
    {
        var tournament = State.Controller.ActiveTournament;
        StringBuilder sb = new StringBuilder();
        string file = "";
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
                $"{Literals.AutoSave}_{DateTime.Now.ToFileTime()}_{name}_{Autosavetype}.{Settings.FileExtension}");
        }
        else
        {
            FileManager.AddFilter($"*.{Settings.FileExtension}", Settings.FileExtensionsName);
            if (FileManager.Save())
            {
                file = FileManager.FileName;
            }
            else
                return;
        }

        if (!file.EndsWith(Settings.FileExtension))
        {
            sb.Clear();
            sb.Append(file);
            sb.Append(Settings.FileExtension);
            file = sb.ToString();
        }

        try
        {
            using (StreamWriter stream = new StreamWriter(file,
                       new FileStreamOptions()
                           { Mode = System.IO.FileMode.Create, Access = FileAccess.Write, Share = FileShare.None }))
            {
                stream.WriteLine(JsonSerializer.Serialize(tournament, options));
            }
        }
        catch (Exception)
        {
            sb.Clear();
            sb.Append(AutosavePath);
            sb.Append(
                $"{Path.PathSeparator}{Literals.AutoSave}_{DateTime.Now.ToFileTime()}_{Literals.ATournament}_{Autosavetype}.{Settings.FileExtension}");
            file = sb.ToString();
            using (StreamWriter stream = new StreamWriter(file,
                       new FileStreamOptions()
                           { Mode = System.IO.FileMode.Create, Access = FileAccess.Write, Share = FileShare.None }))
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
    internal string PrintScoreSheet()
    {
        var tournament = State.Controller.ActiveTournament;
        string title = $"{tournament.Name} - {State.Text.Pairings} - {State.Text.Round} {tournament.DisplayedRound}";
        List<string> print = new List<string>();
        string temp = $"<!DOCTYPE html><html><head><title>{title}</title></head><body>";
        print.Add(temp);
        title = $"Round {tournament.DisplayedRound} - {State.Text.Table}";
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
            sw.Write(JsonSerializer.Serialize(State.Setting, options));
        }
    }
    
    /// <summary>
    /// Saves the current language
    /// </summary>
    public void SaveLanguage()
    {
        var dir = Path.Combine(SavePath, "LangExport");
        var file = Path.Combine(dir, $"{State.Setting.Language}.json");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using (var sw = new StreamWriter(file, false))
        {
            sw.Write(JsonSerializer.Serialize(State.Text, options));
        }
    }

    #endregion

    #region public methods

    /// <summary>
    /// Gets all save files in the auto save folder
    /// </summary>
    /// <returns>Returns all save files as string</returns>
    public string[] GetAutosaveFiles()
    {
        return Directory.GetFiles(AutosavePath, "*" + Settings.FileExtension);
    }

    /// <summary>
    /// Shows a Message Box with OK and Cancel Options
    /// </summary>
    /// <param name="text">The text that should be shown</param>
    /// <returns>True if the User clicked OK and false if not.</returns>
    public bool ShowMessageWithOKCancel(string text)
    {
        return MessageManager.ShowWithOKCancel(text);
    }

    /// <summary>
    /// Shows a Message Box
    /// </summary>
    /// <param name="text">The Text that should be shown</param>
    public void ShowMessage(string text)
    {
        MessageManager.Show(text);
    }

    /// <summary>
    /// Opens the autosave folder in the default file manager
    /// </summary>
    public void OpenAutosaveFolder()
    {
        if (AutosavePathExists)
        {
            var uri = "file://" + AutosavePath;
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }
        else
        {
            MessageManager.Show(State.Text.NoAutoSaveFolder);
        }
    }

    /// <summary>
    /// deletes the autosave folder
    /// </summary>
    public void DeleteAutosaveFolder()
    {
        if (AutosavePathExists)
        {
            Directory.Delete(AutosavePath, true);
            MessageManager.Show(State.Text.AutoSaveFolderDeleted);
        }
        else
        {
            MessageManager.Show(State.Text.NoAutoSaveFolder);
        }
    }

    public (List<LocalFile> Files, char Separator) GetLanguages() =>
        (GetLocalLanguages(), Path.DirectorySeparatorChar);
    

    public (List<LocalFile> Files, char Separator) CheckLanguages()
    {
        var localFiles = GetLocalLanguages();
        var onlineFiles = GetOnlineLanguages();

        var newOrUpdatedFiles = new List<LocalFile>();
        foreach (var file in onlineFiles)
        {
            var l = localFiles.Where(x => x.FileName == file.FileName).ToArray();
            if (l.Length == 1)
            {
                if (l[0].Size != file.Size)
                {
                    newOrUpdatedFiles.Add(file);
                }
            }
            else
            {
                newOrUpdatedFiles.Add(file);
            }
        }

        if (newOrUpdatedFiles.Count > 0)
        {
            using (var client = new WebClient())
            {
                foreach (var file in newOrUpdatedFiles)
                {
                    client.DownloadFile(file.Path, Path.Combine(LanguagePath, $"{file.FileName}.json"));
                }
            }

            localFiles = GetLocalLanguages();
        }
        
        return (localFiles, Path.DirectorySeparatorChar );
    }

    private List<LocalFile> GetLocalLanguages()
    {
        if (!Directory.Exists(LanguagePath))
        {
            return new();
        }

        return Directory.GetFiles(LanguagePath).Select(AddFileInfos).ToList();
    }

    private LocalFile AddFileInfos(string path)
    {
        var fileName = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1
            , path.Length - path.LastIndexOf(Path.DirectorySeparatorChar) - 6);
        var fI = new FileInfo(path);

        return new()
        {
            FileName = fileName, Path = path, Size = fI.Length
        };
    }

    private List<LocalFile> GetOnlineLanguages()
    {
        if (!IsOnline())
        {
            return new();
        }

        var client = new GitHubClient(new ProductHeaderValue("TXM", "1"));
        client.Credentials = Credentials.Anonymous;
        var files = client.Repository.Content.GetAllContentsByRef(83980951, "Languages", "Way-to-TXM-4.0").Result;

        return files.Select(x => new LocalFile()
        {
            FileName = x.Name.Substring(0, (x.Name.Length - 5)), Path = x.DownloadUrl, Size = x.Size
        }).ToList();
    }

    #endregion

    #region private methods

    private bool IsOnline()
    {
        try
        {
            using var client = new WebClient();
            using var stream = client.OpenRead("https://www.github.com");
            return true;
        }
        catch
        {
            // Ignore
        }

        return false;
    }

    /// <summary>
    /// Splits a line from the GOEPP file and creates a Player Object
    /// </summary>
    /// <param name="line">The line which should be converted</param>
    /// <returns>A player object with the data from the line</returns>
    private Models.Player ConvertGOEPPLineToPlayer(string line)
    {
        string[] splitedLine = new string[11];
        int sepBegin = 0, sepEnd;
        for (int i = 0; i < 11; i++)
        {
            sepEnd = line.IndexOf("|", sepBegin);
            splitedLine[i] = line.Substring(sepBegin, sepEnd - sepBegin);
            sepBegin = sepEnd + 2;
        }

        return new Models.Player(Int32.Parse(splitedLine[0]), splitedLine[1], splitedLine[2], splitedLine[3]
            , splitedLine[4],
            splitedLine[5], splitedLine[6], Int32.Parse(splitedLine[9]) == 1, Int32.Parse(splitedLine[7]) == 3);
    }

    /// <summary>
    /// Splits a line from the csv file and creates a Player Object
    /// </summary>
    /// <param name="line">The line which should be converted</param>
    /// <returns>A player object with the data from the line</returns>
    private Models.Player ConvertCSVToPlayer(string line)
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

        return new Models.Player(0, splitedLine[0], splitedLine[1], splitedLine[2], splitedLine[4], "", splitedLine[3],
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