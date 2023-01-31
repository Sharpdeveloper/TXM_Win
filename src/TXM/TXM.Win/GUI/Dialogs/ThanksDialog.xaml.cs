using System.Windows;

using TXM.Core;

namespace TXM.GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for ThanksDialog.xaml
    /// </summary>
    public partial class ThanksDialog : Window, IInfoDialog
    {
        public ThanksDialog()
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
    }
}
