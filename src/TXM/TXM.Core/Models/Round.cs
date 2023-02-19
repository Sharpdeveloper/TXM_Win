using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Collections;

namespace TXM.Core;

public class Round
{
    public int RoundNo { get; set; }
    public ObservableCollection<Pairing> Pairings { get; set; }
    public ObservableCollection<Player> Participants { get; set; }
    public string Scenario { get; set; }

    public Round(int roundNo, ObservableCollection<Pairing> pairings, ObservableCollection<Player> participants, string scenario = "")
    {
        RoundNo = roundNo;
        Pairings = pairings;
        Participants = participants;
        Scenario = scenario;
    }
}
