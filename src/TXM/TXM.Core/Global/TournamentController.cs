using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using TXM.Core.Interfaces;
using TXM.Core.Logic;
using TXM.Core.Models;
using TXM.Core.ViewModels;

namespace TXM.Core.Global;

public class TournamentController
{
    private static TournamentController _instance = new TournamentController();
    public static TournamentController GetInstance() => _instance;

    public Tournament? ActiveTournament { get; set; }

    private Dictionary<string, IWindow> _openWindows = new();

    private TournamentController() { }

    public void LoadTournament(bool autosave = false)
    {
        var overwrite = true;
        var filename = "";
        if (autosave)
        {
            var filenames = State.Io.GetAutosaveFiles();
            var files = new List<AutoSaveFile>();
            for (var i = filenames.Length - 1; i >= 0; i--)
            {
                files.Add(new AutoSaveFile(filenames[i]));
            }

            var asvm = new AutoSaveViewModel();
            asvm.Files = files;
            var dialogResult = State.DialogService.ShowDialog<AutoSaveViewModel>(
                result => { asvm = (AutoSaveViewModel)result; }
                , asvm);
            if (dialogResult == true)
            {
                overwrite = true;
                filename = asvm.FileName;
            }
        }
        else
        {
            if (ActiveTournament != null)
            {
                if (!State.Io.ShowMessageWithOKCancel(State.Text.OverwrittenWarning))
                {
                    overwrite = false;
                }
            }
        }

        if (overwrite)
        {
            ActiveTournament = State.Io.Load(filename);
        }

        if (ActiveTournament != null)
        {
            //TODO Set UI
            // if (tournamentController.ActiveTournament.Rounds != null)
            // {
            //     ButtonGetResults.Content = tournamentController.ActiveTournament.ButtonGetResultsText;
            //     ButtonGetResults.IsEnabled = true;
            //     ButtonCut.IsEnabled = tournamentController.ActiveTournament.ButtonCutState == true;
            //     tournamentController.ActiveTournament.Sort();
            //
            //     ButtonGetResults.ToolTip = ButtonGetResults.Content.ToString();
            //}

            SetRandomTime();
        }
    }

    public string SetRandomTime(string time = "")
    {
        try
        {
            State.Timer.RandomMins = Int32.Parse(time);
        }
        catch (Exception)
        {
            try
            {
                State.Timer.RandomMins = ActiveTournament.Rule.DefaultRandomMins;
            }
            catch (Exception)
            {
                State.Timer.RandomMins = 0;
            }
        }

        return State.Timer.RandomMins.ToString();
    }

    public void Import(bool csv)
    {
        if (ActiveTournament != null)
        {
            if (!State.Io.ShowMessageWithOKCancel(State.Text.OverwrittenWarning))
            {
                return;
            }
        }
        string title;
        if (csv)
        {
            ActiveTournament = State.Io.CSVImport();
            title = State.Text.MenuCSVImport;
        }
        else
        {
            ActiveTournament = State.Io.GOEPPImport();
            title = State.Text.MenuT3Import;
        }

        if (ActiveTournament != null)
        {
            OpenTournamentDialog(title, ActiveTournament, AbstractRules.GetAllRuleNames(true));
            State.Timer.DefaultTime = ActiveTournament.Rule.DefaultTime;
            SetRandomTime();
        }
    }

    public string Print(bool print, bool pairings = false, bool results = false)
    {
        string file = "";
        if (!pairings && ActiveTournament != null)
        {
            file = State.Io.PrintPlayerList();
        }
        else if (pairings)
        {
            file = State.Io.PrintPairings(results);
        }

        if (print)
        {
            var uri = "file://" + file;
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }

        return file;
    }

    public void PrintScoreSheet()
    {
        string file = State.Io.PrintScoreSheet();
        var uri = "file://" + file;
        var psi = new ProcessStartInfo();
        psi.UseShellExecute = true;
        psi.FileName = uri;
        Process.Start(psi);
    }

    public void NewTournament()
    {
        if (ActiveTournament != null)
        {
            if (!State.Io.ShowMessageWithOKCancel(State.Text.OverwrittenWarning))
            {
                return;
            }
        }

        OpenTournamentDialog(State.Text.NewTournament, new Tournament(State.Text.NewTournament)
            , AbstractRules.GetAllRuleNames());
    }

