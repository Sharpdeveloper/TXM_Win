using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using TXM.Core;

namespace TXM.GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for LanguageDialog.xaml
    /// </summary>
    public partial class LanguageDialog : Window
    {
        public string LanguageName { get; private set; }

        public LanguageDialog(IO io)
        {
            InitializeComponent();

            ButtonLoad.Content = io.Lang.GetTranslation(StaticLanguage.Load);
            ButtonCancel.Content = io.Lang.GetTranslation(StaticLanguage.Cancel);
            Title = io.Lang.GetTranslation(StaticLanguage.LoadLanguages);

            foreach(var s in io.GetWebLanguages())
            {
                ListBoxFiles.Items.Add(s);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxFiles.SelectedValue != null)
            {
                if (ListBoxFiles.SelectedValue.ToString() != "")
                {
                    LanguageName = ListBoxFiles.SelectedValue.ToString();
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
