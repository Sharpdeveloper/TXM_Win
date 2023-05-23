using System.Windows;

namespace TXM.Win.Dialogs;

public partial class TournamentDialog
{
    public TournamentDialog()
    {
        InitializeComponent();
    }

    private void NewTournament_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}