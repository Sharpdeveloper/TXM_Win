using Microsoft.VisualBasic;

namespace TXM.Core.Global;

/// <summary>
/// Texts.cs
/// class which contains all texts which can be outputted.
/// These enables translation files
/// </summary>
public sealed class Texts
{
    private Texts() { }
    private static Texts _instance = new Texts();
    public static Texts GetInstance() => _instance;

    public bool IsActionBeforeName = true;

    public string AboutText { get; private set; } =
        $"© {Global.Settings.CopyRightYear} Sharpdeveloper aka TKundNobody\nTXM Version: {Global.Settings.TxmVersion}\n© Icons: Icons8 (www.icons8.com)";

    public string Automatic { get; private set; } = "Automatic";
    public string AutoSaveTitle { get; private set; } = "AutoSave Dialog";
    public string AutoSaveFolderDeleted { get; private set; } = "Auto save folder was deleted.";
    public string BackgroundImage { get; private set; } = "Choose a new background image:";

    public string BackgroundImageInfo { get; private set; } =
        "This background image will be shown in the timer window. The file must have one of the following endings: " +
        Strings.Join(Literals.ImageEndings, " ,");

    public string BBCodeGenerator { get; private set; } = "BBCode Generator";
    public string Cancel { get; private set; } = "Cancel";
    public string ChooseGame { get; private set; } = "Choose the game:";
    public string ChooseGameInfo { get; private set; } = "Choose the game for which you want to run the tournament.";
    public string ChooseLanguage { get; private set; } = "Choose the Language of TXM";

    public string ChooseLanguageInfo { get; private set; } =
        "Choose the Language of TXM. New Languages can only be added, if there is an online connection. Otherwise you can only switch between the default and the downloaded languages.";

    public string ChooseTextColor { get; private set; } = "Choose the text color for the timer.";

    public string ChooseTextColorInfo { get; private set; } =
        "The text on the timer window will have the chosen color.";

    public string ChooseTournamentType { get; private set; } = "Choose the Tournament type:";
    public string ChooseTournamentTypeInfo { get; private set; } = "Choose how the tournament will be handled.";
    public string ClipboardInfo { get; private set; } = "The Tournament is in your clipboard you can paste it to";

    public Dictionary<string, string> Colors = new()
    {
        { Literals.Black, "Black" }, { Literals.Blue, "Blue" }, { Literals.Green, "Green" }
        , { Literals.Orange, "Orange" }, { Literals.Purple, "Purple" }, { Literals.Red, "Red" }
        , { Literals.White, "White" }, { Literals.Yellow, "Yellow" }
    };

    public string CopyToClipboard { get; private set; } = "Copy to Clipboard";
    public string CutSize { get; private set; } = "How many player are in the top cut?";

    public string CutSizeInfo { get; private set; } =
        "If the tournament has no cut, the number should be 0. If it has a cut enter the number of players who can get to the cut.";

    public string Date { get; private set; } = "Date";
    public string Disqualified { get; private set; } = "disqualified";
    public string Disqualify { get; private set; } = "disqualify";
    public string Draws { get; private set; } = "Draws";
    public string DrawsShort { get; private set; } = "D";
    public string Drop { get; private set; } = "drop";
    public string Dropped { get; private set; } = "dropped";
    public string EmptyTournamentWarning { get; private set; } = "Tournament can't be started without player.";
    public string ExcelFile { get; private set; } = "Excel File";
    public string ExportFile { get; private set; } = "Export File";
    public string ExportInfo { get; private set; } = "Alternative you find the export data here:";
    public string ExtendedStrengthOfSchedule { get; private set; } = "extended Strength of Schedule";
    public string ExtendedStrengthOfScheduleShort { get; private set; } = "eSoS";
    public string Faction { get; private set; } = "Faction";
    public string FactionInfo { get; private set; } = "The faction of the players list.";
    public string Firstname { get; private set; } = "Firstname";
    public string FirstnameInfo { get; private set; } = "The first name of the player.";
    public string FirstRound { get; private set; } = "First round";
    public string FixedTable { get; private set; } = "Fixed table no. (if any)";

    public string FixedTableInfo { get; private set; } =
        "Enter a table no, if the player should always be seated at this table. This is useful for wheelchair drivers, streamers, or other player which need to stay at one table.";

