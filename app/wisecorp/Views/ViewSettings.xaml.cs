using System.Windows;
using System.Windows.Controls;
using System;
using wisecorp.ViewModels;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;

namespace wisecorp.Views;

public partial class ViewSettings : Page
{
    public ViewSettings()
    {
        InitializeComponent();

        DataContext = new VMSettings();

        this.Loaded += Page_Loaded;
    }

    /// <summary>
    /// Set the title and size of the window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos((string)Application.Current.FindResource("settings"), 400, 600);
    }
}