using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using TXM.Core.Models;

namespace TXM.Win
{
    public partial class MainWindow
    {
        private ICollectionView dataView;
        private Core.TournamentController tournamentController;
        private int refresh = 0;
        private List<Pairing> currentPairingList;
        private bool hide = false;
        private string currentScenario;

        public MainWindow()
        {
            InitializeComponent();

            Closing += WindowClosed;

            tournamentController = new Core.TournamentController();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowClosed(object sender, CancelEventArgs e)
        {
            tournamentController.Close();
        }

        private void Refresh()
        {
            //currentPairingList = tournamentController.ActiveTournament.Rounds[^1].Pairings.ToList();
            RefreshDataGridPairings();
            //RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants.ToList());
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule != null && tournamentController.ActiveTournament.Rule.UsesScenarios)
            {
                this.LabelScenario.Visibility = Visibility.Visible;
                this.ComboboxScenarios.Visibility = Visibility.Visible;
                this.LabelScenarios.Visibility = Visibility.Visible;
                SetScenarios();
            }
        }

        private void SetScenarios()
        {
            ComboboxScenarios.Items.Clear();
            foreach (var s in tournamentController.ActiveTournament.ActiveScenarios)
            {
                ComboBoxItem newListItem = new ComboBoxItem();
                newListItem.Content = s;
                ComboboxScenarios.Items.Add(newListItem);
            }
            ComboboxScenarios.SelectedIndex = 0;
        }

        private void GetSeed(bool cut = false)
        {
            // List<Pairing> pairings = tournamentController.GetSeed(cut);
            // currentPairingList = pairings;
            RefreshDataGridPairings();
            //AddRoundButton();
            //tournamentController.Save(ButtonGetResults.Content.ToString(), ButtonCut.IsEnabled, true);
            if (tournamentController.ActiveTournament.Rule.UsesScenarios)
            {
                LabelScenarios.Content = "Selected Scenario: " + tournamentController.ActiveTournament.ChosenScenario;
                SetScenarios();
            }
        }

        private void PariringCurrentCellChanged(object sender, EventArgs e)
        {
            //DataGridPairing.CommitEdit();
            refresh++;
            if (refresh >= 5 && currentPairingList != null)
            {
                RefreshDataGridPairings();
                refresh = 0;
            }
        }

        private void RefreshDataGridPlayer(List<Player> players)
        {
            DataGridPlayer.ItemsSource = null;
            DataGridPlayer.ItemsSource = players;
            dataView = CollectionViewSource.GetDefaultView(DataGridPlayer.ItemsSource);
            dataView.SortDescriptions.Clear();

            tournamentController.ActiveTournament.Sort();

            dataView.SortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Ascending));
            dataView.Refresh();
        }

        private void RefreshDataGridPairings()
        {
            if (currentPairingList == null)
                return;
            //DataGridPairing.ItemsSource = null;
            List<Pairing> p = new List<Pairing>();
            if (hide)
            {
                foreach (var pa in currentPairingList)
                {
                    if (!pa.IsHidden)
                        p.Add(pa);
                }
            }
            else
            {
                p = currentPairingList;
            }
            //DataGridPairing.ItemsSource = p;
        }

        private void DataGridPlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if (DataGridPlayer.SelectedItem != null)
            // {
            //     RemovePlayerIsEnabled = true;
            //     DisqualifyPlayerIsEnabled = tournamentController.Started;
            //     EditPlayerIsEnabled = true;
            // }
            // else
            // {
            //     RemovePlayerIsEnabled = false;
            //     EditPlayerIsEnabled = false;
            //     DisqualifyPlayerIsEnabled = false;
            // }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                var cell = (DataGridCell)e.OriginalSource;
                if(cell.Content.GetType() == typeof(CheckBox))
                {
                    var cb = (CheckBox)cell.Content;
                    cb.IsChecked = !cb.IsChecked;
                }
                else
                {
                    // Starts the Edit on the row;
                    DataGrid grd = (DataGrid)sender;
                    grd.BeginEdit(e);
                }  
            }
        }

        private void GridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;

            if (grid == null)
            {
                return;
            }

            try
            {
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
            catch (ArgumentOutOfRangeException) { }
            catch (ArgumentNullException) { }
        }

        private void ButtonNextRound_Click(object sender, RoutedEventArgs e)
        {
            GetSeed();
        }

        // private void ButtonGetResults_Click(object sender, RoutedEventArgs e)
        // {
        //     if (ButtonGetResults.Content.ToString() == "Start Tournament")
        //     {
        //         if (tournamentController.StartTournament(ButtonGetResults.Content.ToString(), ButtonCut.IsEnabled))
        //         {
        //             SetGUIState(false, true);
        //         }
        //         return;
        //     }
        //     if (ButtonGetResults.Content.ToString() == "Next Round")
        //     {
        //         GetSeed();
        //         return;
        //     }
        //     if (ButtonGetResults.Content.ToString() == "Update")
        //     {
        //        // if (tournamentController.GetResults((List<Pairing>)DataGridPairing.ItemsSource, ButtonGetResults.Content.ToString(), false, true))
        //        // {
        //             //RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants.ToList());
        //             tournamentController.ActiveIO.ShowMessage("Update done!");
        //             ButtonGetResults.IsEnabled = true;
        //             ButtonGetResults.Content = tournamentController.ActiveTournament.ButtonGetResultsText;
        //             ButtonCut.IsEnabled = tournamentController.ActiveTournament.ButtonCutState;
        //             ComboBoxRounds.SelectedIndex = ComboBoxRounds.Items.Count - 1;
        //            // currentPairingList = tournamentController.ActiveTournament.Rounds[^1].Pairings;
        //             RefreshDataGridPairings();
        //             ButtonGetResults.ToolTip = ButtonGetResults.Content.ToString();
        //            // tournamentController.Save(ButtonGetResults.Content.ToString(), false, true, "Update_Round");
        //        // }
        //         return;
        //     }
        //     if (ButtonGetResults.Content.ToString() == "Get Results")
        //     {
        //     //    if (tournamentController.GetResults((List<Pairing>)DataGridPairing.ItemsSource, ButtonGetResults.Content.ToString(), ButtonCut.IsEnabled))
        //     //    {
        //            // RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants.ToList());
        //             ChangeGUIState(false);
        //          //   tournamentController.Save(ButtonGetResults.Content.ToString(), false, true, "Result_Round");
        //             LabelScenarios.Content = "";
        //    //     }
        //         return;
        //     }
        // }

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
        {
            GetSeed(true);
        }

        private void DataGridPlayer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //(List<Player>)Data.ItemsSource
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (DataGridPlayer.SelectedItems.Count > 1)
                    return;
                Player player = tournamentController.ActiveTournament.Participants[DataGridPlayer.SelectedIndex];
                player.IsPresent = !player.IsPresent;
                //activeTournament.ChangePlayer(player);
            }
        }

        private void DataGridPairing_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.DisplayIndex != 0)
            {
                int t = e.Row.GetIndex();
                // if (tournamentController.ActiveTournament.Pairings[e.Row.GetIndex()].Locked)
                // {
                //     e.Cancel = true;
                // }
            }
        }

        private void SliderHide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //hide = SliderHide.Value == 2.0;
            RefreshDataGridPairings();
        }

        private void ComboboxScenarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedValue != null)
                tournamentController.ActiveTournament.ChosenScenario = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
        }
    }
}