    public string GameSystemNotChangeable { get; private set; } = "The game system is not changeable.";
    public string GetResults { get; private set; } = "Get Results";
    public string GetResultsInfo { get; private set; } = "Get the results from the current round";
    public string ImageFiles { get; private set; } = "Image Files";
    public string ImportFile { get; private set; } = "Import File";
    public string InvalidFile { get; private set; } = "Please choose a valid <filetype>-file.";
    public string InvalidHour { get; private set; } = "The hour can only between 0 and 23.";
    public string InvalidMinute { get; private set; } = "The minute can only between 0 and 59.";
    public string InvalidResult { get; private set; } = "One ore more results are invalid.";
    public string LastName { get; private set; } = "Last name";
    public string LastNameInfo { get; private set; } = "The last name of the player.";
    public string ListGiven { get; private set; } = "Has List given";

    public string ListGivenInfo { get; private set; } =
        "Check, if the player has given his army / squad list to the TO (if any).";

    public string ListGivenShort { get; private set; } = "L";
    public string Lock { get; private set; } = "Locks the Pairing, so it can't be accidentally changed.";
    public string LockShort { get; private set; } = "L";
    public string Losses { get; private set; } = "Losses";
    public string LossesShort { get; private set; } = "L";
    public string MarginOfVictory { get; private set; } = "Margin of Victory";
    public string MarginOfVictoryShort { get; private set; } = "MoV";
    public string MaxPoints { get; private set; } = "Max Listpoints:";

    public string MaxPointsInfo { get; private set; } =
        "The maximum of points for the lists (if the game system uses any).";

    public string MenuAbout { get; private set; } = "About TXM";
    public string MenuAutoSaveFiles { get; private set; } = "AutoSave Files";
    public string MenuBBCODE { get; private set; } = "Get BBCode for Forum";
    public string MenuBonusPoints { get; private set; } = "Award bonus points";
    public string MenuCalculateWonByes { get; private set; } = "Calculate SoS for won byes";
    public string MenuChangePairings { get; private set; } = "Change Pairings";
    public string MenuCSVAdd { get; private set; } = "Add Information from Excel (CSV)";
    public string MenuCSVImport { get; private set; } = "Excel (CSV) Import";
    public string MenuDeleteAutoSaveFolder { get; private set; } = "Delete AutoSave Folder";
    public string MenuDeleteDropPlayer { get; private set; } = "Delete / Drop Player";
    public string MenuDisqualifyPlayer { get; private set; } = "Disqualify Player";
    public string MenuEditPlayer { get; private set; } = "Edit Player";
    public string MenuEditTournament { get; private set; } = "Edit Tournament";
    public string MenuExit { get; private set; } = "Close TXM";
    public string MenuFile { get; private set; } = "File";
    public string MenuImportExport { get; private set; } = "Import/Export";
    public string MenuInfo { get; private set; } = "Info";
    public string MenuListFortressExport { get; private set; } = "Export for ListFortress";
    public string MenuLoad { get; private set; } = "Load";
    public string MenuLoadAutoSaveFiles { get; private set; } = "Load AutoSave Files";
    public string MenuManual { get; private set; } = "User Manual (online)";
    public string MenuNewPlayer { get; private set; } = "New Player";
    public string MenuNewTournament { get; private set; } = "New Tournament";
    public string MenuOpenAutoSaveFolder { get; private set; } = "Open AutoSave Folder";
    public string MenuPlayerManagement { get; private set; } = "Player Management";
    public string MenuPrint { get; private set; } = "Print";
    public string MenuPrintPairingsWith { get; private set; } = "Print Pairings (with Results)";
    public string MenuPrintPairingsWithout { get; private set; } = "Print Pairings (without Results)";
    public string MenuPrintScoreSheets { get; private set; } = "Print Score Sheet";
    public string MenuPrintTable { get; private set; } = "PrintTable";
    public string MenuResetLastResults { get; private set; } = "Reset last results";
    public string MenuSave { get; private set; } = "Save";
    public string MenuSettings { get; private set; } = "Settings";
    public string MenuShowPairings { get; private set; } = "Show Pairings";
    public string MenuShowTable { get; private set; } = "Show Table";
    public string MenuSupport { get; private set; } = "Support TXM";
    public string MenuT3Export { get; private set; } = "T3 Export";
    public string MenuT3Import { get; private set; } = "T3 Import";
    public string MenuThanks { get; private set; } = "Special Thanks to...";
    public string MenuTimer { get; private set; } = "Timer";
    public string MenuTools { get; private set; } = "Tools";
    public string MenuTournament { get; private set; } = "Tournament";
    public string MenuView { get; private set; } = "View";
    public string Minutes { get; private set; } = "Minutes";
    public string MissingResult { get; private set; } = "There is at least one result missing.";
    public string ModifiedLosses { get; private set; } = "Modified Losses";
    public string ModifiedLossesShort { get; private set; } = "ML";
    public string ModifiedWins { get; private set; } = "Modified Wins";
    public string ModifiedWinsShort { get; private set; } = "MW";
    public string NewPlayer { get; private set; } = "New Player";
    public string NewTournament { get; private set; } = "New Tournament";
    public string NextRound { get; private set; } = "Next Round";
    public string NextRoundInfo { get; private set; } = "Pair the next round";
    public string Nickname { get; private set; } = "Nickname";
    public string NicknameInfo { get; private set; } = "The nickname / gamer name of the player.";
    public string NoAutoSaveFolder { get; private set; } = "There is no auto save folder.";
    public string None { get; private set; } = "None";
    public string NoPairingsYet { get; private set; } = "No Pairings yet";
    public string NoRoundStarted { get; private set; } = "No Round started";
    public string OverwrittenWarning { get; private set; } = "The current tournament will be overwritten.";
    public string Paid { get; private set; } = "Paid";
    public string PaidInfo { get; private set; } = "Check, if the player paid the tournament fee (if any).";
    public string PaidShort { get; private set; } = "$";
    public string Pairings { get; private set; } = "Pairings";
    public string Player { get; private set; } = "Player";
    public string PlayerShort { get; private set; } = "P";
    public string Points { get; private set; } = "Points";
    public string Present { get; private set; } = "Is Present";
    public string PresentInfo { get; private set; } = "Check, if the player is present.";
    public string PresentShort { get; private set; } = "!";
    public string Protection { get; private set; } = "Which team protection do you want?";

