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

using TXM.Core;

namespace TXM.GUI.Dialogs
{
    /// <summary>
    /// Interaktionslogik für OuputDialog.xaml
    /// </summary>
    public partial class OutputDialog : Window
    {
        private static double width = 800, height = 600;
        public OutputDialog(string text, Language lang)
        {
            InitializeComponent();

            Title = lang.GetTranslation(StaticLanguage.OutputTitle);
            ButtonCopy.Content = lang.GetTranslation(StaticLanguage.Copy);
            ButtonClose.Content = lang.GetTranslation(StaticLanguage.Exit);

            this.textblockOutput.Text = text;
            this.Height = height;
            this.Width = width;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.textblockOutput.Text);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            height = this.Height;
            width = this.Width;
        }
    }
}
