using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core.Global;

public sealed partial class Settings : ObservableObject
{
    public Settings() { }
    private static Settings _instance = new Settings();
    public static Settings GetInstance() => _instance;
    
    #region Constants
    public const string FileExtension = "txmj";
    public const string FileExtensionsName = "TXM Tournaments";
    public const string TxmVersion = "V4.0.0 Beta 1";
    public const string CopyRightYear = "2014 - 2023";
    #endregion

    [ObservableProperty]
    private string _textColor = Literals.Black;
    
    [ObservableProperty]
    private string _bgImagePath = "";
    
    [ObservableProperty]
    private double _textSize = 300.0;
    
    [ObservableProperty]
    private string _language = Literals.LanguageDefault;
}