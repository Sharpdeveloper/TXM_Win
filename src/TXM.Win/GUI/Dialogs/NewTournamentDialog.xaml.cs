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
    /// Interaktionslogik für NewTournamentDilaog.xaml
    /// </summary>
    public partial class NewTournamentDialog : Window
    {
        public bool NewTournament { get; private set; }
        public bool Changes { get; private set; }
        private List<string> tournamentTypesXwing;

        public NewTournamentDialog(Language lang, Tournament2 tournament = null)
        {
            InitializeComponent();

            tournamentTypesXwing = new List<string>();
            tournamentTypesXwing.Add(lang.GetTranslation(StaticLanguage.SWISS));

            foreach (string s in tournamentTypesXwing)
                ComboBoxTournamentType.Items.Add(s);

            ComboBoxTournamentType.SelectedIndex = 0;
            Changes = false;

            LabelTournamentForm.Content = "1. " + lang.GetTranslation(StaticLanguage.ChooseTournamentType);
            LabelCut.Content = "2. " +  lang.GetTranslation(StaticLanguage.CutOrNoCut);
            LabelTournamentName.Content = "3. " + lang.GetTranslation(StaticLanguage.TournamentName);
            LabelListsize.Content = "4. " + lang.GetTranslation(StaticLanguage.MaximalListPoints);
            ButtonOK.Content = lang.GetTranslation(StaticLanguage.CreateTournament);
            Title = lang.GetTranslation(StaticLanguage.CreateTournament);
            RadioButtonTop4.Content = lang.GetTranslation(StaticLanguage.TOP4);
            RadioButtonTop8.Content = lang.GetTranslation(StaticLanguage.TOP8);
            RadioButtonTop16.Content = lang.GetTranslation(StaticLanguage.TOP16);
            RadioButtonNoCut.Content = lang.GetTranslation(StaticLanguage.NoCut);
            ButtonCancel.Content = lang.GetTranslation(StaticLanguage.Cancel);

            if(tournament != null)
            {
                TextboxName.Text = tournament.Name;
                if (tournament.Cut == TournamentCut.Top4)
                    RadioButtonTop4.IsChecked = true;
                else if (tournament.Cut == TournamentCut.Top8)
                    RadioButtonTop8.IsChecked = true;
                else if (tournament.Cut == TournamentCut.Top16)
                    RadioButtonTop16.IsChecked = true;
                else
                    RadioButtonNoCut.IsChecked = true;
                radioButtonTPYes.IsChecked = tournament.TeamProtection;
                radioButtonTPNo.IsChecked = !tournament.TeamProtection;
                TextBoxMaxSquad.Text = tournament.MaxSquadPoints.ToString();
                ButtonOK.Content = lang.GetTranslation(StaticLanguage.Save);
                Title = lang.GetTranslation(StaticLanguage.ChangeTournament);
            }
        }

        private void NewTournament_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxTournamentType.SelectedValue == null)
            {
                System.Windows.MessageBox.Show("Der Turniermodus muss ausgewählt werden.", "Hinweis");
                return;
            }
            if (TextboxName.Text == "")
            {
                System.Windows.MessageBox.Show("Der Turniername muss ausgewählt werden.", "Hinweis");
                return;
            }
            NewTournament = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NewTournament = false;
            Changes = false;
            this.Close();
        }

        public string GetTournamentMode()
        {
            return ComboBoxTournamentType.SelectedValue.ToString();
        }

        public string GetName()
        {
            return TextboxName.Text;
        }

        public int GetMaxSquadSize()
        {
            int t;
            try
            {
                t = Int32.Parse(TextBoxMaxSquad.Text);
            }
            catch
            {
                t = 100;
            }
            return t;
        }

        public TournamentCut GetCut()
        {
            if (RadioButtonTop4.IsChecked == true)
                return TournamentCut.Top4;
            else if (RadioButtonTop8.IsChecked == true)
                return TournamentCut.Top8;
            else if (RadioButtonTop16.IsChecked == true)
                return TournamentCut.Top16;
            else
                return TournamentCut.NoCut;
        }

        public bool TeamProtection()
        {
            return radioButtonTPYes.IsChecked == true;
        }

        public bool PrintDDGER()
        {
            return radioButtonPDD_GER.IsChecked == true;
        }

        public bool PrintDDENG()
        {
            return radioButtonPDD_ENG.IsChecked == true;
        }

        public bool TournamentTypeSingle()
        {
            return radioButtonTypeSingle.IsChecked == true;
        }

        private void ComboBoxTournamentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }

        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }

        private void IntegerUpDownMaxSquad_ValueChanged(object sender, TextChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }
    }
}