    public string ProtectionInfo { get; private set; } =
        "Team protection prevents team members to get paired against each other. Only recommended for casual events.";

    public string Ok { get; private set; } = "OK";
    public string RandomMinutes { get; private set; } = "How many Minutes should at max randomized?";

    public string RandomMinutesInfo { get; private set; } =
        "The timer will random add or subtract a number of minutes to the maximum which is entered here.";

    public string RandomTime { get; private set; } = "Shall the time be randomized?";

    public string RandomTimeInfo { get; private set; } =
        "The timer can add/subtract a random amount of minutes from the time.";

    public string Rank { get; private set; } = "Rank";
    public string RankShort { get; private set; } = "#";
    public string Remove { get; private set; } = "remove";
    public string RemoveWarning { get; private set; } = "Do you really want to ";
    public string ResultEdited { get; private set; } = "Is the result edited?";
    public string ResultEditedShort { get; private set; } = "OK?";
    public string Results { get; private set; } = "Results";
    public string Round { get; private set; } = "Round";
    public string SaveStateChange { get; private set; } = "Change_Pairings";
    public string SaveStatePairing { get; private set; } = "Pairings_Round_";
    public string SaveStateSeeding { get; private set; } = "Seeding_Round_";
    public string SaveStateStart { get; private set; } = "Tournament_Start";
    public string Score { get; private set; } = "Score";
    public string Select { get; private set; } = "Select";
    public string Settings { get; private set; } = "Settings";
    public string Single { get; private set; } = "Single";
    public string SquadList { get; private set; } = "Army / Squad list (if any)";

    public string SquadListInfo { get; private set; } =
        "A link to the the army / squad list of the player (if the game system uses any).";

    public string Standings { get; private set; } = "Standings";
    public string StartTime { get; private set; } = "Should the timer automatically start at:";
    public string StartTimeInfo { get; private set; } = "The timer will automatically start at the given time.";

    public string StartTimeFormat { get; private set; } =
        "The Time is only allowed in the format HH:MM. AM/PM not supported.";