    public void StartTournament()
    {
        if (ActiveTournament.Participants.Count != 0)
        {
            State.Io.Save(true, State.Text.SaveStateStart);
            ActiveTournament.StartTournament();
        }
        else
        {
            State.Io.ShowMessage(State.Text.EmptyTournamentWarning);
        }
    }


    public void SaveTournament(bool autosave = false, string text = "")
    {
        State.Io.Save(autosave, text + ActiveTournament.Rounds.Count);
    }

    public void EditPairings(string buttonGetResultsText, bool CutIsEnabled)
    {
        //TODO: Correct Pairings needs to be set
        var pvm = new PairingsViewModel
        {
            Pairings = ActiveTournament.Rounds[^1].Pairings
        };
        var dialogResult = State.DialogService.ShowDialog<PairingsViewModel>(
            result => { pvm = (PairingsViewModel)result; }
            , pvm);
        if (dialogResult == true)
        {
            ActiveTournament.Rounds[^1].Pairings.Clear();
            foreach (var p in pvm.Pairings.OrderBy(x => x.TableNo))
            {
                ActiveTournament.Rounds[^1].Pairings.Add(p);
            }

            State.Io.Save(true, State.Text.SaveStateChange);
        }
    }

    public void EditTournament()
    {
        OpenTournamentDialog(State.Text.MenuEditTournament, ActiveTournament, new List<string>()
        {
            State.Text.GameSystemNotChangeable
        }, ActiveTournament.Rounds.Count == 1);
    }

    public void NewPlayer()
    {
        var result = OpenPlayerDialog(State.Text.NewPlayer, new Player(State.Text.NewPlayer));
        if (result.result == true)
        {
            ActiveTournament.AddPlayer(result.player);
        }
    }

    public void EditPlayer(Player player)
    {
        var result = OpenPlayerDialog(State.Text.MenuEditPlayer, new Player(player));
        if (result.result == true)
        {
            player.Team = result.player.Team;
            player.Name = result.player.Name;
            player.Firstname = result.player.Firstname;
            player.HasWonBye = result.player.HasWonBye;
            player.HasListGiven = result.player.HasListGiven;
            player.HasPaid = result.player.HasPaid;
            player.TableNo = result.player.TableNo;
            player.IsPresent = result.player.IsPresent;
        }
    }

    public bool RemovePlayer(Player player, bool disqualify = false)
    {
        var text = State.Text.Remove;
        if (ActiveTournament.Rounds.Count > 1)
        {
            text = State.Text.Drop;
        }
        else if (ActiveTournament.Rounds.Count > 1 & disqualify)
        {
            text = State.Text.Disqualify;
        }

        if (State.Io.ShowMessageWithOKCancel(State.Text.RemoveWarning + " " +
                                             State.Text.PlayerAction(player.DisplayName, text) + " ?"))
        {
            if (text == State.Text.Remove)
            {
                ActiveTournament.RemovePlayer(player);
            }
            else if (text == State.Text.Drop)
            {
                ActiveTournament.DropPlayer(player);
            }
            else if (text == State.Text.Disqualify)
            {
                ActiveTournament.DisqualifyPlayer(player);
            }

            return true;
        }

        return false;
    }

    public void ShowProjector(bool table)
    {
        if (!_openWindows.ContainsKey(Literals.Projector))
        {
            OpenProjectorWindow();
        }

        var window = _openWindows.First(x => x.Key == Literals.Projector).Value;
        var pvm = window.DataContext as ProjectorViewModel;

        var file = "";
        var title = "";
        if (table)
        {
            file = Print(false);
            title = $"{ActiveTournament.Name} - {State.Text.Standings}";
        }
        else
        {
            file = Print(false, true);
            title = $"{ActiveTournament.Name} - {State.Text.Pairings}";
        }

        pvm.Path = file;
        pvm.Title = title;
        pvm.Timer = State.Timer;
    }

    public void Close()
    {
        foreach (var windows in _openWindows.Values)
        {
            windows.Close();
        }
    }

