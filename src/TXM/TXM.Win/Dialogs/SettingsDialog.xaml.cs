using System.Windows;

namespace TXM.Win.Dialogs;

public partial class SettingsDialog : Window
{
    public SettingsDialog()
    {
        InitializeComponent();
    }
    
    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}