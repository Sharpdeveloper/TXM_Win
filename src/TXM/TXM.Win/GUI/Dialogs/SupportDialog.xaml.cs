using System;
using System.Diagnostics;
using System.Windows;

using TXM.Core;

namespace TXM.GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class SupportDialog : Window, IInfoDialog
    {
        public SupportDialog()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            textBox.Text = text;
        }

        public new void ShowDialog()
        {
            base.ShowDialog();
        }

        private void ButtonManual_Click(object sender, RoutedEventArgs e)
        {
            var uri = "http://github.com/Sharpdeveloper/TXM/wiki/Request-support-for-another-game";
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }

        private void ButtonDonate_Click(object sender, RoutedEventArgs e)
        {
            var uri = "http://paypal.me/TKundNobody?country.x=DE&locale.x=de_DE";
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }
    }
}
