using TXM.Core.Interfaces;

namespace TXM.Core.Global;

public static class State
{
    public static Literals Literal { get; } = Literals.GetInstance();
    public static InputOutput Io { get; } = InputOutput.GetInstance();
    public static IDialogService DialogService { get; set; }
    public static IWindowService WindowService { get; set; }
    public static Settings Setting { get; set; } = Io.LoadSettings();
    public static Texts Text { get; set; } =  Setting.Language == "English (default)" ? Texts.GetInstance() : Io.LoadLanguage(Setting.Language);
    public static IClipboard Clipboard { get; set; }
    public static TournamentTimer Timer { get; } = TournamentTimer.GetInstance();
    public static TournamentController Controller { get; } = TournamentController.GetInstance();
}