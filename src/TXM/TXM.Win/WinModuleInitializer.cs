using System.Runtime.CompilerServices;

using TXM.Core.Global;
using TXM.Core.ViewModels;
using TXM.Win.Dialogs;
using TXM.Win.Windows;

namespace TXM.Win;

public static class WinModuleInitializer
{
    [ModuleInitializer]
    public static void WinInitializer()
    {
        //Initialize the global State
        State.Io.FileManager = new WindowsFile();
        State.Io.MessageManager = new WindowsMessage();
        State.DialogService = WindowsDialogService.GetInstance();
        State.DialogService.RegisterDialog<TournamentViewModel, TournamentDialog>();
        State.DialogService.RegisterDialog<AutoSaveViewModel, AutoSaveDialog>();
        State.DialogService.RegisterDialog<PlayerViewModel, PlayerDialog>();
        State.DialogService.RegisterDialog<PairingsViewModel, PairingsDialog>();
        State.DialogService.RegisterDialog<OutputViewModel, OutputDialog>();
        State.DialogService.RegisterDialog<SettingsViewModel, SettingsDialog>();
        State.DialogService.RegisterDialog<ThanksViewModel, ThanksDialog>();
        State.DialogService.RegisterDialog<AboutViewModel, AboutDialog>();
        State.DialogService.RegisterDialog<SupportViewModel, SupportDialog>();
        State.WindowService = WindowsWindowService.GetInstance();
        State.WindowService.RegisterWindow<ProjectorViewModel, ProjectorWindow>();
        State.WindowService.RegisterWindow<TimerViewModel, TimerWindow>();
        State.Clipboard = new WindowsClipboard();
    }
}