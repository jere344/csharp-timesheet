using System.Windows;
using System.Windows.Controls;
using System;
using wisecorp.ViewModels;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;
using wisecorp.Models.DBModels;

namespace wisecorp.Views;

public partial class ViewApproveTS : Page
{
    public ViewApproveTS()
    {
        InitializeComponent();

        DataContext = new VMApproveTS();

        this.Loaded += Page_Loaded;

        // Attach the event handler to the DataGrid
        DataGrid1.MouseDoubleClick += DataGrid_CellMouseDoubleClick;
        DataGrid2.MouseDoubleClick += DataGrid_CellMouseDoubleClick;
    }

    /// <summary>
    /// Set the title and size of the window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos((string)Application.Current.FindResource("approveTimeSheet"), 660, 900);
    }

    private void DataGrid_CellMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Work selectedWork)
        {
            var column = dataGrid.CurrentColumn as DataGridTextColumn;
            if (column != null)
            {
                string comment = column.Header switch
                {
                    "Sunday Hours" => selectedWork.CommentSun,
                    "Monday Hours" => selectedWork.CommentMon,
                    "Tuesday Hours" => selectedWork.CommentTue,
                    "Wednesday Hours" => selectedWork.CommentWed,
                    "Thursday Hours" => selectedWork.Commenthur,
                    "Friday Hours" => selectedWork.CommentFri,
                    "Saturday Hours" => selectedWork.CommentSat,
                    _ => null
                };

                if (!string.IsNullOrEmpty(comment))
                {
                    MessageBox.Show(comment, "Comment", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}