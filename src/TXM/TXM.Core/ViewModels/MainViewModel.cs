using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TXM.Core.Global;
using TXM.Core.Logic;
using TXM.Core.Models;

namespace TXM.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private bool IsSaveExecutable => ActiveTournament != null;
    private bool IsCSVAddExecutable => ActiveTournament != null;
    private bool IsExportExecutable => ActiveTournament != null;
    private bool IsPrintExecutable => ActiveTournament != null;
    private bool IsEditTournamentExecutable => ActiveTournament != null;
    private bool IsCalculateWonByesExecutable => ActiveTournament != null;
    private bool IsAwardBonusPointsExecutable => ActiveTournament != null;
    private bool IsNewPlayerExecutable => ActiveTournament != null;
    private bool IsEditPlayerExecutable => SelectedPlayer != null;
    private bool IsRemovePlayerExecutable => SelectedPlayer != null;
    private bool IsDisqualifyPlayerExecutable => SelectedPlayer != null;
    private bool IsStartExecuteable => ActiveTournament!= null && !ActiveTournament.IsStarted;

    private bool IsPairingsChangeExecutable =>
        ActiveTournament?.Rounds.Count > 0 && ActiveTournament?.Rounds[^1].Pairings != null;

    private bool IsResultResetable =>
        ActiveTournament?.Rounds.Count > 0 && ActiveTournament?.Rounds[^1].Pairings != null;

    private bool IsShowPairingsExecutable =>
        ActiveTournament?.Rounds.Count > 0 && ActiveTournament?.Rounds[^1].Pairings != null;

    private bool IsShowTableExecutable => ActiveTournament?.Participants.Count > 0;
    private bool IsGetBBCodeExecutable => ActiveTournament?.Participants.Count > 0;
    private bool IsNextRoundExecutable => ActiveTournament?.IsStarted == true && ActiveTournament?.IsSeeded == false;
    private bool IsGetResultsExecutable => ActiveTournament?.IsStarted == true && ActiveTournament?.IsSeeded == true;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    [NotifyPropertyChangedFor(nameof(ModifiedWinsVisibility))]
    [NotifyPropertyChangedFor(nameof(DrawsVisibility))]
    [NotifyPropertyChangedFor(nameof(ModifiedLossesVisibility))]
    [NotifyPropertyChangedFor(nameof(MarginOfVictoryVisibility))]
    [NotifyPropertyChangedFor(nameof(ExtendedStrengthOfScheduleVisibility))]
    [NotifyPropertyChangedFor(nameof(PointsVisibility))]
    [NotifyPropertyChangedFor(nameof(WinnerVisibility))]
    [NotifyPropertyChangedFor(nameof(GetResultsText))]
    public Tournament? activeTournament;

    [ObservableProperty]
    public Texts text = State.Text;
    
    public string GetResultsText => ActiveTournament?.Rounds.Count > 0 ? (ActiveTournament?.DisplayedRound == ActiveTournament?.Rounds[^1].RoundText ? Text.GetResults : Text.Update) : "";
    public string GetResultsInfo =>  ActiveTournament?.Rounds.Count > 0 ? (ActiveTournament?.DisplayedRound == ActiveTournament?.Rounds[^1].RoundText ? Text.GetResultsInfo : Text.UpdateInfo) : "";

    [ObservableProperty]
    private Player? _selectedPlayer;

    [ObservableProperty]
    private TournamentTimer _timer = State.Timer;

    public bool ModifiedWinsVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ModWins);

    public bool DrawsVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.Draws);

    public bool ModifiedLossesVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ModLoss);

    public bool MarginOfVictoryVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.MoV);

    public bool ExtendedStrengthOfScheduleVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ESoS);

    public bool PointsVisibility =>
        ActiveTournament?.Rule != null
        && ActiveTournament.Rule.IsTournamentPointsInputNeeded;

    public bool WinnerVisibility =>
        ActiveTournament?.Rule != null &&
        ActiveTournament.Rule.IsWinnerDropDownNeeded;

    public string Title => ActiveTournament == null
        ? "TXM - The Tournament App"
        : $"{ActiveTournament.Name} - TXM - The Tournament App";

    public MainViewModel()
    {
        ActiveTournament = new Tournament("Schlacht", 12345, "2.0", new XWing2Rules());
        ActiveTournament.AddPlayer(new Player("TKundNobody"));
        ActiveTournament.AddPlayer(new Player("Tesdeor"));
        ActiveTournament.Rounds.Add(new Round("Start", new()));
        ActiveTournament.Rounds[0].Pairings.Add(new Pairing()
            { Player1 = ActiveTournament.Participants[0], Player2 = ActiveTournament.Participants[1] });
        ActiveTournament.Rounds.Add(new Round(1, new()));
        ActiveTournament.Rounds[1].Pairings.Add(new Pairing()
            { Player1 = ActiveTournament.Participants[1], Player2 = ActiveTournament.Participants[0] });
        State.Controller.ActiveTournament = ActiveTournament;
    }

    [RelayCommand]
    private void NewTournament()
    {
        State.Controller.NewTournament();
        ActiveTournament = State.Controller.ActiveTournament;
        CheckNotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void Load()
    {
        State.Controller.LoadTournament();
        ActiveTournament = State.Controller.ActiveTournament;
        CheckNotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(IsSaveExecutable))]
    private void Save()
    {
        //TODO Anpassen => ggf. ins Tournament verlagern
        State.Controller.SaveTournament();
    }

    [RelayCommand]
    private void LoadAutoSave()
    {
        if (State.Io.AutosavePathExists)
        {
            State.Controller.LoadTournament(true);
            ActiveTournament = State.Controller.ActiveTournament;
            CheckNotifyCanExecuteChanged();
        }
        else
        {
            State.Io.ShowMessage(Text.NoAutoSaveFolder);
        }
    }

    [RelayCommand]
    private void OpenAutoSaveFolder()
    {
        if (State.Io.AutosavePathExists)
        {
            State.Io.OpenAutosaveFolder();
        }
        else
        {
            State.Io.ShowMessage(Text.NoAutoSaveFolder);
        }
    }

    [RelayCommand]
    private void DeleteAutoSaveFolder() => State.Io.DeleteAutosaveFolder();

        [RelayCommand]
    private void T3Import()
    {
        State.Controller.Import(false);
        ActiveTournament = State.Controller.ActiveTournament;
        CheckNotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void CSVImport()
    {
        State.Controller.Import(true);
        ActiveTournament = State.Controller.ActiveTournament;
        CheckNotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(IsCSVAddExecutable))]
    private void CSVAdd() => State.Io.CSVImportAdd();

        [RelayCommand(CanExecute = nameof(IsExportExecutable))]
    private void T3Export() => State.Io.GOEPPExport(ActiveTournament!);

        [RelayCommand(CanExecute = nameof(IsExportExecutable))]
    private void ListFortressExport()
    {
        (string json, string file) result = State.Io.GetJsonForListfortress();
        State.Clipboard.SetText(result.json);
        State.Io.ShowMessage($"{Text.ClipboardInfo} listfortress.com.\n{Text.ExportInfo} {result.file}");
    }

    [RelayCommand(CanExecute = nameof(IsPrintExecutable))]
    private void PrintTable() => State.Controller.Print(true);

    [RelayCommand(CanExecute = nameof(IsPrintExecutable))]
    private void PrintPairingWithout() => State.Controller.Print(true, true);

    [RelayCommand(CanExecute = nameof(IsPrintExecutable))]
    private void PrintPairingWithResult() =>     State.Controller.Print(true, true, true);

    [RelayCommand(CanExecute = nameof(IsPrintExecutable))]
    private void PrintScoreSheet() => State.Controller.PrintScoreSheet();

    [RelayCommand(CanExecute = nameof(IsPairingsChangeExecutable))]
    private void ChangePairing() => State.Controller.EditPairings("" /*ButtonGetResults.Content.ToString()*/, false /*ButtonCut.IsEnabled*/);

    [RelayCommand(CanExecute = nameof(IsResultResetable))]
    private void ResetLastResults() => ActiveTournament.RemoveLastRound();

    [RelayCommand(CanExecute = nameof(IsEditTournamentExecutable))]
    private void EditTournament() => State.Controller.EditTournament();

    [RelayCommand(CanExecute = nameof(IsCalculateWonByesExecutable))]
    private void CalculateWonByes() => ActiveTournament.CalculateWonBye();

    [RelayCommand(CanExecute = nameof(IsAwardBonusPointsExecutable))]
    private void AwardBonusPoints()
    {
        ActiveTournament.GetBonusSeed();
        State.Controller.SaveTournament(true, "Bonus");
    }

    [RelayCommand(CanExecute = nameof(IsNewPlayerExecutable))]
    private void NewPlayer() => State.Controller.NewPlayer();

    [RelayCommand(CanExecute = nameof(IsEditPlayerExecutable))]
    private void EditPlayer() => State.Controller.EditPlayer(SelectedPlayer);

    [RelayCommand(CanExecute = nameof(IsRemovePlayerExecutable))]
    private void RemovePlayer()
    {
        if (State.Controller.RemovePlayer(SelectedPlayer))
        {
            SelectedPlayer = null;
        }
    }

    [RelayCommand(CanExecute = nameof(IsDisqualifyPlayerExecutable))]
    private void DisqualifyPlayer()
    {
        if (State.Controller.RemovePlayer(SelectedPlayer, true))
        {
            SelectedPlayer = null;
        }
    }

    [RelayCommand(CanExecute = nameof(IsShowPairingsExecutable))]
    private void ShowPairings() => State.Controller.ShowProjector(false);

    [RelayCommand(CanExecute = nameof(IsShowTableExecutable))]
    private void ShowTable() => State.Controller.ShowProjector(true);

    [RelayCommand(CanExecute = nameof(IsGetBBCodeExecutable))]
    private void GetBBCode() => State.Controller.GetBBCode();

    [RelayCommand]
    private void ShowTimer() => State.Controller.ShowTimer();

    [RelayCommand]
    private void StartTimer() => Timer.StartTimer();

    [RelayCommand]
    private void PauseTimer() => Timer.PauseTimer();

    [RelayCommand]
    private void ResetTimer() => Timer.ResetTimer();

    [RelayCommand]
    private void ShowSettings() => State.Controller.ShowSettings();

    [RelayCommand]
    private void OpenManual() => State.Controller.ShowUserManual();

    [RelayCommand]
    private void ShowThanks() => State.Controller.ShowThanks();

    [RelayCommand]
    private void ShowAbout() => State.Controller.ShowAbout();

    [RelayCommand]
    private void ShowSupport() => State.Controller.ShowSupport();
    
    [RelayCommand(CanExecute = nameof(IsStartExecuteable))]
    private void Start() => State.Controller.StartTournament();

    [RelayCommand(CanExecute = nameof(IsNextRoundExecutable))]
    private void NextRound() => State.Controller.GetSeed(false);
    
    [RelayCommand(CanExecute = nameof(IsGetResultsExecutable))]
    private void GetResults() => State.Controller.GetSeed(false);

    private void CheckNotifyCanExecuteChanged()
    {
        SaveCommand.NotifyCanExecuteChanged();
        CSVAddCommand.NotifyCanExecuteChanged();
        T3ExportCommand.NotifyCanExecuteChanged();
        ListFortressExportCommand.NotifyCanExecuteChanged();
        PrintTableCommand.NotifyCanExecuteChanged();
        PrintPairingWithoutCommand.NotifyCanExecuteChanged();
        PrintPairingWithResultCommand.NotifyCanExecuteChanged();
        PrintScoreSheetCommand.NotifyCanExecuteChanged();
        ChangePairingCommand.NotifyCanExecuteChanged();
        ResetLastResultsCommand.NotifyCanExecuteChanged();
        EditTournamentCommand.NotifyCanExecuteChanged();
        CalculateWonByesCommand.NotifyCanExecuteChanged();
        AwardBonusPointsCommand.NotifyCanExecuteChanged();
        NewPlayerCommand.NotifyCanExecuteChanged();
        EditPlayerCommand.NotifyCanExecuteChanged();
        RemovePlayerCommand.NotifyCanExecuteChanged();
        DisqualifyPlayerCommand.NotifyCanExecuteChanged();
        ShowPairingsCommand.NotifyCanExecuteChanged();
        ShowTableCommand.NotifyCanExecuteChanged();
        GetBBCodeCommand.NotifyCanExecuteChanged();
        StartCommand.NotifyCanExecuteChanged();
        NextRoundCommand.NotifyCanExecuteChanged();
        GetResultsCommand.NotifyCanExecuteChanged();
    }
}