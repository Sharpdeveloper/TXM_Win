using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Global;
using TXM.Core.Models;

namespace TXM.Core.ViewModels;

public partial class PlayerViewModel : ObservableObject
{

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private Texts _text = State.Text;

    [ObservableProperty]
    private Player _activePlayer;

    [ObservableProperty]
    private string[] _factions = State.Controller.ActiveTournament.Rule.Factions;
}