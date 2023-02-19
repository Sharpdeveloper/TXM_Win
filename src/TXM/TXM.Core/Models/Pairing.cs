using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TXM.Core;

public partial class Pairing : ObservableObject
{
    #region Static Fields
    internal static int startingTableNo = 0;

    public static void ResetTableNo(int startNo = 0)
    {
        startingTableNo = startNo;
    }
    #endregion

    #region Properties
    [ObservableProperty]
    public int tableNo;

    [ObservableProperty]
    public int player1ID;

    [ObservableProperty]
    public int player2ID;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHidden))]
    public int player1Score;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHidden))]
    public int player2Score;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHidden))]
    public string winner;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHidden))]
    public bool isResultEdited;

    [ObservableProperty]
    public int player1Points;

    [ObservableProperty]
    public int player2Points;

    [ObservableProperty]
    public string player1Name;

    [ObservableProperty]
    public string player2Name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHidden))]
    public bool isLocked;

    public bool IsHidden
    {
        get
        {
            return (Player1Score != 0 && Player2Score != 0 && (Player1Score != Player2Score || Winner != "Automatic")) || IsResultEdited || IsLocked;
        }
    }
    public Player? Player1
    {
        private get
        {
            return null;
        }
        set
        {
            if (value != null)
            {
                Player1ID = value.ID;
                Player1Name = value.DisplayName;
            }
        }
    }
    public Player? Player2
    {
        private get
        {
            return null;
        }
        set
        {
            if (value != null)
            {
                Player2ID = value.ID;
                Player2Name = value.DisplayName;
            }
        }
    }
    #endregion

    #region Constructor
    public Pairing(int tableNo = -1)
    {
        Winner = "Automatic";
        if (tableNo == -1)
        {
            tableNo = ++startingTableNo;
        }
        TableNo = tableNo;
        IsResultEdited = false;
        IsLocked = false;
    }

    public Pairing(Pairing p)
    {
        TableNo = p.TableNo;
        Player1ID = p.Player1ID;
        Player2ID = p.Player2ID;
        Player1Score = p.Player1Score;
        Player2Score = p.Player2Score;
        IsResultEdited = p.IsResultEdited;
        Winner = p.Winner;
        Player1Points = p.Player1Points;
        Player2Points = p.Player2Points;
        IsLocked = p.IsLocked;
        Player1Name = p.Player1Name;
        Player2Name = p.Player2Name;
    }
    #endregion
}
