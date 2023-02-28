using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Logic;
using TXM.Core.Models;

namespace TXM.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public Tournament? activeTournament;

    public MainViewModel()
    {
         activeTournament = new Tournament("Schlacht", 12345, "2.0", new XWing25Rules());
         activeTournament.AddPlayer(new Player("TKundNobody"));
         activeTournament.AddPlayer(new Player("Tesdeor"));
    }  
}