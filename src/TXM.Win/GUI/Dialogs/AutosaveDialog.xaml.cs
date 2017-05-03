using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

using TXM.Core;

namespace TXM.GUI.Dialogs
{
    /// <summary>
    /// Interaktionslogik für AutosaveDialog.xaml
    /// </summary>
    public partial class AutosaveDialog : Window
    {
        public bool DialogReturn { get; private set; }
        public string FileName { get; private set; }
        public AutosaveDialog(IO io)
        {
            InitializeComponent();
            string[] filenames = io.GetAutosaveFiles();
            foreach(string s in filenames)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = s;
                ListBoxFiles.Items.Add(lbi);
            }
            DialogReturn = false;

            ButtonCancel.Content = "Cancel";
            ButtonLoad.Content = "Load";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogReturn = true;
            FileName = ((ListBoxItem)ListBoxFiles.SelectedItem).Content.ToString();
            Close();
        }
    }
}
