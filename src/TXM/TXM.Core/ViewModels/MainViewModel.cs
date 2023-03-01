using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Logic;
using TXM.Core.Models;
using TXM.Core.Text;

namespace TXM.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public Tournament? activeTournament;

    public bool? ModifiedWinsVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ModWins);
    
    public bool? DrawsVisibility =>
        ActiveTournament?.Rule?.IsDrawPossible;
    
    public bool? ModifiedLossesVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ModLoss);
    
    public bool? MarginOfVictoryVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.MoV);
    
    public bool? ExtendedStrengthOfScheduleVisibility =>
        ActiveTournament?.Rule?.OptionalFields != null &&
        ActiveTournament.Rule.OptionalFields.Contains(Literals.ESoS);

    public MainViewModel()
    {
         activeTournament = new Tournament("Schlacht", 12345, "2.0", new XWing25Rules());
         activeTournament.AddPlayer(new Player("TKundNobody"));
         activeTournament.AddPlayer(new Player("Tesdeor"));
    }  
}