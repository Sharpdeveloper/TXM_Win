using System.Text.Json.Serialization;

using Microsoft.VisualBasic;

namespace TXM.Core.Global;

/// <summary>
/// Texts.cs
/// class which contains all texts which can be outputted.
/// These enables translation files
/// </summary>
public sealed class Texts
{
    public Texts() { }
    private static Texts _instance = new Texts();
    public static Texts GetInstance() => _instance;

    public static void SetInstance(Texts _newInstance)
    {
        _instance = _newInstance;
    }

    [JsonIgnore]
    public string AboutText { get; set; } =
        $"© {Global.Settings.CopyRightYear} Sharpdeveloper aka TKundNobody\nTXM Version: {Global.Settings.TxmVersion}\n© Icons: Icons8 (www.icons8.com)";

    public string Automatic { get; set; } = "Automatic";
    public string AutoSaveTitle { get; set; } = "AutoSave Dialog";
    public string AutoSaveFolderDeleted { get; set; } = "Auto save folder was deleted.";
    public string BackgroundImage { get; set; } = "Choose a new background image:";

    public string BackgroundImageInfo { get; set; } =
        "This background image will be shown in the timer window. The file must have one of the following endings: " +
        Strings.Join(Literals.ImageEndings, " ,");

    public string BBCodeGenerator { get; set; } = "BBCode Generator";
    public string Cancel { get; set; } = "Cancel";
    public string CheckLanguages { get; set; } = "Check for new / updated Languages.";
    public string ChooseGame { get; set; } = "Choose the game:";
    public string ChooseGameInfo { get; set; } = "Choose the game for which you want to run the tournament.";
    public string ChooseLanguage { get; set; } = "Choose the Language of TXM";

    public string ChooseLanguageInfo { get; set; } =
        "You can choose between the downloaded languages. If you want to get new or updated languages, please use the link below the box.";
    public string ChooseTextColor { get; set; } = "Choose the text color for the timer.";

    public string ChooseTextColorInfo { get; set; } =
        "The text on the timer window will have the chosen color.";

    public string ChooseTournamentType { get; set; } = "Choose the Tournament type:";
    public string ChooseTournamentTypeInfo { get; set; } = "Choose how the tournament will be handled.";
    public string ChosenScenario { get; set; } = "Chosen Scenario";
    public string ClipboardInfo { get; set; } = "The Tournament is in your clipboard you can paste it to <site>.";

    public Dictionary<string, string> Colors { get; set; } = new()
    {
        { Literals.Black, "Black" }, { Literals.Blue, "Blue" }, { Literals.Green, "Green" }
        , { Literals.Orange, "Orange" }, { Literals.Purple, "Purple" }, { Literals.Red, "Red" }
        , { Literals.White, "White" }, { Literals.Yellow, "Yellow" }
    };

    public string CopyToClipboard { get; set; } = "Copy to Clipboard";
    public string CutSize { get; set; } = "How many player are in the top cut?";

    public string CutSizeInfo { get; set; } =
        "If the tournament has no cut, the number should be 0. If it has a cut enter the number of players who can get to the cut.";

    public string Date { get; set; } = "Date";
    public string Disqualified { get; set; } = "disqualified";
    public string Disqualify { get; set; } = "disqualify";
    public string Draws { get; set; } = "Draws";
    public string DrawsShort { get; set; } = "D";
    public string Drop { get; set; } = "drop";
    public string Dropped { get; set; } = "dropped";
    public string EmptyTournamentWarning { get; set; } = "Tournament can't be started without player.";
    public string ExcelFile { get; set; } = "Excel File";
    public string ExportFile { get; set; } = "Export File";
    public string ExportInfo { get; set; } = "Alternative you find the export data here:";
    public string ExtendedStrengthOfSchedule { get; set; } = "extended Strength of Schedule";
    public string ExtendedStrengthOfScheduleShort { get; set; } = "eSoS";
    public string Faction { get; set; } = "Faction";
    public string FactionInfo { get; set; } = "The faction of the players list.";
    public string Firstname { get; set; } = "Firstname";
    public string FirstnameInfo { get; set; } = "The first name of the player.";
    public string FirstRound { get; set; } = "First round";
    public string FixedTable { get; set; } = "Fixed table no. (if any)";

    public string FixedTableInfo { get; set; } =
        "Enter a table no, if the player should always be seated at this table. This is useful for wheelchair drivers, streamers, or other player which need to stay at one table.";

