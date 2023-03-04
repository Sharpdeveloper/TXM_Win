using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Logic;
using TXM.Core.Models;
using TXM.Core.Text;

namespace TXM.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public Tournament? activeTournament;

    //TODO: TabControl Binding to Rounds => Rounds.Pairings Binding for Pairingsgrid
    public ObservableCollection<Pairing> DisplayedPairings { get; set; }

    public bool ModifiedWinsVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ModWins);
    
    public bool DrawsVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.Draws);
    
    public bool ModifiedLossesVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ModLoss);

    public bool MarginOfVictoryVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.MoV);
    
    public bool ExtendedStrengthOfScheduleVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ESoS);

    public bool PointsVisibility =>
        ActiveTournament?.Rule != null
        && ActiveTournament.Rule.IsTournamentPointsInputNeeded;

    public bool WinnerVisibility =>
        ActiveTournament?.Rule != null &&
        ActiveTournament.Rule.IsWinnerDropDownNeeded;
    
    public MainViewModel()
    {
         activeTournament = new Tournament("Schlacht", 12345, "2.0", new XWing25Rules());
         activeTournament.AddPlayer(new Player("TKundNobody"));
         activeTournament.AddPlayer(new Player("Tesdeor"));
         DisplayedPairings = new ObservableCollection<Pairing>();
         DisplayedPairings.Add(new Pairing(){Player1 = activeTournament.Participants[0], Player2 = activeTournament.Participants[1]});
    }  
}