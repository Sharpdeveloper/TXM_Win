using System.Windows;

namespace TXM.Win.Dialogs;

    public partial class AutoSaveDialog
    {
        public AutoSaveDialog()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void DataGridAutosave_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogResult = DataGridAutosave.SelectedItem != null;
            Close();
        }
    }