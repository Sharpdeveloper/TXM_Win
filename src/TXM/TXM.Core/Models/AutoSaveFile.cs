using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core.Models;

public partial class AutoSaveFile : ObservableObject
{
    // ReSharper disable twice CommentTypo
    //AutoSave_131430321409284710_Das Erwachen der Nacht - Episode III_Pairings_Round2
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

    private string _fileName;
    public string FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            _fileName = _fileName.Substring(_fileName.LastIndexOf(Path.DirectorySeparatorChar)+1);
            string[] parts = _fileName.Split('_');
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

    public AutoSaveFile(string fileName)
    {
        FileName = fileName;
    }
}