    public string GameSystemNotChangeable { get; set; } = "The game system is not changeable.";
    public string GetResults { get; set; } = "Get Results";
    public string GetResultsInfo { get; set; } = "Get the results from the current round";
    public string ImageFiles { get; set; } = "Image Files";
    public string ImportFile { get; set; } = "Import File";
    public string InvalidFile { get; set; } = "Please choose a valid <filetype>-file.";
    public string InvalidHour { get; set; } = "The hour can only between 0 and 23.";
    public string InvalidMinute { get; set; } = "The minute can only between 0 and 59.";
    public string InvalidResult { get; set; } = "One ore more results are invalid.";
    public string LastName { get; set; } = "Last name";
    public string LastNameInfo { get; set; } = "The last name of the player.";
    public string ListGiven { get; set; } = "Has List given";

    public string ListGivenInfo { get; set; } =
        "Check, if the player has given his army / squad list to the TO (if any).";

    public string ListGivenShort { get; set; } = "L";
    public string Lock { get; set; } = "Locks the Pairing, so it can't be accidentally changed.";
    public string LockShort { get; set; } = "L";
    public string Losses { get; set; } = "Losses";
    public string LossesShort { get; set; } = "L";
    public string MarginOfVictory { get; set; } = "Margin of Victory";
    public string MarginOfVictoryShort { get; set; } = "MoV";
    public string MaxPoints { get; set; } = "Max Listpoints:";

    public string MaxPointsInfo { get; set; } =
        "The maximum of points for the lists (if the game system uses any).";

    public string MenuAbout { get; set; } = "About TXM";
    public string MenuAutoSaveFiles { get; set; } = "AutoSave Files";
    public string MenuBBCODE { get; set; } = "Get BBCode for Forum";
    public string MenuBonusPoints { get; set; } = "Award bonus points";
    public string MenuCalculateWonByes { get; set; } = "Calculate SoS for won byes";
    public string MenuChangePairings { get; set; } = "Change Pairings";
    public string MenuCSVAdd { get; set; } = "Add Information from Excel (CSV)";
    public string MenuCSVImport { get; set; } = "Excel (CSV) Import";
    public string MenuDeleteAutoSaveFolder { get; set; } = "Delete AutoSave Folder";
    public string MenuDeleteDropPlayer { get; set; } = "Delete / Drop Player";
    public string MenuDisqualifyPlayer { get; set; } = "Disqualify Player";
    public string MenuEditPlayer { get; set; } = "Edit Player";
    public string MenuEditTournament { get; set; } = "Edit Tournament";
    public string MenuExit { get; set; } = "Close TXM";
    public string MenuFile { get; set; } = "File";
    public string MenuImportExport { get; set; } = "Import/Export";
    public string MenuInfo { get; set; } = "Info";
    public string MenuListFortressExport { get; set; } = "Export for ListFortress.com";
    public string MenuLoad { get; set; } = "Load";
    public string MenuLoadAutoSaveFiles { get; set; } = "Load AutoSave Files";
    public string MenuManual { get; set; } = "User Manual (online; english)";
    public string MenuNewPlayer { get; set; } = "New Player";
    public string MenuNewTournament { get; set; } = "New Tournament";
    public string MenuOpenAutoSaveFolder { get; set; } = "Open AutoSave Folder";
    public string MenuPlayerManagement { get; set; } = "Player Management";
    public string MenuPrint { get; set; } = "Print";
    public string MenuPrintPairingsWith { get; set; } = "Print Pairings (with Results)";
    public string MenuPrintPairingsWithout { get; set; } = "Print Pairings (without Results)";
    public string MenuPrintScoreSheets { get; set; } = "Print Score Sheet";
    public string MenuPrintTable { get; set; } = "Print Table";
    public string MenuResetLastResults { get; set; } = "Reset last results";
    public string MenuSave { get; set; } = "Save";
    public string MenuSettings { get; set; } = "Settings";
    public string MenuShowPairings { get; set; } = "Show Pairings";
    public string MenuShowTable { get; set; } = "Show Table";
    public string MenuSupport { get; set; } = "Support TXM";
    public string MenuT3Export { get; set; } = "T3 Export";
    public string MenuT3Import { get; set; } = "T3 Import";
    public string MenuThanks { get; set; } = "Special Thanks to...";
    public string MenuTimer { get; set; } = "Timer";
    public string MenuTools { get; set; } = "Tools";
    public string MenuTournament { get; set; } = "Tournament";
    public string MenuView { get; set; } = "View";
    public string Minutes { get; set; } = "Minutes";
    public string MissingResult { get; set; } = "There is at least one result missing.";
    public string ModifiedLosses { get; set; } = "Modified Losses";
    public string ModifiedLossesShort { get; set; } = "ML";
    public string ModifiedWins { get; set; } = "Modified Wins";
    public string ModifiedWinsShort { get; set; } = "MW";
    public string NewLanguageInfo { get; set; } = "To apply the new language, you have to restart TXM.";
    public string NewPlayer { get; set; } = "New Player";
    public string NewTournament { get; set; } = "New Tournament";
    public string NextRound { get; set; } = "Next Round";
    public string NextRoundInfo { get; set; } = "Pair the next round";
    public string Nickname { get; set; } = "Nickname";
    public string NicknameInfo { get; set; } = "The nickname / gamer name of the player.";
    public string NoAutoSaveFolder { get; set; } = "There is no auto save folder.";
    public string None { get; set; } = "None";
    public string NoPairingsYet { get; set; } = "No Pairings yet";
    public string NoRoundStarted { get; set; } = "No Round started";
    public string OverwrittenWarning { get; set; } = "The current tournament will be overwritten.";
    public string Paid { get; set; } = "Paid";
    public string PaidInfo { get; set; } = "Check, if the player paid the tournament fee (if any).";
    public string PaidShort { get; set; } = "$";
    public string Pairings { get; set; } = "Pairings";
    public string Player { get; set; } = "Player";
    public string PlayerShort { get; set; } = "P";
    public string Points { get; set; } = "Points";
    public string Present { get; set; } = "Is Present";
    public string PresentInfo { get; set; } = "Check, if the player is present.";
    public string PresentShort { get; set; } = "!";
    public string Protection { get; set; } = "Which team protection do you want?";

