using System.Windows;
using System.Windows.Controls;
using System;
using wisecorp.ViewModels;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;

namespace wisecorp.Views;

public partial class ViewLogin : Page
{
    public ViewLogin()
    {
        InitializeComponent();
        this.DataContext = new VMLogin();

        this.Loaded += Page_Loaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos("WiseCorp - Login", 600, 600);
    }
}