    public void GetBBCode()
    {
        var ovm = new OutputViewModel
        {
            Title = State.Text.BBCodeGenerator, IsTableOutput = true
        };
        var dialogResult = State.DialogService.ShowDialog<OutputViewModel>(
            result => { ovm = (OutputViewModel)result; }
            , ovm);
        if (dialogResult == true)
        {
            StringBuilder sb = new();
            if (ovm.IsResultOutput)
            {
                sb.Append(State.Io.CreateOutputForPairings(ActiveTournament, true, true));
            }

            if (ovm.IsTableOutput)
            {
                sb.Append(State.Io.CreateOutputForPairings(ActiveTournament, true, false));
            }

            if (ovm.IsPairingOutput)
            {
                sb.Append(State.Io.CreateOutputForTable(ActiveTournament, true));
            }

            State.Clipboard.SetText(sb.ToString());
        }
    }

    public void ShowTimer()
    {
        if (!_openWindows.ContainsKey(Literals.Timer))
        {
            OpenTimerWindow();
        }
    }

    public void ShowSettings()
    {
        var svm = new SettingsViewModel()
        {
            Title = State.Text.Settings, TextColors = State.Text.Colors.Values.OrderBy(x => x).ToList()
            , TextColor = State.Setting.TextColor, TextSize = State.Setting.TextSize, LanguageData = new()
            {
                new LocalFile()
                {
                    FileName = Literals.LanguageDefault
                }
            }
            , Language = State.Setting.Language, IsTimerRandom = State.Timer.IsTimerRandom
            , RandomMinutes = State.Timer.RandomMins, Time = State.Timer.DefaultTime
            , StartingHour = State.Timer.StartHour, StartingMinute = State.Timer.StartMinute
            , StartTime = $"{State.Timer.StartHour:D2}:{State.Timer.StartMinute:D2}"
            , BgImagePath = State.Setting.BgImagePath
        };
        var lang = State.Io.GetLanguages();
        lang.Files.ForEach(x => svm.LanguageData.Add(x));
        svm.PathSeparator = lang.Separator;
        var dialogResult = State.DialogService.ShowDialog<SettingsViewModel>(
            result => { svm = (SettingsViewModel)result; }
            , svm);
        if (dialogResult == true)
        {
            State.Setting.TextColor = svm.TextColor;
            State.Setting.TextSize = svm.TextSize;
            State.Setting.Language = svm.Language;
            State.Setting.BgImagePath = svm.BgImagePath;
            State.Timer.IsTimerRandom = svm.IsTimerRandom;
            State.Timer.RandomMins = svm.RandomMinutes;
            State.Timer.DefaultTime = svm.Time;
            State.Timer.StartHour = svm.StartingHour;
            State.Timer.StartMinute = svm.StartingHour;
            State.Io.SaveSettings();
        }
    }

    private void OpenTimerWindow()
    {
        var tvm = new TimerViewModel()
        {
            Title = State.Text.TimerTitle
        };
        var window = State.WindowService.Show<TimerViewModel>(
            window =>
            {
                var key = _openWindows.FirstOrDefault(x => x.Value == window).Key;
                _openWindows.Remove(key);
            },
            tvm);
        _openWindows.Add(Literals.Timer, window);
    }

    public void ShowUserManual()
    {
        var uri = "https://github.com/Sharpdeveloper/TXM/wiki/User-Manual";
        var psi = new ProcessStartInfo();
        psi.UseShellExecute = true;
        psi.FileName = uri;
        Process.Start(psi);
    }

    private void OpenProjectorWindow()
    {
        var pvm = new ProjectorViewModel();
        var window = State.WindowService.Show<ProjectorViewModel>(
            window =>
            {
                var key = _openWindows.FirstOrDefault(x => x.Value == window).Key;
                _openWindows.Remove(key);
            },
            pvm);
        _openWindows.Add(Literals.Projector, window);
    }

