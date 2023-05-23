namespace TXM.Core.Global;

/// <summary>
/// Literals.cs
/// static class for string literals which were used internal in the application
/// </summary>
public sealed class Literals
{
    private Literals() { }
    private static Literals _instance = new Literals();
    public static Literals GetInstance() => _instance;

    public const string ATournament = "A_Tournament";
    public const string AutoSave = "AutoSave";
    public const string Bonus = "Bonus";
    public const string Draws = "Draws";
    public const string ESoS = "eSoS";
    public const string ModLoss = "ModLoss";
    public const string ModWins = "ModWins";
    public const string MoV = "MoV";
    public const string Projector = "ProjectorWindow";
    public const string Timer = "TimerWindow";
    public const string Black = "Black";
    public const string Blue = "Blue";
    public const string Green = "Green";
    public const string Orange = "Orange";
    public const string Purple = "Purple";
    public const string Red = "Red";
    public const string White = "White";
    public const string Yellow = "Yellow";
    public const string LanguageDefault = "English (default)";
    public const string Appname = "TXM - The Tournament App";

    public static readonly string[] ImageEndings = new []
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".tif",
        ".tiff"
    };
}