using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core;

public sealed partial class Settings: ObservableObject
{
    #region Constants
    private const string fileextension = "txmj";
    private const string fileextensionName = "TXM Tournaments";
    private const string txmversion = "V4.0.0 Beta 1";
    private const string copyrightYear = "2014 - 2023";
    #endregion

    #region Properties
    public static string FILEEXTENSION
    {
        get
        {
            return fileextension;
        }
    }

    public static string TXMVERSION
    {
        get
        {
            return txmversion;
        }
    }

    public static string FILEEXTENSIONSNAME
    {
        get
        {
            return fileextensionName;
        }
    }

    public static string COPYRIGHTYEAR
    {
        get
        {
            return copyrightYear;
        }
    }
    #endregion

    [ObservableProperty]
    public string textColor = "Black";
    [ObservableProperty]
    public string bGImagePath = "";
    [ObservableProperty]
    public double textSize = 25.0;

    public Settings()
    {
    }
    
    
}