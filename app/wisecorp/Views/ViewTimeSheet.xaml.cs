using System.Windows;
using System.Windows.Controls;
using System;
using wisecorp.ViewModels;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using wisecorp.Models.DBModels;

namespace wisecorp.Views;

public partial class ViewTimeSheet : Page
{
    public ViewTimeSheet()
    {
        InitializeComponent();
        this.DataContext = new VMTimeSheet();

        this.Loaded += Page_Loaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos("WiseCorp - Time Sheet", 700, 1000);
    }

     private void OpenHourWindow(object sender, RoutedEventArgs e)
     {
        Button button = (Button)sender;
        int day = int.Parse(button.Tag.ToString() ?? throw new InvalidOperationException("No day tag on button"));
        Work work = (Work)button.DataContext;

        VMTimeSheet vm = (VMTimeSheet)this.DataContext;

        vm.OpenHourWindow(work, day); 
     }

     private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is System.Windows.Controls.Calendar)
        {
            // Release mouse capture to allow other controls to be clicked immediately
            Mouse.Capture(null);
        }
    }

}
