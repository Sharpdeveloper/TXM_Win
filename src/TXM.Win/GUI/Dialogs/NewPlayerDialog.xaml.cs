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
    /// Interaktionslogik für NewPlayerDialog.xaml
    /// </summary>
    public partial class NewPlayerDialog : Window
    {
        public bool DialogReturn { get; private set; }
        public bool Changes { get; private set; }
        public int SquadPoints { get; private set; }
        public Player CurrentPlayer { get; private set; }
        private List<string> Nicknames;
        private bool uniqueName = false;
        private bool nicknameRequiered = true;

        public NewPlayerDialog(List<string> nicknames, Player player = null)
        {
            InitializeComponent();
            Nicknames = nicknames;
            TextBoxNickname.Focus();
            Changes = false;
            //Noch steuern, welcher Spieler angelegt werden soll
            foreach (string s in Player.GetFactions())
                ComboBoxFaction.Items.Add(s);
            if(player !=null)
            {
                CurrentPlayer = player;
                TextBoxForename.Text = CurrentPlayer.Forename;
                TextBoxName.Text = CurrentPlayer.Name;
                TextBoxNickname.Text = CurrentPlayer.Nickname;
                TextBoxTeam.Text = CurrentPlayer.Team;
                if (Player.StringToFaction("Imperium") == ((Player)CurrentPlayer).PlayersFaction)
                    ComboBoxFaction.SelectedIndex = 0;
                else if (Player.StringToFaction("Rebels") == ((Player)CurrentPlayer).PlayersFaction)
                    ComboBoxFaction.SelectedIndex = 1;
                else
                    ComboBoxFaction.SelectedIndex = 2;
                CheckboxFreeticket.IsChecked = ((Player)CurrentPlayer).WonFreeticket;
                CheckboxPayed.IsChecked = CurrentPlayer.Payed;
                CheckboxSquadListGiven.IsChecked = CurrentPlayer.SquadListGiven;
                TextBoxNickname.IsEnabled = false;
                uniqueName = true;
                nicknameRequiered = false;
                TextBoxTableNR.Text = player.TableNr.ToString();
                CheckboxPresent.IsChecked = CurrentPlayer.Present;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogReturn = false;
            this.Close();
        }

        private void NewPlayer_Click(object sender, RoutedEventArgs e)
        {
            NewPlayer();
        }

        private void NewPlayer()
        {
            if (TextBoxNickname.Text == "" && nicknameRequiered)
                return;
            else if (ComboBoxFaction.SelectedIndex == -1)
                return;
            else
            {
                DialogReturn = true;
                this.Close();
            }
        }

        public string GetFaction()
        {
            return ComboBoxFaction.SelectedValue.ToString();
        }

        public string GetNickName()
        {
            return TextBoxNickname.Text;                
        }

        public string GetName()
        {
            return TextBoxName.Text;
        }

        public string GetForename()
        {
            return TextBoxForename.Text;
        }

        public string GetTeam()
        {
            return TextBoxTeam.Text;
        }

        private void ValueChanged(object sender, TextChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }

        private void ValueChanged(object sender, SelectionChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }

        public bool FreeTicket()
        {
            return CheckboxFreeticket.IsChecked == true;
        }

        public bool Present()
        {
            return CheckboxPresent.IsChecked == true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                NewPlayer();
            }
        }

        public int TableNr
        {
            get
            {
                int t;
                try
                {
                    t = Int32.Parse(TextBoxTableNR.Text);
                }
                catch
                {
                    t = 0;
                }
                return t;
            }
        }

        public bool Paid()
        {
            return CheckboxPayed.IsChecked == true;
        }

        public bool SquadListGiven()
        {
            return CheckboxSquadListGiven.IsChecked == true;
        }

        private void TextBoxNickname_TextChanged(object sender, TextChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }

        private void IntegerUpDownTableNr_ValueChanged(object sender, TextChangedEventArgs e)
        {
            Changes = true;
            e.Handled = true;
        }
    }
}