    public string ProtectionInfo { get; set; } =
        "Team protection prevents team members to get paired against each other. Only recommended for casual events.";

    public string Ok { get; set; } = "OK";
    public string RandomMinutes { get; set; } = "How many Minutes should at max randomized?";

    public string RandomMinutesInfo { get; set; } =
        "The timer will random add or subtract a number of minutes to the maximum which is entered here.";

    public string RandomTime { get; set; } = "Shall the time be randomized?";

    public string RandomTimeInfo { get; set; } =
        "The timer can add/subtract a random amount of minutes from the time.";

    public string Rank { get; set; } = "Rank";
    public string RankShort { get; set; } = "#";
    public string Remove { get; set; } = "remove";
    public string ActionWarning { get; set; } = "Do you really want to <action> <player>?";
    public string ResultEdited { get; set; } = "Is the result edited?";
    public string ResultEditedShort { get; set; } = "OK?";
    public string Results { get; set; } = "Results";
    public string Round { get; set; } = "Round";
    public string Save { get; set; } = "Save";
    public string SaveStateChange { get; set; } = "Change_Pairings";
    public string SaveStatePairing { get; set; } = "Pairings_Round_";
    public string SaveStateSeeding { get; set; } = "Seeding_Round_";
    public string SaveStateStart { get; set; } = "Tournament_Start";
    public string Score { get; set; } = "Score";
    public string Select { get; set; } = "Select";
    public string SelectScenario { get; set; } = "Select Scenario:";
    public string SelectScenarioInfo { get; set; } = "Here you can select the Scenario manually or let TXM choose randomly.";
    public string Settings { get; set; } = "Settings";
    public string Single { get; set; } = "Single";
    public string SquadList { get; set; } = "Army / Squad list (if any)";

    public string SquadListInfo { get; set; } =
        "A link to the the army / squad list of the player (if the game system uses any).";

    public string Standings { get; set; } = "Standings";
    public string StartCut { get; set; } = "Start Cut";
    public string StartCutInfo { get; set; } = "End the SWISS rounds and start the final cut.";
    public string StartTime { get; set; } = "Should the timer automatically start at:";
    public string StartTimeInfo { get; set; } = "The timer will automatically start at the given time.";

    public string StartTimeFormat { get; set; } =
        "The Time is only allowed in the format HH:MM. AM/PM not supported.";

    public string Start { get; set; } = "Start";
    public string StartInfo { get; set; } = "Start Tournament";
    public string State { get; set; } = "State";
    public string StrengthOfSchedule { get; set; } = "Strength of Schedule";
    public string StrengthOfScheduleShort { get; set; } = "SoS";
    public string SupportTitle { get; set; } = "Support";
    public string Swiss { get; set; } = "Swiss";
    public string Table { get; set; } = "Table";
    public string TableShort { get; set; } = "T";
    public string TableNo { get; set; } = "Table Number";
    public string TableNoShort { get; set; } = "T#";
    public string Team { get; set; } = "Team";
    public string TeamInfo { get; set; } = "The team of the player.";
    public string TextSize { get; set; } = "Size of the text:";
    public string TextSizeInfo { get; set; } = "This is the size for the text in the timer window.";
    public string ThanksTitle { get; set; } = "Thanks";
    public string Time { get; set; } = "Time";
    [JsonIgnore]
    public string TimeFormat { get; set; } = "HH:MM (24h)";
    public string TimerMinutes { get; set; } = "How Long should the timer run?";

