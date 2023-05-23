using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TXM.Core.Global;
using TXM.Core.Models;

namespace TXM.Core.ViewModels;

public partial class PairingsViewModel : ObservableObject
{
    public ObservableCollection<Pairing> Pairings { get; set; }

    [ObservableProperty]
    private Texts _text = State.Text;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _addText = "+";

    [ObservableProperty]
    private string _subText = "-";

    [ObservableProperty]
    private List<string> _player1List;

    [ObservableProperty]
    private List<string> _player2List;

    public List<(string Name, int Id)> UnpairedPlayers { get; set; }
    public List<int> UnusedTables { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPlayer2Enabled))]
    [NotifyPropertyChangedFor(nameof(SetPlayer2List))]
    public string _player1Name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddExecuteable))]
    public string _player2Name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSubExecuteable))]
    private Pairing? _selectedPairing;

    public bool IsPlayer2Enabled => Player1Name != "";

    private bool IsAddExecuteable => (Player2Name != "" && Player1Name != "");

    private bool IsSubExecuteable => SelectedPairing != null;

    public PairingsViewModel()
    {
        Player1Name = "";
        Player2Name = "";
        Title = "Change Pairings";
    }

    [RelayCommand(CanExecute = nameof(IsAddExecuteable))]
    private void Add()
    {
        Pairing p;
        if (UnusedTables.Count == 0)
        {
            p = new Pairing();
        }
        else
        {
            p = new Pairing(UnusedTables[0]);
            UnusedTables.Remove(UnusedTables[0]);
        }

        for (var i = 0; i < UnpairedPlayers.Count; i++)
        {
            if (p.Player1ID != 0 && p.Player2ID != 0)
            {
                break;
            }

            var player = UnpairedPlayers[i];
            if (player.Name == Player1Name)
            {
                p.Player1Name = player.Name;
                p.Player1ID = player.Id;
                UnpairedPlayers.Remove(player);
                i--;
                continue;
            }

            if (player.Name == Player2Name)
            {
                p.Player2Name = player.Name;
                p.Player2ID = player.Id;
                UnpairedPlayers.Remove(player);
                i--;
                continue;
            }
        }

        if (Player2Name == Player.Bye.DisplayName)
        {
            State.Controller.ActiveTournament.Participants.First(x => x.ID == p.Player1ID).HasBye = true;
            p.Player2 = Player.Bye;
        }

        Pairings.Add(p);

        Player1Name = "";
        Player2Name = "";
        Player1List.Clear();
        Player2List.Clear();
        foreach (var player in UnpairedPlayers)
        {
            Player1List.Add(player.Name);
        }
    }

    [RelayCommand(CanExecute = nameof(IsSubExecuteable))]
    private void Sub()
    {
        UnpairedPlayers.Add((SelectedPairing!.Player1Name, SelectedPairing.Player1ID));
        Player1List.Add(SelectedPairing.Player1Name);
        if (IsPlayer2Enabled)
        {
            Player2List.Add(SelectedPairing.Player1Name);
        }

        if (SelectedPairing.Player2ID != Player.Bye.ID)
        {
            UnpairedPlayers.Add((SelectedPairing.Player2Name, SelectedPairing.Player2ID));
            Player1List.Add(SelectedPairing.Player2Name);
            if (IsPlayer2Enabled)
            {
                Player2List.Add(SelectedPairing.Player2Name);
            }
        }

        Player1List.Sort();
        if (IsPlayer2Enabled)
        {
            Player2List.Sort();
        }

        UnusedTables.Add(SelectedPairing.TableNo);
        Pairings.Remove(SelectedPairing);
    }

    public bool SetPlayer2List
    {
        get
        {
            Player2List.Clear();
            foreach (var s in Player1List.Where(s => s != Player1Name))
            {
                Player2List.Add(s);
            }

            Player2List.Add(Player.Bye.DisplayName);
            return true;
        }
    }
}