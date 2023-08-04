using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TXM.Core.Models;

namespace TXM.Win
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
