using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class StatisticsWindow : Window
    {
        private Statistic stats;
        private ICollectionView dataViewPilots;
        private ICollectionView dataViewShips;
        private ICollectionView dataViewUpgrades;
        private IO io;
        private Language lang;
        private bool english;

        public StatisticsWindow(IO _io, Language _lang, bool _english)
        {
            InitializeComponent();

            io = _io;
            english = _english;
            english = false;
            lang = _lang;

            MenuItemFile.Header = lang.GetTranslation(StaticLanguage.HeaderFile);
            MenuItemSave.Header = lang.GetTranslation(StaticLanguage.Save);
            MenuItemLoad.Header = lang.GetTranslation(StaticLanguage.Load);
            MenuItemImport.Header = lang.GetTranslation(StaticLanguage.Import);
            MenuItemImportTXT.Header = lang.GetTranslation(StaticLanguage.TXTFile);
            MenuItemImportTXTOverview.Header = lang.GetTranslation(StaticLanguage.TXTFileOverview);
            MenuItemExport.Header = lang.GetTranslation(StaticLanguage.Export);
            MenuItemExportCSV.Header = lang.GetTranslation(StaticLanguage.CSVFile);
            MenuItemExit.Header = lang.GetTranslation(StaticLanguage.Exit);
            MenuItemUpdateData.Header = lang.GetTranslation(StaticLanguage.UpdateData);
            Title = lang.GetTranslation(StaticLanguage.Statistic);

            stats = new Statistic();

            DataGridPilots.ItemsSource = stats.IPilots;
            DataGridPilots.CanUserSortColumns = false;
            DataGridPilots.SelectionMode = DataGridSelectionMode.Single;

            DataGridShips.ItemsSource = stats.IShips;
            DataGridShips.CanUserSortColumns = false;
            DataGridShips.SelectionMode = DataGridSelectionMode.Single;

            DataGridUpgrades.ItemsSource = stats.IUpgrades;
            DataGridUpgrades.CanUserSortColumns = false;
            DataGridUpgrades.SelectionMode = DataGridSelectionMode.Single;

            DataGridTextColumn dgc;

            dgc = new DataGridTextColumn();
            dgc.Header = lang.GetTranslation(StaticLanguage.Name);
            if(english)
                dgc.Binding = new Binding("Name");
            else
                dgc.Binding = new Binding("Gername");
            dgc.IsReadOnly = true;
            DataGridPilots.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = lang.GetTranslation(StaticLanguage.Quantity);
            dgc.Binding = new Binding("Count");
            dgc.IsReadOnly = true;
            DataGridPilots.Columns.Add(dgc);

            dataViewPilots = CollectionViewSource.GetDefaultView(DataGridPilots.ItemsSource);
            dataViewPilots.SortDescriptions.Clear();

            dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            if (english)
                dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            else
                dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Gername", System.ComponentModel.ListSortDirection.Ascending));

            dgc = new DataGridTextColumn();
            dgc.Header = lang.GetTranslation(StaticLanguage.Name);
            if (english)
                dgc.Binding = new Binding("Name");
            else
                dgc.Binding = new Binding("Gername");
            dgc.IsReadOnly = true;
            DataGridShips.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = lang.GetTranslation(StaticLanguage.Quantity);
            dgc.Binding = new Binding("Count");
            dgc.IsReadOnly = true;
            DataGridShips.Columns.Add(dgc);

            dataViewShips = CollectionViewSource.GetDefaultView(DataGridShips.ItemsSource);
            dataViewShips.SortDescriptions.Clear();

            dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            if (english)
                dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            else
                dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Gername", System.ComponentModel.ListSortDirection.Ascending));

            dgc = new DataGridTextColumn();
            dgc.Header = lang.GetTranslation(StaticLanguage.Name);
            if (english)
                dgc.Binding = new Binding("Name");
            else
                dgc.Binding = new Binding("Gername");
            dgc.IsReadOnly = true;
            DataGridUpgrades.Columns.Add(dgc);
            dgc = new DataGridTextColumn();
            dgc.Header = lang.GetTranslation(StaticLanguage.Quantity);
            dgc.Binding = new Binding("Count");
            dgc.IsReadOnly = true;
            DataGridUpgrades.Columns.Add(dgc);

            dataViewUpgrades = CollectionViewSource.GetDefaultView(DataGridUpgrades.ItemsSource);
            dataViewUpgrades.SortDescriptions.Clear();

            dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            if (english)
                dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            else
                dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Gername", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void Update_Data_Click(object sender, RoutedEventArgs e)
        {
            io.LoadYASBFiles();
            io.LoadContents(true);
            stats.Reset();
            io.ShowMessage(lang.GetTranslation(StaticLanguage.AllDataLoaded));
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (stats.Ships.Count == 0)
                stats = io.LoadContents();
            if (stats == null)
                return;
            if (TextBoxURL.Text.Contains(@"geordanr.github.io/xwing"))
            {
                stats.Parse(TextBoxURL.Text);
                Refresh();
                TextBoxURL.Text = "";
            }
            else
            {
                io.ShowMessage(lang.GetTranslation(StaticLanguage.WrongListLink));
            }
        }

        private void Refresh()
        {
            DataGridPilots.ItemsSource = null;
            DataGridPilots.ItemsSource = stats.IPilots;
            dataViewPilots = CollectionViewSource.GetDefaultView(DataGridPilots.ItemsSource);
            dataViewPilots.SortDescriptions.Clear();
            dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            if(english)
                dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            else
                dataViewPilots.SortDescriptions.Add(new System.ComponentModel.SortDescription("Gername", System.ComponentModel.ListSortDirection.Ascending));

            DataGridShips.ItemsSource = null;
            DataGridShips.ItemsSource = stats.IShips;
            dataViewShips = CollectionViewSource.GetDefaultView(DataGridShips.ItemsSource);
            dataViewShips.SortDescriptions.Clear();
            dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            if (english)
                dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            else
                dataViewShips.SortDescriptions.Add(new System.ComponentModel.SortDescription("Gername", System.ComponentModel.ListSortDirection.Ascending));

            DataGridUpgrades.ItemsSource = null;
            DataGridUpgrades.ItemsSource = stats.IUpgrades;
            dataViewUpgrades = CollectionViewSource.GetDefaultView(DataGridUpgrades.ItemsSource);
            dataViewUpgrades.SortDescriptions.Clear();
            dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Count", System.ComponentModel.ListSortDirection.Descending));
            if (english)
                dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            else
                dataViewUpgrades.SortDescriptions.Add(new System.ComponentModel.SortDescription("Gername", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void Beenden(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            io.SaveStatistic(stats, true);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            stats = io.LoadStatistic(true);
            if (stats != null)
                Refresh();
        }

        private void CSV_Export_Click(object sender, RoutedEventArgs e)
        {
            io.Export(stats, true);
        }

        private void Forum_click(object sender, RoutedEventArgs e)
        {
            io.Export(stats, false);
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (stats.Ships.Count == 0)
                stats = io.LoadContents();
            if (stats == null)
                return;
            if (TextBoxURL.Text.Contains(@"geordanr.github.io/xwing"))
            {
                if (stats.IPilots.Count == 0)
                {
                    io.ShowMessage(lang.GetTranslation(StaticLanguage.NoData));
                }
                try
                {
                    stats.Parse(TextBoxURL.Text, false);
                }
                catch (ArgumentException aex)
                {
                    io.ShowMessage(lang.GetTranslation(StaticLanguage.InvalidSquad));
                }
                Refresh();
                TextBoxURL.Text = "";
            }
            else
            {
                io.ShowMessage(lang.GetTranslation(StaticLanguage.WrongListLink));
            }
        }

        private void CSV_Import_Click(object sender, RoutedEventArgs e)
        {
            stats = io.LoadContents();
            if (stats == null)
                return;
            io.LoadCSV(stats);
            Refresh();
            TextBoxURL.Text = "";
        }

        private void CSV_Import_Overview_Click(object sender, RoutedEventArgs e)
        {
            stats = io.LoadContents(false, true);
            if (stats == null)
                return;
            io.LoadCSV(stats, true);
            Refresh();
            TextBoxURL.Text = "";
        }
    }
}