    public string Start { get; private set; } = "Start";
    public string StartInfo { get; private set; } = "Start Tournament";
    public string State { get; private set; } = "State";
    public string StrengthOfSchedule { get; private set; } = "Strength of Schedule";
    public string StrengthOfScheduleShort { get; private set; } = "SoS";
    public string SupportTitle { get; private set; } = "Support";
    public string Swiss { get; private set; } = "Swiss";
    public string Table { get; private set; } = "Table";
    public string TableShort { get; private set; } = "T";
    public string TableNo { get; private set; } = "Table Number";
    public string TableNoShort { get; private set; } = "T#";
    public string Team { get; private set; } = "Team";
    public string TeamInfo { get; private set; } = "The team of the player.";
    public string TextSize { get; private set; } = "Size of the text:";
    public string TextSizeInfo { get; private set; } = "This is the size for the text in the timer window.";
    public string ThanksTitle { get; private set; } = "Thanks";
    public string Time { get; private set; } = "Time";
    public string TimeFormat { get; private set; } = "HH:MM (24h)";
    public string TimerMinutes { get; private set; } = "How Long should the timer run?";

    public string TimerMinutesInfo { get; private set; } =
        "When you start the Timer it will run the given time in minutes. Changing the tournament game will set the timer to the games default time.";

    public string TimerSettingsPermanent { get; private set; } = "Permanent Timer Settings:";
    public string TimerSettingsTemporary { get; private set; } = "Temporary Timer Settings:";
    public string TimerTitle { get; private set; } = "Timer";
    public string Tournament { get; private set; } = "Tournament";
    public string TournamentName { get; private set; } = "Tournament name:";
    public string TournamentNameInfo { get; private set; } = "The name of the tournament.";
    public string TournamentPoints { get; private set; } = "Tournament Points";
    public string TournamentPointsShort { get; private set; } = "TP";
    public string TournamentType { get; private set; } = "Tournament type:";
    public string TournamentTypeInfo { get; private set; } = "The type of the tournament. Default is single.";
    public string Update { get; private set; } = "Update";
    public string UpdateInfo { get; private set; } = "The currently displayed round will be updated with the changes.";
    public string Winner { get; private set; } = "Winner";
    public string Wins { get; private set; } = "Wins";
    public string WinsShort { get; private set; } = "W";
    public string WonBye { get; private set; } = "Won bye";
    public string WonByeInfo { get; private set; } = "Check, if the player has a won bye (from a previous event).";

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

    public string ThanksText1 { get; private set; } = "Special Thanks to following Friends and Tester";
    public string ThanksText2 { get; private set; } = "Tester, User and the Reason for at least half of the features";
    public string ThanksText3 { get; private set; } = "Teammate and tester";
    public string ThanksText4 { get; private set; } = "Tester";
    public string ThanksText5 { get; private set; } = "User who finds every weird error";
    public string ThanksText6 { get; private set; } = "Creater of the TXM-Logo";
    public string ThanksText7 { get; private set; } = "Tester";
    public string ThanksText8 { get; private set; } = "Poweruser";
    public string ThanksText9 { get; private set; } = "Tester with the ability to find stranger errors";
    public string ThanksText10 { get; private set; } = "Tester";

    public string Thanks =>
        $"{ThanksText1}:\n\nBarlmoro - {ThanksText2}\ntgBrain - {ThanksText3}\nKyle_Nemesis - {ThanksText4}\nPhoton - {ThanksText5}\nN4-DO - {ThanksText6}\nMercya - {ThanksText7}\nBackfire84 - {ThanksText8}\nGreenViper - {ThanksText9}\nCarnis - {ThanksText10}";

    public string SupportText1 { get; private set; } =
        "If you like TXM and want to support it, you can do it ins several ways:";
    public string SupportText2 { get; private set; } = "Spread the word, the more user TXM has, the better it gets!";
    public string SupportText3 { get; private set; } = "Test everything!";
    public string SupportText4 { get; private set; } = "Help to add more Games. You can find more information in";
    public string SupportText5 { get; private set; } = "the online manual";
    public string SupportText6 { get; private set; } = "Translate TXM to have TXM in your language. Information also in";
    public string SupportText7 { get; private set; } = "the online manual";
    public string SupportText8 { get; private set; } = "Leave a tip, if you want:";
    public string SupportText9 { get; private set; } = "Donate via";


    public string SupportText => $"{SupportText1}\n\n- {SupportText2}\n- {SupportText3}\n- {SupportText4} ";
    public string DocuText => $"{SupportText5}";
    public string NextText => $"\n- {SupportText6}: ";
    public string LangText => $"{SupportText7}";
    public string LastText => $"\n- {SupportText8}: ";
    public string DonateText => $"{SupportText9} Paypal";

    public string PlayerAction(string player, string action)
    {
        if (IsActionBeforeName)
        {
            return action + " " + player;
        }

        return player + " " + action;
    }


    //TODO JSON Import (export)
}