    private void OpenTournamentDialog(string dialogTitle
        , Tournament tournament
        , List<string> tournamentSystems
        , bool isGameSystemChangeable = true)
    {
        var tvm = new TournamentViewModel
        {
            ActiveTournament = tournament, Title = dialogTitle, TournamentSystems = tournamentSystems
            , IsGameSystemChangeable = isGameSystemChangeable
        };
        var dialogResult = State.DialogService.ShowDialog<TournamentViewModel>(
            result => { tvm = (TournamentViewModel)result; }
            , tvm);
        if (dialogResult == true)
        {
            ActiveTournament = tvm.ActiveTournament;
            State.Timer.DefaultTime = ActiveTournament.Rule?.DefaultTime ?? 60;
        }
    }

    private (bool? result, Player player) OpenPlayerDialog(string dialogTitle, Player player)
    {
        var pvm = new PlayerViewModel()
        {
            ActivePlayer = player, Title = dialogTitle
        };
        var dialogResult = State.DialogService.ShowDialog<PlayerViewModel>(
            result => { pvm = (PlayerViewModel)result; }
            , pvm);
        return (dialogResult, pvm.ActivePlayer);
    }

    public void ShowThanks()
    {
        var tvm = new ThanksViewModel()
        {
            Title = State.Text.ThanksTitle, Text = State.Text.Thanks
        };
        State.DialogService.ShowDialog<ThanksViewModel>(
            result => { tvm = (ThanksViewModel)result; }
            , tvm);
    }

    public void ShowAbout()
    {
        var avm = new AboutViewModel()
        {
            Title = State.Text.ThanksTitle, Text = State.Text.AboutText
        };
        State.DialogService.ShowDialog<AboutViewModel>(
            result => { avm = (AboutViewModel)result; }
            , avm);
    }

    public void GetSeed(bool cut)
    {
        if (cut)
        {
            ActiveTournament.OperateCut();
        }
        else
        {
            ActiveTournament.NextRound();
        }
    }

    public void GetResults()
    {
        var allResultsEdited = true;
        var pairings = State.Controller.ActiveTournament!.Rounds[ActiveTournament.SelectedRound].Pairings;
        if (ActiveTournament!.Rule!.IsDrawPossible)
        {
            foreach (var p in pairings)
            {
                if (!p.IsResultEdited)
                {
                    allResultsEdited = false;
                    State.Io.ShowMessage(State.Text.MissingResult);
                    return;
                }
            }
        }

        if (CheckResults(pairings))
        {
            ActiveTournament.GetResults();
        }
        else
        {
            State.Io.ShowMessage(State.Text.InvalidResult);
            return;
        }

        ActiveTournament.Sort();
    }

    private bool CheckResults(IEnumerable<Pairing> pairings)
    {
        return pairings.Where(p => !ActiveTournament!.Rule!.IsDrawPossible).All(p =>
            p.Player1Score != p.Player2Score || p.Winner != State.Text.Automatic ||
            p.Player2ID == ActiveTournament.Bye.ID || p.Player2ID == ActiveTournament.WonBye.ID);
    }

    public void ShowSupport()
    {
        var svm = new SupportViewModel()
        {
            Title = State.Text.SupportTitle, SupportText = State.Text.SupportText, DocuText = State.Text.DocuText
            , NextText = State.Text.NextText, LangText = State.Text.LangText, LastText = State.Text.LastText
            , DonateText = State.Text.DonateText
        };
        State.DialogService.ShowDialog<SupportViewModel>(
            result => { svm = (SupportViewModel)result; }
            , svm);
    }

    public void OpenNewGameDocu()
    {
        var uri = "http://github.com/Sharpdeveloper/TXM/wiki/Request-support-for-another-game";
        var psi = new ProcessStartInfo();
        psi.UseShellExecute = true;
        psi.FileName = uri;
        Process.Start(psi);
    }

    public void OpenNewLangDocu()
    {
        var uri = "https://github.com/Sharpdeveloper/TXM/wiki/Help-translate-TXM";
        var psi = new ProcessStartInfo();
        psi.UseShellExecute = true;
        psi.FileName = uri;
        Process.Start(psi);
    }

    public void OpenDonate()
    {
        var uri = "http://paypal.me/TKundNobody?country.x=DE&locale.x=de_DE";
        var psi = new ProcessStartInfo();
        psi.UseShellExecute = true;
        psi.FileName = uri;
        Process.Start(psi);
    }
}