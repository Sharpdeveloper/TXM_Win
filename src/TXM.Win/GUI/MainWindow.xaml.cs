using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TXM.GUI.Dialogs;
using TXM.GUI.Windows;
using TXM.Core;

using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace TXM.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Tournament2 activeTournament;
        private ICollectionView dataView;
        private bool started = false, firststart = false;
        private TimerWindow tw;
        private IO io;

        public MainWindow()
        {
            InitializeComponent();

            io = new IO(new WindowsFile(), new WindowsMessage());

            ImageSource s = new BitmapImage(new Uri(io.IconFiles["ChangePairings"]));
            ButtonChangePairing.Content = new System.Windows.Controls.Image() { Source = s, Width=25, Height=25};
            s = new BitmapImage(new Uri(io.IconFiles["Save"]));
            ButtonSave.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["Disqualify"]));
            ButtonDisqualifyPlayer.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["EditUser"]));
            ButtonEditPlayer.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["AddUser"]));
            ButtonNewPlayer.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["Randomizer"]));
            ButtonRandom.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["Timer"]));
            ButtonTimer.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["RemoveUser"]));
            ButtonRemovePlayer.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(io.IconFiles["Reset"]));
            ButtonResetLastResults.Content = new System.Windows.Controls.Image() { Source = s, Width = 25, Height = 25 };

            DataGridPlayer.CanUserSortColumns = false;
            DataGridPlayer.SelectionMode = DataGridSelectionMode.Single;

            DataGridPlayer.Columns.Clear();
            DataGridPairing.Columns.Clear();

            DataGridTextColumn dgc;
            DataGridCheckBoxColumn dgcb;

            dgc = new DataGridTextColumn();
            dgc.Header = "#";
            dgc.Binding = new Binding("Rank");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Forename";
            dgc.Binding = new Binding("Forename");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Nickname";
            dgc.Binding = new Binding("Nickname");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgcb = new DataGridCheckBoxColumn();
            dgcb.Header = "$";
            dgcb.Binding = new Binding("Payed");
            dgcb.IsReadOnly = false;
            DataGridPlayer.Columns.Add(dgcb);
            dgcb = new DataGridCheckBoxColumn();
            dgcb.Header = "L";
            dgcb.Binding = new Binding("SquadListGiven");
            dgcb.IsReadOnly = false;
            DataGridPlayer.Columns.Add(dgcb);
            dgcb = new DataGridCheckBoxColumn();
            dgcb.Header = "!";
            dgcb.Binding = new Binding("Present");
            dgcb.IsReadOnly = false;
            DataGridPlayer.Columns.Add(dgcb);
            dgc = new DataGridTextColumn();
            dgc.Header = "Team";
            dgc.Binding = new Binding("Team");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Faction";
            dgc.Binding = new Binding("PlayersFactionAsString");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            /*dgc = new DataGridTextColumn();
            dgc.Header = "SquadPoints";
            dgc.Binding = new Binding("PointOfSquad");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);*/
            dgc = new DataGridTextColumn();
            dgc.Header = "TP";
            dgc.Binding = new Binding("Points");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "W";
            dgc.Binding = new Binding("Wins");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "L";
            dgc.Binding = new Binding("Looses");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "MoV";
            dgc.Binding = new Binding("MarginOfVictory");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "SoS";
            dgc.Binding = new Binding("PointsOfEnemies");
            dgc.IsReadOnly = true;
            DataGridPlayer.Columns.Add(dgc);

            initDataGridPairing();
        }

        private void initDataGridPairing(bool update = false)
        {
            List<String> winners = new List<string>();
            winners.Add("Automatic");
            winners.Add("Player 1");
            winners.Add("Player 2");
            DataGridPairing.Columns.Clear();

            DataGridTextColumn dgc;
            DataGridCheckBoxColumn dgcb;
            DataGridComboBoxColumn dgcbc;

            dgc = new DataGridTextColumn();
            dgc.Header = "Table";
            dgc.Binding = new Binding("TableNr");
            dgc.IsReadOnly = true;
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Player 1";
            dgc.Binding = new Binding("Player1Name");
            dgc.IsReadOnly = true;
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Player 2";
            dgc.Binding = new Binding("Player2Name");
            dgc.IsReadOnly = true;
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Score (P1)";
            dgc.Binding = new Binding("Player1Score");
            dgc.IsReadOnly = false;
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = "Score (P2)";
            dgc.Binding = new Binding("Player2Score");
            dgc.IsReadOnly = false;
            DataGridPairing.Columns.Add(dgc);
            dgcbc = new DataGridComboBoxColumn();
            dgcbc.Header = "Winner";
            dgcbc.ItemsSource = winners;
            dgcbc.TextBinding = new Binding("Winner");
            dgcbc.IsReadOnly = false;
            DataGridPairing.Columns.Add(dgcbc);
            if (update)
            {
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = "Changed?";
                Binding b = new Binding("ResultEdited");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                dgcb.Binding = b;
                DataGridPairing.Columns.Add(dgcb);
            }

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NewPlayer_Click(object sender, RoutedEventArgs e)
        {
            NewPlayerDialog npd = new NewPlayerDialog(activeTournament.Nicknames);
            npd.ShowDialog();
            if (npd.DialogReturn)
            {
                Player xwp = new Player(npd.GetNickName(), npd.SquadPoints, Player.StringToFaction(npd.GetFaction()));
                xwp.Team = npd.GetTeam();
                xwp.Name = npd.GetName();
                xwp.Forename = npd.GetForename();
                xwp.WonFreeticket = npd.FreeTicket();
                xwp.SquadListGiven = npd.SquadListGiven();
                xwp.Payed = npd.Paid();
                xwp.TableNr = npd.TableNr;
                xwp.Present = npd.Present();
                AddPlayer(xwp);
            }
            e.Handled = true;
        }

        private void NewTournament_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                if (!io.ShowMessageWithOKCancel("The current tournament will be overwritten."))
                    return;
            }
            NewTournamentDialog ntd = new NewTournamentDialog();
            ntd.ShowDialog();
            if (ntd.NewTournament)
            {
                activeTournament = new Tournament2(ntd.GetName(), ntd.GetMaxSquadSize(), ntd.GetCut());
                activeTournament.TeamProtection = ntd.TeamProtection();
                activeTournament.Single = ntd.TournamentTypeSingle();
                activeTournament.PrintDDGER = ntd.PrintDDGER();
                activeTournament.PrintDDENG = ntd.PrintDDENG();
                SetGUIState(true);
                RefreshDataGridPairings(activeTournament.Pairings);
                RefreshDataGridPlayer(activeTournament.Participants);
                SetIO();
            }
        }

        private void NewTimer_Click(object sender, RoutedEventArgs e)
        {
            tw = new TimerWindow(io);
            tw.Show();
            tw.Changed += tw_Changed;
        }

        private void tw_Changed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(PrintTime));
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            RandomizerWindow rw = new RandomizerWindow();
            rw.Show();
        }

        private void StartTournament_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament.Participants.Count != 0)
            {
                StartTournament();
                SetGUIState(false, true);
            }
            else
            {
                io.ShowMessage("Tournament can't be started without player");
            }
        }

        private void RibbonButtonEditPlayer_Click(object sender, RoutedEventArgs e)
        {
            ChangePlayer();
            e.Handled = true;
        }

        private void GOEPPImport_Click(object sender, RoutedEventArgs e)
        {
            activeTournament = io.GOEPPImport();
            if (activeTournament != null)
            {
                SetGUIState(true);
                DataGridPlayer.ItemsSource = activeTournament.Participants;
                RefreshDataGridPlayer(activeTournament.Participants);
                SetIO();
            }
        }

        private void GOEPPExport_Click(object sender, RoutedEventArgs e)
        {
            io.GOEPPExport(activeTournament);
        }

        private void EndTournament_Click(object sender, RoutedEventArgs e)
        {
            GetResults(true);
            ButtonEndTournament.IsEnabled = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            io.Save(activeTournament, false, ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Load(bool autosave = false)
        {
            Nullable<bool> overwrite = true;
            string filename = "";
            ListBoxRounds.Items.Clear();
            if (autosave)
            {
                AutosaveDialog ad = new AutosaveDialog(io);
                ad.ShowDialog();
                overwrite = ad.DialogReturn;
                filename = ad.FileName;
            }
            else
            {
                if (activeTournament != null)
                {
                    if (!io.ShowMessageWithOKCancel("The current tournament will be overwritten."))
                        overwrite = false;
                }
            }
            if (overwrite == true)
            {
                activeTournament = io.Load(filename);
                //Load Actions
                if (activeTournament != null)
                {
                    if (activeTournament.Rounds != null)
                    {
                        for (int i = 1; i <= activeTournament.Rounds.Count; i++)
                            AddRoundButton(i);
                    }
                    if (activeTournament.Rounds != null && activeTournament.Rounds.Count > 0)
                        DataGridPairing.ItemsSource = activeTournament.Rounds[activeTournament.Rounds.Count - 1].Pairings;
                    if (activeTournament.FirstRound && (activeTournament.Rounds == null || activeTournament.Rounds.Count == 0))
                    {
                        SetGUIState(true);
                    }
                    else
                    {
                        SetGUIState(false, true);
                    }
                    ButtonGetResults.Content = "Get Results";
                    ButtonGetResults.IsEnabled = activeTournament.ButtonGetResultState == true;
                    ButtonNextRound.IsEnabled = activeTournament.ButtonNextRoundState == true;
                    ButtonCut.IsEnabled = activeTournament.ButtonCutState == true;
                    activeTournament.Sort();
                    RefreshDataGridPlayer(activeTournament.Participants);
                    if (activeTournament.Pairings != null)
                        RefreshDataGridPairings(activeTournament.Pairings);
                    SetIO();
                }
            }
        }

        private void SetIO()
        {
            activeTournament.Io = new IO(new WindowsFile(), new WindowsMessage());
        }

        public void AddPlayer(Player player)
        {
            activeTournament.AddPlayer(player);
            RefreshDataGridPlayer(activeTournament.Participants);
            RefreshDataGridPairings(activeTournament.Pairings);
        }

        public void StartTournament()
        {
            firststart = true;
            started = true;
            io.Save(activeTournament, true, ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled, "TournamentStart");
        }

        public void GetSeed(bool cut = false)
        {
            List<Pairing> pairings = activeTournament.GetSeed(firststart, cut);
            RefreshDataGridPairings(pairings);
            AddRoundButton();
            ChangeGUIState(true);
            io.Save(activeTournament, true, ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled, "Pairings_Round" + activeTournament.Rounds.Count);
            firststart = false;
        }

        private bool CheckResults(List<Pairing> pairings)
        {
            foreach (Pairing p in pairings)
            {
                if (p.Player1Score != 0 && p.Player1Score < 12)
                    return false;
                if (p.Player2Score != 0 && p.Player2Score < 12)
                    return false;
                if (p.Player1Score == p.Player2Score && p.Winner == "Automatic" && !p.Player1.Freeticket && !p.Player2.Freeticket)
                    return false;
            }
            return true;
        }

        public void GetResults(bool end = false)
        {
            List<Pairing> pairings = (List<Pairing>)DataGridPairing.ItemsSource;
            if (pairings.Count == 1)
                end = true;
            bool allResultsEdited = true;
            /*foreach (Pairing p in pairings)
            {
                if (!p.ResultEdited)
                {
                    allResultsEdited = false;
                    break;
                }
            }*/
            if (allResultsEdited)
            {
                if (CheckResults(pairings))
                {
                    activeTournament.GetResults(pairings);
                }
                else
                {
                    io.ShowMessage("One ore more results are invalid.\nWinner must be 1 for Player 1 or 2 for Player 2, if the Points are equal.");
                    return;
                }
            }
            else
            {
                io.ShowMessage("There is a result missing.");
                return;
            }

            if (end)
            {
                if (!activeTournament.CutStarted)
                {
                    activeTournament.CalculateWonFreeticket();
                }
                DataGridPairing.Visibility = System.Windows.Visibility.Hidden;
                ChangeGUIState(false, true);
            }
            else
            {
                ChangeGUIState(false);
            }
            activeTournament.Sort();
            RefreshDataGridPlayer(activeTournament.Participants);
            io.Save(activeTournament, true, ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled, "Result_Round" + activeTournament.Rounds.Count);
        }

        private void PariringCurrentCellChanged(object sender, EventArgs e)
        {
            DataGridPairing.CommitEdit();
        }

        private void RefreshDataGridPlayer(List<Player> players)
        {
            DataGridPlayer.ItemsSource = null;
            DataGridPlayer.ItemsSource = players;
            dataView = CollectionViewSource.GetDefaultView(DataGridPlayer.ItemsSource);
            dataView.SortDescriptions.Clear();

            activeTournament.Sort();

            dataView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Rank", System.ComponentModel.ListSortDirection.Ascending));
            dataView.Refresh();
            RefreshRanks();
        }

        private void RefreshDataGridPairings(List<Pairing> pairings)
        {
            DataGridPairing.ItemsSource = null;
            DataGridPairing.ItemsSource = pairings;
        }

        private void DataGridPlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridPlayer.SelectedItem != null && !started)
            {
                RemovePlayerIsEnabled = true;
                EditPlayerIsEnabled = true;
            }
            else
            {
                RemovePlayerIsEnabled = false;
                EditPlayerIsEnabled = false;
            }
        }

        public void ChangePlayer()
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                Player xwp = activeTournament.Participants[DataGridPlayer.SelectedIndex];
                NewPlayerDialog npd = new NewPlayerDialog(activeTournament.Nicknames, xwp);
                npd.Title = "Player " + xwp.Nickname + " change";
                npd.ButtonOK.Content = "Accept changes.";
                npd.ShowDialog();
                if (npd.DialogReturn && npd.Changes)
                {
                    xwp.Team = npd.GetTeam();
                    xwp.Name = npd.GetName();
                    xwp.Forename = npd.GetForename();
                    xwp.WonFreeticket = npd.FreeTicket();
                    xwp.SquadListGiven = npd.SquadListGiven();
                    xwp.Payed = npd.Paid();
                    xwp.TableNr = npd.TableNr;
                    xwp.Present = npd.Present();
                    RefreshDataGridPlayer(activeTournament.Participants);
                }
            }
        }

        private void RefreshRanks()
        {
            for (int i = 1; i <= activeTournament.Participants.Count; i++)
                activeTournament.Participants[i - 1].Rank = i;
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void GridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;

            if (grid == null)
            {
                return;
            }

            // Assume first column is the checkbox column.
            if (grid.CurrentColumn == grid.Columns[5])
            {
                var gridCheckBox = (grid.CurrentColumn.GetCellContent(grid.SelectedItem) as CheckBox);

                if (gridCheckBox != null)
                {
                    gridCheckBox.IsChecked = !gridCheckBox.IsChecked;
                }
            }
        }

        private void DataGridPlayer_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            ChangePlayer();
        }

        private void ButtonNextRound_Click(object sender, RoutedEventArgs e)
        {
            GetSeed();
        }

        private void AddRoundButton(int actRound = -1)
        {
            ListBoxItem newListItem = new ListBoxItem();
            if (actRound == -1)
                actRound = activeTournament.Rounds.Count;
            newListItem.Content = "Round " + actRound;
            newListItem.MouseUp += ButtonRound_Click;
            ListBoxRounds.Items.Add(newListItem);
        }

        private void ButtonGetResults_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonGetResults.Content.ToString() == "Update")
            {
                activeTournament.GetResults((List<Pairing>)DataGridPairing.ItemsSource, true);
                RefreshDataGridPlayer(activeTournament.Participants);
                io.ShowMessage("Update done!");
                ButtonGetResults.IsEnabled = activeTournament.ButtonGetResultState;
                ButtonGetResults.Content = "Get Results";
                ButtonNextRound.IsEnabled = activeTournament.ButtonNextRoundState;
                ButtonCut.IsEnabled = activeTournament.ButtonCutState;
                ListBoxRounds.SelectedIndex = ListBoxRounds.Items.Count - 1;
                RefreshDataGridPairings(activeTournament.Pairings);
            }
            else
                GetResults();
        }

        private void ButtonRound_Click(object sender, RoutedEventArgs e)
        {
            string header = ((ListBoxItem)sender).Content.ToString();
            header = header.Remove(0, header.IndexOf(" "));
            int round = Int32.Parse(header);
            RefreshDataGridPairings(activeTournament.Rounds[round - 1].Pairings);
            RefreshDataGridPlayer(activeTournament.Rounds[round - 1].Participants);
            if (activeTournament.Rounds.Count == round)
            {
                ButtonGetResults.IsEnabled = activeTournament.ButtonGetResultState;
                ButtonGetResults.Content = "Get Results";
                ButtonNextRound.IsEnabled = activeTournament.ButtonNextRoundState;
                ButtonCut.IsEnabled = activeTournament.ButtonCutState;
                initDataGridPairing();
            }
            else
            {
                activeTournament.ButtonGetResultState = ButtonGetResults.IsEnabled;
                activeTournament.ButtonNextRoundState = ButtonNextRound.IsEnabled;
                activeTournament.ButtonCutState = ButtonCut.IsEnabled;
                ButtonGetResults.IsEnabled = true;
                ButtonGetResults.Content = "Update";
                ButtonNextRound.IsEnabled = false;
                ButtonCut.IsEnabled = false;
                initDataGridPairing(true);
            }
            activeTournament.DisplayedRound = round;
        }

        private void ButtonAutosave_Click(object sender, RoutedEventArgs e)
        {
            if (io.AutosavePathExists)
                Load(true);
            else
                io.ShowMessage("No Autosavefolder");
        }

        private void MenuItemShoAutoSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            io.ShowAutosaveFolder();
        }

        private void MenuItemLoadStatistics_Click(object sender, RoutedEventArgs e)
        {
            activeTournament.Statistics = io.LoadContents();
            io.LoadCSV(activeTournament);
            RefreshDataGridPlayer(activeTournament.Participants);
        }

        private void MenuItemTSettings_Click(object sender, RoutedEventArgs e)
        {
            NewTournamentDialog ntd = new NewTournamentDialog(activeTournament);
            ntd.ShowDialog();
            if (ntd.Changes)
            {
                activeTournament.Name = ntd.GetName();
                activeTournament.Cut = ntd.GetCut();
                activeTournament.MaxSquadPoints = ntd.GetMaxSquadSize();
                activeTournament.TeamProtection = ntd.TeamProtection();
                activeTournament.PrintDDGER = ntd.PrintDDGER();
                activeTournament.PrintDDENG = ntd.PrintDDENG();
                activeTournament.Single = ntd.TournamentTypeSingle();
            }
        }

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
        {
            GetSeed(true);
        }

        private void MenuItemDeleteAutosave_Click(object sender, RoutedEventArgs e)
        {
            io.DeleteAutosaveFolder();
        }

        private void MenuItem_Click_Table_Output(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                if (activeTournament.Rounds.Count == activeTournament.DisplayedRound)
                {
                    OutputDialog od = new OutputDialog(IO.TableForForum(activeTournament, activeTournament.Participants));
                    od.Show();
                }
                else
                {
                    OutputDialog od = new OutputDialog(IO.TableForForum(activeTournament, activeTournament.Rounds[activeTournament.DisplayedRound - 1].Participants));
                    od.Show();
                }
            }
        }

        private void MenuItem_Click_Pairing_Output(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                if (activeTournament.Rounds.Count == activeTournament.DisplayedRound)
                {
                    OutputDialog od = new OutputDialog(IO.PairingForForum(activeTournament, activeTournament.Pairings));
                    od.Show();
                }
                else
                {
                    OutputDialog od = new OutputDialog(IO.PairingForForum(activeTournament, activeTournament.Rounds[activeTournament.DisplayedRound - 1].Pairings));
                    od.Show();
                }
            }
        }

        private void MenuItem_Click_Results_Output(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                if (activeTournament.Rounds.Count - 2 >= 0)
                {
                    OutputDialog od = new OutputDialog(IO.PairingForForum(activeTournament, activeTournament.Rounds[activeTournament.Rounds.Count - 2].Pairings));
                    od.Show();
                }
            }
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                Player player = ((List<Player>)DataGridPlayer.ItemsSource)[DataGridPlayer.SelectedIndex];
                if (io.ShowMessageWithOKCancel("Do you really want to remove " + player.DisplayName + "?"))
                {
                    activeTournament.RemovePlayer(player);
                }
                RefreshDataGridPlayer(activeTournament.Participants);
            }
        }

        private void DropPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                Player player = ((List<Player>)DataGridPlayer.ItemsSource)[DataGridPlayer.SelectedIndex];
                if (io.ShowMessageWithOKCancel("The player " + player.DisplayName + " will be dropped. " + "This can't be undone."))
                {
                    activeTournament.DropPlayer(player);
                    activeTournament.Sort();
                    RefreshDataGridPlayer(activeTournament.Participants);
                }
            }
        }

        private void ButtonChangePairing_Click(object sender, RoutedEventArgs e)
        {
            SetPairingDialog spd = new SetPairingDialog(activeTournament.Participants, activeTournament.Pairings);
            spd.ShowDialog();
            if (spd.OK)
            {
                activeTournament.Pairings = spd.PremadePairing;
                RefreshDataGridPairings(activeTournament.Pairings);
                io.Save(activeTournament, true, ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled, "ChangePairings");
            }
        }

        private void MenuItemResetLastResults_Click(object sender, RoutedEventArgs e)
        {
            List<Pairing> pl = new List<Pairing>();
            foreach (var p in activeTournament.Rounds[activeTournament.Rounds.Count - 1].Pairings)
                pl.Add(new Pairing(p) { ResultEdited = true, }) ;
            activeTournament.RemoveLastRound();
            ChangeGUIState(true);
            RefreshDataGridPairings(pl);
            RefreshDataGridPlayer(activeTournament.Participants);
        }

        private void SetGUIState(bool start, bool tournamentStart = false)
        {
            if (start)
            {
                NewPlayerIsEnabled = true;
                MenuItemTSettings.IsEnabled = activeTournament != null;
                ButtonStart.IsEnabled = activeTournament != null;
                ButtonNewTournament.IsEnabled = true;
                ButtonGOEPPImport.IsEnabled = true;
                EditPlayerIsEnabled = activeTournament != null;
                RemovePlayerIsEnabled = activeTournament != null;
                ChangePairingIsEnabled = activeTournament.Pairings != null;
                SaveIsEnabled = activeTournament != null;
                ResetLastResultsIsEnabled = false;
                ButtonNextRound.IsEnabled = false;
                ButtonGetResults.IsEnabled = false;
                ButtonGetResults.Content = "Get Results";
                DisqualifyPlayerIsEnabled = false;
            }
            if (tournamentStart)
            {
                if (activeTournament.Rounds.Count > 1)
                    NewPlayerIsEnabled = false;
                else
                    NewPlayerIsEnabled = true;
                MenuItemTSettings.IsEnabled = true;
                ButtonStart.IsEnabled = false;
                ButtonGOEPPExport.IsEnabled = true;
                ButtonEndTournament.IsEnabled = true;
                EditPlayerIsEnabled = false;
                RemovePlayerIsEnabled = false;
                ChangePairingIsEnabled = true;
                SaveIsEnabled = true;
                ResetLastResultsIsEnabled = true;
                ButtonNextRound.IsEnabled = true;
                DisqualifyPlayerIsEnabled = true;
                ButtonGetResults.Content = "Get Results";
            }
        }

        private void ChangeGUIState(bool seed, bool end = false)
        {
            if (seed)
            {
                ButtonGetResults.IsEnabled = true;
                ButtonNextRound.IsEnabled = false;
                ButtonCut.IsEnabled = false;
                MenuItemResetLastResults.IsEnabled = false;
                DisqualifyPlayerIsEnabled = true;
            }
            else
            {
                ButtonGetResults.IsEnabled = false;
                MenuItemResetLastResults.IsEnabled = !end;
                ButtonNextRound.IsEnabled = !end;
                if (activeTournament.Cut == TournamentCut.NoCut || activeTournament.CutStarted)
                    ButtonCut.IsEnabled = false;
                else
                    ButtonCut.IsEnabled = true;
            }
        }

        public bool SaveIsEnabled
        {
            get
            {
                return ButtonSave.IsEnabled;
            }
            set
            {
                ButtonSave.IsEnabled = value;
                MenuItemSave.IsEnabled = value;
            }
        }

        public bool NewPlayerIsEnabled
        {
            get
            {
                return ButtonNewPlayer.IsEnabled;
            }
            set
            {
                ButtonNewPlayer.IsEnabled = value;
                MenuItemNewPlayer.IsEnabled = value;
            }
        }

        public bool EditPlayerIsEnabled
        {
            get
            {
                return ButtonEditPlayer.IsEnabled;
            }
            set
            {
                ButtonEditPlayer.IsEnabled = value;
                MenuItemEditPlayer.IsEnabled = value;
            }
        }

        public bool RemovePlayerIsEnabled
        {
            get
            {
                return ButtonRemovePlayer.IsEnabled;
            }
            set
            {
                ButtonRemovePlayer.IsEnabled = value;
                MenuItemRemovePlayer.IsEnabled = value;
            }
        }

        public bool DisqualifyPlayerIsEnabled
        {
            get
            {
                return ButtonDisqualifyPlayer.IsEnabled;
            }
            set
            {
                ButtonDisqualifyPlayer.IsEnabled = value;
                MenuItemDisqualifyPlayer.IsEnabled = value;
                MenuItemDropPlayer.IsEnabled = value;
            }
        }

        public bool ChangePairingIsEnabled
        {
            get
            {
                return ButtonChangePairing.IsEnabled;
            }
            set
            {
                ButtonChangePairing.IsEnabled = value;
                MenuItemChangePairing.IsEnabled = value;
            }
        }

        public bool ResetLastResultsIsEnabled
        {
            get
            {
                return ButtonResetLastResults.IsEnabled;
            }
            set
            {
                ButtonResetLastResults.IsEnabled = value;
                MenuItemResetLastResults.IsEnabled = value;
            }
        }

        public bool TimerIsEnabled
        {
            get
            {
                return ButtonTimer.IsEnabled;
            }
            set
            {
                ButtonTimer.IsEnabled = value;
                MenuItemTimer.IsEnabled = value;
            }
        }

        public bool RandomIsEnabled
        {
            get
            {
                return ButtonRandom.IsEnabled;
            }
            set
            {
                ButtonRandom.IsEnabled = value;
                MenuItemRandom.IsEnabled = value;
            }
        }

        private void DisqualifyPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                Player player = ((List<Player>)DataGridPlayer.ItemsSource)[DataGridPlayer.SelectedIndex];
                if (io.ShowMessageWithOKCancel("The player " + player.DisplayName + " will be disqualified. This can't be undone." ))
                {
                    activeTournament.DisqualifyPlayer(player);
                    activeTournament.Sort();
                    RefreshDataGridPlayer(activeTournament.Participants);
                }
            }
        }

        private void MenuItemShowPairings_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                BeamerWindow bw = new Windows.BeamerWindow(DataGridPairing.ItemsSource, "Pairings", false);
                bw.Show();
            }
        }

        private void MenuItemShowTable_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                BeamerWindow bw = new Windows.BeamerWindow(DataGridPlayer.ItemsSource, "Table", true);
                bw.Show();
            }
        }

        private void MenuItemPrint_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                io.Print(activeTournament);
            }
        }

        private void MenuItemTimeStart_Click(object sender, RoutedEventArgs e)
        {
            if (tw == null)
            {
                io.ShowMessage("Timer must be started first.");
            }
            else
                tw.StartTimer();
        }

        private void MenuItemTimePause_Click(object sender, RoutedEventArgs e)
        {
            if (tw == null)
            {
                io.ShowMessage("Timer must be started first.");
            }
            else
            {
                tw.PauseTimer();
                if (tw.Started)
                    MenuItemTimePause.Header = "Pause";
                else
                    MenuItemTimePause.Header = "Resume";
            }
        }

        private void MenuItemTimeReset_Click(object sender, RoutedEventArgs e)
        {
            if (tw == null)
            {
                io.ShowMessage("Timer must be started first.");
            }
            else
                tw.ResetTimer();
        }

        private void PrintTime()
        {
            MenuItemTime.Header = tw.ActualTime;
        }

        private void MenuItemPrintPairing_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                io.Print(activeTournament, false);
            }
        }

        private void RefreshPlayerList(object sender, RoutedEventArgs e)
        {
            RefreshDataGridPlayer(activeTournament.Participants);
        }

        private void RefreshPairingsList(object sender, RoutedEventArgs e)
        {
            RefreshDataGridPairings(activeTournament.Pairings);
        }

        private void MenuItemPrintParingScore_Click(object sender, RoutedEventArgs e)
        {
            io.PrintScoreSheet(activeTournament);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog ad = new AboutDialog();
            ad.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ad.ShowDialog();
        }

        private void DataGridPlayer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //(List<Player>)Data.ItemsSource
            if(e.EditAction == DataGridEditAction.Commit)
            {
                Player player = activeTournament.Participants[DataGridPlayer.SelectedIndex];
                player.Present = !player.Present;
                //activeTournament.ChangePlayer(player);
            }
        }

        private void MenuItemPrintResult_Click(object sender, RoutedEventArgs e)
        {
            if (activeTournament != null)
            {
                io.Print(activeTournament, true);
            }
        }
    }
}
