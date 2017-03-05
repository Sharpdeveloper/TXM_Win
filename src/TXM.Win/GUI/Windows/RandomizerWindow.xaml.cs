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
using System.Windows.Navigation;
using System.Windows.Shapes;

using TXM.Core;

namespace TXM.GUI.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class RandomizerWindow : Window
    {
        private Random rnd;

        public RandomizerWindow(Language lang)
        {
            InitializeComponent();

            ButtonClose.Content = lang.GetTranslation(StaticLanguage.Exit);
            ButtonRandom.Content = lang.GetTranslation(StaticLanguage.ChooseRandomPlayer);
            LabelInfo.Content = lang.GetTranslation(StaticLanguage.ChoosenPlayer);
            LabelNumberOfPlayer.Content = lang.GetTranslation(StaticLanguage.CountOfPlayer);

            rnd = new Random();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int t;
            try
            {
                t = Int32.Parse(TextBoxPlayerCount.Text);
            }
            catch
            {
                t = 16;
            }
            LabelResult.Content = rnd.Next(1, (t) + 1);
        }
    }
}
