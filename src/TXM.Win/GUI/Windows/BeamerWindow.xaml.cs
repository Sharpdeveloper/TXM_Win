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

        public BeamerWindow(System.Collections.IEnumerable data, string title, bool table, Language lang)
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
                dgc.Header = lang.GetTranslation(StaticLanguage.No);
                dgc.Binding = new Binding("Nr");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Forename);
                dgc.Binding = new Binding("Forename");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Nickname);
                dgc.Binding = new Binding("Nickname");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = lang.GetTranslation(StaticLanguage.WonByeShort);
                dgcb.Binding = new Binding("WonFreeticket");
                dgcb.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgcb);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = lang.GetTranslation(StaticLanguage.PaidShort);
                dgcb.Binding = new Binding("Payed");
                dgcb.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgcb);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = lang.GetTranslation(StaticLanguage.ListGivenShort);
                dgcb.Binding = new Binding("SquadListGiven");
                dgcb.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgcb);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Team);
                dgc.Binding = new Binding("Team");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Faction);
                dgc.Binding = new Binding("PlayersFactionAsString");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.ListPointsShort);
                dgc.Binding = new Binding("PointOfSquad");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Points);
                dgc.Binding = new Binding("Points");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.WinsShort);
                dgc.Binding = new Binding("Wins");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.ModifiedWinsShort);
                dgc.Binding = new Binding("ModifiedWins");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.DrawShort);
                dgc.Binding = new Binding("Draws");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.LoosesShort);
                dgc.Binding = new Binding("Looses");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.MarginOfVictoryShort);
                dgc.Binding = new Binding("MarginOfVictory");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.StrengthOfSceduleShort);
                dgc.Binding = new Binding("PointsOfEnemies");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
            }
            else
            {
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.TableNr);
                dgc.Binding = new Binding("TableNr");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Player) + " 1";
                dgc.Binding = new Binding("Player1Name");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Player) + " 2";
                dgc.Binding = new Binding("Player2Name");
                dgc.IsReadOnly = true;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Destroyed) + " (" + lang.GetTranslation(StaticLanguage.Player) +" 1)";
                dgc.Binding = new Binding("Player1Score");
                dgc.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgc);
                dgc = new DataGridTextColumn();
                dgc.Header = lang.GetTranslation(StaticLanguage.Destroyed) + " (" + lang.GetTranslation(StaticLanguage.Player) + " 2)";
                dgc.Binding = new Binding("Player2Score");
                dgc.IsReadOnly = false;
                DataGridOutput.Columns.Add(dgc);
                dgcb = new DataGridCheckBoxColumn();
                dgcb.Header = lang.GetTranslation(StaticLanguage.OK) + "?";
                Binding b = new Binding("ResultEdited");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                dgcb.Binding = b;
                DataGridOutput.Columns.Add(dgcb);
            }

            Title = title;
            DataGridOutput.ItemsSource = data;
        }
    }
}