    public string TimerMinutesInfo { get; set; } =
        "When you start the Timer it will run the given time in minutes. Changing the tournament game will set the timer to the games default time.";

    public string TimerSettingsPermanent { get; set; } = "Permanent Timer Settings:";
    public string TimerSettingsTemporary { get; set; } = "Temporary Timer Settings:";
    public string TimerTitle { get; set; } = "Timer";
    public string Tournament { get; set; } = "Tournament";
    public string TournamentName { get; set; } = "Tournament name:";
    public string TournamentNameInfo { get; set; } = "The name of the tournament.";
    public string TournamentPoints { get; set; } = "Tournament Points";
    public string TournamentPointsShort { get; set; } = "TP";
    public string TournamentType { get; set; } = "Tournament type:";
    public string TournamentTypeInfo { get; set; } = "The type of the tournament. Default is single.";
    public string Update { get; set; } = "Update";
    public string UpdateInfo { get; set; } = "The currently displayed round will be updated with the changes.";
    public string Winner { get; set; } = "Winner";
    public string Wins { get; set; } = "Wins";
    public string WinsShort { get; set; } = "W";
    public string WonBye { get; set; } = "Won bye";
    public string WonByeInfo { get; set; } = "Check, if the player has a won bye (from a previous event).";

    public string PairingsColon => $"{Pairings}:";
    public string Player1 => $"{Player} 1";
    public string Player2 => $"{Player} 2";
    public string Player1Points => $"{Player} 1 {Points}";
    public string Player1PointsShort => $"{PlayerShort}1 {Points}";
    public string Player2Points => $"{Player} 2 {Points}";
    public string Player2PointsShort => $"{PlayerShort}2 {Points}";
    public string Player1Score => $"{Player} 1 {Score}";
    public string Player1ScoreShort => $"{PlayerShort}1 {Score}";
    public string Player2Score => $"{Player} 2 {Score}";
    public string Player2ScoreShort => $"{PlayerShort}2 {Score}";

    public string ThanksText1 { get; set; } = "Special Thanks to following Friends and Tester";
    public string ThanksText2 { get; set; } = "Tester, User and the Reason for at least half of the features";
    public string ThanksText3 { get; set; } = "Teammate and tester";
    public string ThanksText4 { get; set; } = "Tester";
    public string ThanksText5 { get; set; } = "User who finds every weird error";
    public string ThanksText6 { get; set; } = "Creater of the TXM-Logo";
    public string ThanksText7 { get; set; } = "Tester";
    public string ThanksText8 { get; set; } = "Poweruser";
    public string ThanksText9 { get; set; } = "Tester with the ability to find stranger errors";
    public string ThanksText10 { get; set; } = "Tester";

    [JsonIgnore]
    public string Thanks =>
        $"{ThanksText1}:\n\nBarlmoro - {ThanksText2}\ntgBrain - {ThanksText3}\nKyle_Nemesis - {ThanksText4}\nPhoton - {ThanksText5}\nN4-DO - {ThanksText6}\nMercya - {ThanksText7}\nBackfire84 - {ThanksText8}\nGreenViper - {ThanksText9}\nCarnis - {ThanksText10}";

    public string SupportText1 { get; set; } =
        "If you like TXM and want to support it, you can do it ins several ways:";
    public string SupportText2 { get; set; } = "Spread the word, the more user TXM has, the better it gets!";
    public string SupportText3 { get; set; } = "Test everything!";
    public string SupportText4 { get; set; } = "Help to add more Games. You can find more information in";
    public string SupportText5 { get; set; } = "the online manual (english)";
    public string SupportText6 { get; set; } = "Translate TXM to have TXM in your language. Information also in";
    public string SupportText7 { get; set; } = "the online manual";
    public string SupportText8 { get; set; } = "Leave a tip, if you want:";
    public string SupportText9 { get; set; } = "Donate via";

    [JsonIgnore]
    public string SupportText => $"{SupportText1}\n\n- {SupportText2}\n- {SupportText3}\n- {SupportText4} ";
    [JsonIgnore]
    public string DocuText => $"{SupportText5}";
    [JsonIgnore]
    public string NextText => $"\n- {SupportText6}: ";
    [JsonIgnore]
    public string LangText => $"{SupportText7}";
    [JsonIgnore]
    public string LastText => $"\n- {SupportText8}: ";
    [JsonIgnore]
    public string DonateText => $"{SupportText9} Paypal";

    public string PlayerAction(string player, string action)
    {
        return ActionWarning.Replace("<action>", action).Replace("<player>", player);
    }
}