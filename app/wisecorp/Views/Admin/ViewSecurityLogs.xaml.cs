using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using wisecorp.Models.DBModels;
using wisecorp.ViewModels;

namespace wisecorp.Views;

public partial class ViewSecurityLogs : Page
{
    private DispatcherTimer _resizeTimer;

    public ViewSecurityLogs()
    {
        InitializeComponent();
        
        DataContext = new VMSecurityLogs();

        this.Loaded += Page_Loaded;
        this.SizeChanged += Page_SizeChanged;

        _resizeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300)
        };
        _resizeTimer.Tick += ResizeTimer_Tick;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos("WiseCorp - Admin", 600, 900);
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _resizeTimer.Stop();
        _resizeTimer.Tag = (int)e.NewSize.Height;
        _resizeTimer.Start();
    }

    private void ResizeTimer_Tick(object? sender, EventArgs e)
    {
        _resizeTimer.Stop();
        var vm = DataContext as VMSecurityLogs;
        if (vm != null)
        {
            vm.UpdateItemsPerPage((int)_resizeTimer.Tag);
        }
    }

    private void SecurityLogsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is SecurityLog selectedLog)
        {
            if (dataGrid.CurrentColumn.Header.ToString() == "Account ID" && selectedLog.AccountId.HasValue)
            {
                ShowAccountDetails(selectedLog.AccountId.Value);
            }
            else
            {
                ShowCustomMessageBox(selectedLog.Description, "Log Description");
            }
        }
    }

    private void ShowAccountDetails(int accountId)
    {
        var account = ((VMSecurityLogs)DataContext).GetAccountById(accountId);
        if (account != null)
        {
            var textBox = new TextBox
            {
                Text = $"Full Name: {account.FullName}\nEmail: {account.Email}\nPhone: {account.Phone}\nRole: {account.Role.Name}",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20),
                IsReadOnly = true,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var window = new Window
            {
                Title = "Account Details",
                Content = textBox,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }
    }

    private void ShowCustomMessageBox(string message, string title)
    {
        var textBox = new TextBox
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(20),
            IsReadOnly = true,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        var window = new Window
        {
            Title = title,
            Content = textBox,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        window.ShowDialog();
    }
}
