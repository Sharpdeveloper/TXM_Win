using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TXM.GUI.Dialogs;
using TXM.GUI.Windows;
using TXM.Core;

namespace TXM.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICollectionView dataView;
        private TournamentController tournamentController;

        public MainWindow()
        {
            InitializeComponent();

            tournamentController = new TournamentController(new IO(new WindowsFile(), new WindowsMessage()));

            tournamentController.ActiveTimer.Changed += Time_Changed;

            ImageSource s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["ChangePairings"]));
            ButtonChangePairing.Content = new Image() { Source = s, Width=25, Height=25};
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["Save"]));
            ButtonSave.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["Disqualify"]));
            ButtonDisqualifyPlayer.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["EditUser"]));
            ButtonEditPlayer.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["AddUser"]));
            ButtonNewPlayer.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["Timer"]));
            ButtonTimer.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["RemoveUser"]));
            ButtonRemovePlayer.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["Reset"]));
            ButtonResetLastResults.Content = new Image() { Source = s, Width = 25, Height = 25 };
            s = new BitmapImage(new Uri(tournamentController.ActiveIO.IconFiles["TXM_Logo"]));
            ImageLogo.Source = s;

            DataGridPlayer.SelectionMode = DataGridSelectionMode.Single;

            InitDataGridPlayer();
            InitDataGridPairing();
        }

        private void InitDataGridPlayer()
        {
            DataGridPlayer.Columns.Clear();

            DataGridTextColumn dgc;
            DataGridCheckBoxColumn dgcb;

            dgc = new DataGridTextColumn()
            {
                Header = "#",
                Binding = new Binding("Rank"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Forename",
                Binding = new Binding("Forename"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Nickname",
                Binding = new Binding("Nickname"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            dgcb = new DataGridCheckBoxColumn()
            {
                Header = "$",
                Binding = new Binding("Paid"),
                IsReadOnly = false
            };
            DataGridPlayer.Columns.Add(dgcb);
            dgcb = new DataGridCheckBoxColumn()
            {
                Header = "L",
                Binding = new Binding("ListGiven"),
                IsReadOnly = false
            };
            DataGridPlayer.Columns.Add(dgcb);
            dgcb = new DataGridCheckBoxColumn()
            {
                Header = "!",
                Binding = new Binding("Present"),
                IsReadOnly = false
            };
            DataGridPlayer.Columns.Add(dgcb);
            dgc = new DataGridTextColumn()
            {
                Header = "Team",
                Binding = new Binding("Team"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Faction",
                Binding = new Binding("Faction"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "TP",
                Binding = new Binding("TournamentPoints"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "W",
                Binding = new Binding("Wins"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.OptionalFields.Contains("ModWins"))
            {
                dgc = new DataGridTextColumn()
                {
                    Header = "MW",
                    Binding = new Binding("ModifiedWins"),
                    IsReadOnly = true
                };
                DataGridPlayer.Columns.Add(dgc);
            }
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.OptionalFields.Contains("Draws"))
            {
                dgc = new DataGridTextColumn()
                {
                    Header = "D",
                    Binding = new Binding("Draws"),
                    IsReadOnly = true
                };
                DataGridPlayer.Columns.Add(dgc);
            }
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.OptionalFields.Contains("ModLoss"))
            {
                dgc = new DataGridTextColumn()
                {
                    Header = "ML",
                    Binding = new Binding("ModifiedLosses"),
                    IsReadOnly = true
                };
                DataGridPlayer.Columns.Add(dgc);
            }
            dgc = new DataGridTextColumn()
            {
                Header = "L",
                Binding = new Binding("Losses"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.OptionalFields.Contains("MoV"))
            {
                dgc = new DataGridTextColumn()
                {
                    Header = "MoV",
                    Binding = new Binding("MarginOfVictory"),
                    IsReadOnly = true
                };
                DataGridPlayer.Columns.Add(dgc);
            }
            dgc = new DataGridTextColumn()
            {
                Header = "SoS",
                Binding = new Binding("StrengthOfSchedule"),
                IsReadOnly = true
            };
            DataGridPlayer.Columns.Add(dgc);
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.OptionalFields.Contains("eSoS"))
            {
                dgc = new DataGridTextColumn()
                {
                    Header = "eSoS",
                    Binding = new Binding("ExtendedStrengthOfSchedule"),
                    IsReadOnly = true
                };
                DataGridPlayer.Columns.Add(dgc);
            }
        }

        private void InitDataGridPairing(bool update = false)
        {
            List<String> winners = new List<string>
            {
                "Automatic",
                "Player 1",
                "Player 2"
            };
            DataGridPairing.Columns.Clear();

            DataGridTextColumn dgc;
            DataGridCheckBoxColumn dgcb;
            DataGridComboBoxColumn dgcbc;

            dgc = new DataGridTextColumn()
            {
                Header = "T#",
                Binding = new Binding("TableNr"),
                IsReadOnly = true
            };
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Player 1",
                Binding = new Binding("Player1Name"),
                IsReadOnly = true
            };
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Player 2",
                Binding = new Binding("Player2Name"),
                IsReadOnly = true
            };
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Score (P1)",
                Binding = new Binding("Player1Score"),
                IsReadOnly = false
            };
            DataGridPairing.Columns.Add(dgc);
            dgc = new DataGridTextColumn()
            {
                Header = "Score (P2)",
                Binding = new Binding("Player2Score"),
                IsReadOnly = false
            };
            DataGridPairing.Columns.Add(dgc);
            if (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.IsWinnerDropDownNeeded)
            {
                dgcbc = new DataGridComboBoxColumn()
                {
                    Header = "Winner",
                    ItemsSource = winners,
                    TextBinding = new Binding("Winner"),
                    IsReadOnly = false
                };
                DataGridPairing.Columns.Add(dgcbc);
            }
            if (!update && (tournamentController.ActiveTournament != null && tournamentController.ActiveTournament.Rule.IsDrawPossible))
            {
                dgcb = new DataGridCheckBoxColumn()
                {
                    Header = "OK?"
                };
                Binding b = new Binding("ResultEdited")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                dgcb.Binding = b;
                DataGridPairing.Columns.Add(dgcb);
            }
            if ((update))
            {
                dgcb = new DataGridCheckBoxColumn()
                {
                    Header = "Changed?"
                };
                Binding b = new Binding("ResultEdited")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
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
            tournamentController.NewPlayer(new NewPlayerDialog(tournamentController.ActiveTournament.Rule));
            Refresh();
            e.Handled = true;
        }

        private void Refresh()
        {
            RefreshDataGridPairings(tournamentController.ActiveTournament.Pairings);
            RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
        }

        private void NewTournament_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.NewTournament(new NewTournamentDialog());
            if(tournamentController.ActiveTournament != null)
            {
                SetGUIState(true);
                Refresh();
                InitDataGridPlayer();
                InitDataGridPairing();
            }
            e.Handled = true;
        }

        private void NewTimer_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.OpenTimerWindow(new TimerWindow());
        }

        private void Time_Changed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(PrintTime));
        }

        private void StartTournament_Click(object sender, RoutedEventArgs e)
        {
            if(tournamentController.StartTournament(ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled))
            {
                SetGUIState(false, true);
            }
        }

        private void RibbonButtonEditPlayer_Click(object sender, RoutedEventArgs e)
        {
            EditPlayer();
            e.Handled = true;
        }

        private void EditPlayer()
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                tournamentController.EditPlayer(new NewPlayerDialog(tournamentController.ActiveTournament.Rule), DataGridPlayer.SelectedIndex);
                RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
            }
        }

        private void GOEPPImport_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.Import(new NewTournamentDialog(), false);
            if (tournamentController.ActiveTournament != null)
            {
                ComboBoxRounds.Items.Clear();
                InitDataGridPlayer();
                InitDataGridPairing();
                SetGUIState(true);
                DataGridPlayer.ItemsSource = tournamentController.ActiveTournament.Participants;
                RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
            }
        }

        private void GOEPPExport_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.GOEPPExport();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.Save(ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Load(bool autosave = false)
        {
            tournamentController.Load(new AutosaveDialog(), autosave);
            if(tournamentController.ActiveTournament != null)
            {
                ComboBoxRounds.Items.Clear();
                if (tournamentController.ActiveTournament.Rounds != null)
                {
                    for (int i = 1; i <= tournamentController.ActiveTournament.Rounds.Count; i++)
                        AddRoundButton(i);
                    if (tournamentController.ActiveTournament.Rounds != null && tournamentController.ActiveTournament.Rounds.Count > 0)
                        DataGridPairing.ItemsSource = tournamentController.ActiveTournament.Rounds[tournamentController.ActiveTournament.Rounds.Count - 1].Pairings;
                    if (tournamentController.ActiveTournament.FirstRound && (tournamentController.ActiveTournament.Rounds == null || tournamentController.ActiveTournament.Rounds.Count == 0))
                    {
                        SetGUIState(true);
                    }
                    else
                    {
                        SetGUIState(false, true);
                    }
                    ButtonGetResults.Content = "Get Results";
                    ButtonGetResults.IsEnabled = tournamentController.ActiveTournament.ButtonGetResultState == true;
                    ButtonNextRound.IsEnabled = tournamentController.ActiveTournament.ButtonNextRoundState == true;
                    ButtonCut.IsEnabled = tournamentController.ActiveTournament.ButtonCutState == true;
                    tournamentController.ActiveTournament.Sort();
                    RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
                    if (tournamentController.ActiveTournament.Pairings != null)
                        RefreshDataGridPairings(tournamentController.ActiveTournament.Pairings);

                    InitDataGridPlayer();
                    InitDataGridPairing();

                }
            }
            ButtonGetResults.IsEnabled = true;
            ButtonCut.IsEnabled = true;
            ButtonNextRound.IsEnabled = true;
        }

        private void GetSeed(bool cut = false)
        {
            List<Pairing> pairings = tournamentController.GetSeed(cut);
            RefreshDataGridPairings(pairings);
            AddRoundButton();
            ChangeGUIState(true);
            tournamentController.Save(ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled, true);
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

            tournamentController.ActiveTournament.Sort();

            dataView.SortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Ascending));
            dataView.Refresh();
        }

        private void RefreshDataGridPairings(List<Pairing> pairings)
        {
            DataGridPairing.ItemsSource = null;
            DataGridPairing.ItemsSource = pairings;
        }

        private void DataGridPlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridPlayer.SelectedItem != null && !tournamentController.Started)
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

            try { 
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
        }

        private void DataGridPlayer_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            EditPlayer();
            e.Handled = true;
        }

        private void ButtonNextRound_Click(object sender, RoutedEventArgs e)
        {
            GetSeed();
        }

        private void AddRoundButton(int actRound = -1)
        {
            ComboBoxItem newListItem = new ComboBoxItem();
            if (actRound == -1)
                actRound = tournamentController.ActiveTournament.Rounds.Count;
            newListItem.Content = actRound;
            ComboBoxRounds.Items.Add(newListItem);
        }

        private void ButtonGetResults_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonGetResults.Content.ToString() == "Update")
            {
                if (tournamentController.GetResults((List<Pairing>)DataGridPairing.ItemsSource, false, false, false, true))
                {
                    RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
                    tournamentController.ActiveIO.ShowMessage("Update done!");
                    ButtonGetResults.IsEnabled = tournamentController.ActiveTournament.ButtonGetResultState;
                    ButtonGetResults.Content = "Get Results";
                    ButtonNextRound.IsEnabled = tournamentController.ActiveTournament.ButtonNextRoundState;
                    ButtonCut.IsEnabled = tournamentController.ActiveTournament.ButtonCutState;
                    ComboBoxRounds.SelectedIndex = ComboBoxRounds.Items.Count - 1;
                    RefreshDataGridPairings(tournamentController.ActiveTournament.Pairings);
                }
            }
            else
            {
                if (tournamentController.GetResults((List<Pairing>)DataGridPairing.ItemsSource, ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled))
                {
                    RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
                    ChangeGUIState(false);
                }
            }
        }

        private void ComboBoxRounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string header = ((ComboBox)sender).SelectedValue.ToString();
            header = header.Remove(0, header.IndexOf(" "));
            int round = Int32.Parse(header);
            RefreshDataGridPairings(tournamentController.ActiveTournament.Rounds[round - 1].Pairings);
            RefreshDataGridPlayer(tournamentController.ActiveTournament.Rounds[round - 1].Participants);
            if (tournamentController.ActiveTournament.Rounds.Count == round)
            {
                ButtonGetResults.IsEnabled = tournamentController.ActiveTournament.ButtonGetResultState;
                ButtonGetResults.Content = "Get Results";
                ButtonNextRound.IsEnabled = tournamentController.ActiveTournament.ButtonNextRoundState;
                ButtonCut.IsEnabled = tournamentController.ActiveTournament.ButtonCutState;
                InitDataGridPairing();
            }
            else
            {
                tournamentController.ActiveTournament.ButtonGetResultState = ButtonGetResults.IsEnabled;
                tournamentController.ActiveTournament.ButtonNextRoundState = ButtonNextRound.IsEnabled;
                tournamentController.ActiveTournament.ButtonCutState = ButtonCut.IsEnabled;
                ButtonGetResults.IsEnabled = true;
                ButtonGetResults.Content = "Update";
                ButtonNextRound.IsEnabled = false;
                ButtonCut.IsEnabled = false;
                InitDataGridPairing(true);
            }
            tournamentController.ActiveTournament.DisplayedRound = round;
        }

        private void ButtonAutosave_Click(object sender, RoutedEventArgs e)
        {
            if (tournamentController.ActiveIO.AutosavePathExists)
                Load(true);
            else
                tournamentController.ActiveIO.ShowMessage("No Autosavefolder");
        }

        private void MenuItemOpenAutoSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.ActiveIO.OpenAutosaveFolder();
        }

        private void MenuItemTSettings_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.EditTournament(new NewTournamentDialog());           
        }

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
        {
            GetSeed(true);
        }

        private void MenuItemDeleteAutosave_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.ActiveIO.DeleteAutosaveFolder();
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                tournamentController.RemovePlayer(DataGridPlayer.SelectedIndex);
                RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
            }
        }

        private void ButtonChangePairing_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.EditPairings(new SetPairingDialog(), ButtonGetResults.IsEnabled, ButtonNextRound.IsEnabled, ButtonCut.IsEnabled);
            RefreshDataGridPairings(tournamentController.ActiveTournament.Pairings);
        }

        private void MenuItemResetLastResults_Click(object sender, RoutedEventArgs e)
        {
            List<Pairing> pl = tournamentController.ResetLastResults();
            ChangeGUIState(true);
            RefreshDataGridPairings(pl);
            RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
        }

        private void SetGUIState(bool start, bool tournamentStart = false)
        {
            if (start)
            {
                NewPlayerIsEnabled = true;
                MenuItemTSettings.IsEnabled = tournamentController.ActiveTournament != null;
                ButtonStart.IsEnabled = tournamentController.ActiveTournament != null;
                ButtonNewTournament.IsEnabled = true;
                ButtonGOEPPImport.IsEnabled = true;
                EditPlayerIsEnabled = tournamentController.ActiveTournament != null;
                RemovePlayerIsEnabled = tournamentController.ActiveTournament != null;
                ChangePairingIsEnabled = tournamentController.ActiveTournament.Pairings != null;
                SaveIsEnabled = tournamentController.ActiveTournament != null;
                ResetLastResultsIsEnabled = false;
                ButtonNextRound.IsEnabled = false;
                ButtonGetResults.IsEnabled = false;
                ButtonGetResults.Content = "Get Results";
                DisqualifyPlayerIsEnabled = false;
            }
            if (tournamentStart)
            {
                if (tournamentController.ActiveTournament.Rounds.Count > 1)
                    NewPlayerIsEnabled = false;
                else
                    NewPlayerIsEnabled = true;
                MenuItemTSettings.IsEnabled = true;
                ButtonStart.IsEnabled = false;
                ButtonGOEPPExport.IsEnabled = true;
                //ButtonEndTournament.IsEnabled = true;
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
                if (tournamentController.ActiveTournament.Cut == TournamentCut.NoCut || tournamentController.ActiveTournament.CutStarted)
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

        private void DisqualifyPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPlayer.SelectedIndex >= 0)
            {
                tournamentController.RemovePlayer(DataGridPlayer.SelectedIndex, true);
                RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
            }
        }

        private void MenuItemShowPairings_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.ShowProjector(new ProjectorWindow(), false);
        }

        private void MenuItemShowTable_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.ShowProjector(new ProjectorWindow(), true);
        }

        private void MenuItemPrint_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.Print(true);
        }

        private void MenuItemTimeStart_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.StartTimer();
        }

        private void MenuItemTimePause_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.PauseTimer();
            if (tournamentController.ActiveTimer.Started)
                ButtonPause.Content = "Pause";
            else
                ButtonPause.Content = "Resume";
        }

        private void MenuItemTimeReset_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.ResetTimer();
        }

        private void PrintTime()
        {
            LabelTime.Content = tournamentController.ActiveTimer.ActualTime;
        }

        private void MenuItemPrintPairing_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.Print(true, true);
        }

        private void RefreshPlayerList(object sender, RoutedEventArgs e)
        {
            RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
        }

        private void RefreshPairingsList(object sender, RoutedEventArgs e)
        {
            RefreshDataGridPairings(tournamentController.ActiveTournament.Pairings);
        }

        private void MenuItemPrintParingScore_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.PrintScoreSheet();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog ad = new AboutDialog()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            tournamentController.ShowAbout(ad);
        }

        private void DataGridPlayer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //(List<Player>)Data.ItemsSource
            if(e.EditAction == DataGridEditAction.Commit)
            {
                Player player = tournamentController.ActiveTournament.Participants[DataGridPlayer.SelectedIndex];
                player.Present = !player.Present;
                //activeTournament.ChangePlayer(player);
            }
        }

        private void MenuItemPrintResult_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.Print(true, true, true);
        }

        private void MenuItemBBCode_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.GetBBCode(new OutputDialog(), new WindowsClipboard());
        }

        private void MenuItemCSVImport_Click(object sender, RoutedEventArgs e)
        {
            tournamentController.Import(new NewTournamentDialog(), true);
            if (tournamentController.ActiveTournament != null)
            {
                ComboBoxRounds.Items.Clear();
                InitDataGridPlayer();
                InitDataGridPairing();
                SetGUIState(true);
                DataGridPlayer.ItemsSource = tournamentController.ActiveTournament.Participants;
                RefreshDataGridPlayer(tournamentController.ActiveTournament.Participants);
            }
        }

        private void TextBoxTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxTime.Text = tournamentController.SetTimer(TextBoxTime.Text);
        }
    }
}
