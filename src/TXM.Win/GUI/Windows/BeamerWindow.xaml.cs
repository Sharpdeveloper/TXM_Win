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

namespace TXM.GUI.Windows
{
    /// <summary>
    /// Interaction logic for BeamerWindow.xaml
    /// </summary>
    public partial class BeamerWindow : Window
    {
        //private ICollectionView dataView;

        public BeamerWindow(System.Collections.IEnumerable data, string title, bool table)
        {
            InitializeComponent();

            DataGridTextColumn dgc;
            DataGridCheckBoxColumn dgcb;
            
            if (table)
            {
                dgc = new DataGridTextColumn();
                dgc.Header = "#";
                dgc.Binding = new Binding("Rank");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Forename";
                dgc.Binding = new Binding("Forename");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Nickname";
                dgc.Binding = new Binding("Nickname");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = "$";
                dgcb.Binding = new Binding("Payed");
                dgcb.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgcb);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = "L";
                dgcb.Binding = new Binding("SquadListGiven");
                dgcb.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgcb);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = "!";
                dgcb.Binding = new Binding("Present");
                dgcb.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgcb);
                dgc = new DataGridTextColumn();
                dgc.Header = "Team";
                dgc.Binding = new Binding("Team");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Faction";
                dgc.Binding = new Binding("PlayersFactionAsString");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "TP";
                dgc.Binding = new Binding("Points");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "W";
                dgc.Binding = new Binding("Wins");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "L";
                dgc.Binding = new Binding("Looses");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "MoV";
                dgc.Binding = new Binding("MarginOfVictory");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "SoS";
                dgc.Binding = new Binding("PointsOfEnemies");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
            }
            else
            {
                dgc = new DataGridTextColumn();
                dgc.Header = "Table-No.";
                dgc.Binding = new Binding("TableNr");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Player 1";
                dgc.Binding = new Binding("Player1Name");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Player 2";
                dgc.Binding = new Binding("Player2Name");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Destroyed (Player 1)";
                dgc.Binding = new Binding("Player1Score");
                dgc.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Destroyed (Player 2)";
                dgc.Binding = new Binding("Player2Score");
                dgc.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = "Winner";
                dgc.Binding = new Binding("Winner");
                dgc.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgc);
            }

            Title = title;
            DataGridOutput.ItemsSource = data;
        }
    }
}
