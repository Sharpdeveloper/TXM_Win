using CommunityToolkit.Mvvm.ComponentModel;

using TXM.Core.Global;
using TXM.Core.Logic;

namespace TXM.Core.ViewModels;

public partial class TournamentViewModel: ObservableObject
{
    [ObservableProperty]
    private Tournament? _activeTournament;

    [ObservableProperty]
    private Texts _text = State.Text;

    [ObservableProperty]
    private string _title;

    public string GameSystem
    {
        get => ActiveTournament?.Rule?.GetName()?? "";
        set
        {
            ActiveTournament!.Rule = AbstractRules.GetRule(value);
            ActiveTournament.MaxPoints = ActiveTournament.Rule.DefaultMaxPoints;
        }
    }

    public string PairingType
    {
        get
        {
            if (ActiveTournament?.PairingType == TournamentPairingType.Swiss)
            {
                return Text.Swiss;
            }
            else
            {
                return "";
            }
        }
        set
        {
            if (value == Text.Swiss)
            {
                ActiveTournament.PairingType = TournamentPairingType.Swiss;
            }
        }
    }
    
    public string Protection
    {
        get
        {
            if (ActiveTournament?.Protection == TeamProtection.None)
            {
                return Text.None;
            }
            else if (ActiveTournament?.Protection == TeamProtection.FirstRound)
            {
                return Text.FirstRound;
            }
            else
            {
                return "";
            }
        }
        set
        {
            if (value == Text.None)
            {
                ActiveTournament.Protection = TeamProtection.None;
            }
            else if (value == Text.FirstRound)
            {
                ActiveTournament.Protection = TeamProtection.FirstRound;
            }
        }
    }
    
    public string Type
    {
        get
        {
            if (ActiveTournament?.Type == TournamentType.Single)
            {
                return Text.Single;
            }
            else
            {
                return "";
            }
        }
        set
        {
            if (value == Text.Single)
            {
                ActiveTournament.Type = TournamentType.Single;
            }
        }
    }

    [ObservableProperty]
    private List<string> _tournamentSystems;

    [ObservableProperty]
    private bool _isGameSystemChangeable = true;
    
    public TournamentViewModel()
    {

    }
}