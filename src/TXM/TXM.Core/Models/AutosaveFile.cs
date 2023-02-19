using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core;

public partial class AutosaveFile : ObservableObject
{
    //Autosave_131430321409284710_Das Erwachen der Nacht - Episode III_Pairings_Round2
    [ObservableProperty]
    public string date;

    [ObservableProperty]
    public string time;

    [ObservableProperty]
    public string tournament;

    [ObservableProperty]
    public string state;

    [ObservableProperty]
    public string round;

    public string Filename { get; set; }

    public AutosaveFile(string filename)
    {
        Filename = filename;
        filename = filename.Substring(filename.LastIndexOf(Path.DirectorySeparatorChar)+1);
        string[] parts = filename.Split('_');
        DateTime dt = new DateTime(long.Parse(parts[1]));
        dt = dt.AddYears(1600);
        Date = dt.ToShortDateString();
        Time = dt.ToShortTimeString();
        Tournament = parts[2];
        try
        {
            State = parts[3].Split('.')[0];
        }
        catch (Exception)
        {
            State = parts[3];
        }
        try
        {
            Round = parts[4].Split('.')[0];
        }
        catch(Exception)
        {
            Round = "";
        }
    }
}
