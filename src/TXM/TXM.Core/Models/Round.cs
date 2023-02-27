using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core.Models;

public partial class Round: ObservableObject
{
    [ObservableProperty]
    public string roundText;

    private int roundNo;
    public int RoundNo
    {
        get => roundNo;
        set
        {
            roundText = $"Round {value}";
            roundNo = value;
        }
    }
    public ObservableCollection<Pairing> Pairings { get; set; }

    [ObservableProperty]
    public string scenario;

    public Round(int roundNo, ObservableCollection<Pairing> pairings, string scenario = "")
    {
        RoundNo = roundNo;
        Pairings = pairings;
        Scenario = scenario;
    }

    public Round(int roundNo)
    {
        RoundNo = roundNo;
    }

    public Round(string roundText, ObservableCollection<Pairing> pairings, string scenario)
    {
        RoundText = roundText;
        roundNo = -1;
        Pairings = pairings;
        Scenario = scenario;
    }
}
