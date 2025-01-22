﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;
using System.Windows.Media;
using System.Diagnostics;
using MaterialDesignThemes.Wpf;



namespace wisecorp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NavigationController NavigationController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = NavigationController;
            MainNavigationFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            NavigationController = new NavigationController(MainNavigationFrame.NavigationService);
            NavigationController.NavigateHome();
            FillLeftDrawer();

            // Every time the NavigationController changes, update the buttons (because automatic binding from XAML is a PAIN without DataContext)
            NavigationController.PropertyChanged += NavigationController_PropertyChanged;
            UpdateNavigationButtons();
        }

        private void NavigationController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NavigationController.CanGoBack) || e.PropertyName == nameof(NavigationController.CanGoForward))
            {
                UpdateNavigationButtons();
            }
        }

        private void UpdateNavigationButtons()
        {
            btnBack.IsEnabled = NavigationController.CanGoBack;
            btnNext.IsEnabled = NavigationController.CanGoForward;
        }

        public void NavigateTo(string uri, object? parameter = null)
        {
            if (uri == "Views/Logout.xaml")
            {
                App.Current.ConnectedAccount = null;
                // we also need to clear remember me on logout
                App.Current.SavedSettings["SessionToken"] = null;
                NavigateTo(Routes.HomeViews["Guest"]);

                NavigationController.ClearHistory();
                return;
            }
            NavigationController._NavigateTo(uri, parameter);
            DrawerHost.CloseDrawerCommand.Execute(Dock.Left, null);
        }

        public void ChangeAccount()
        {
            FillLeftDrawer();
            NavigationController.ClearHistory();
        }

        public void FillLeftDrawer()
        {
            // Clear the stack panel to avoid duplication on multiple calls
            // LeftDrawerStackPanel.Children.Clear();
            LeftDrawerStackPanel.Items.Clear();

            // Get the permission level of the connected user or default to "Guest"
            string permission = App.Current.ConnectedAccount?.Role?.Name ?? "Guest";

            // Loop through all views and add buttons for those the user has access to
            foreach (var view in Routes.ViewInfos)
            {
                if (view.Value.TryGetValue("Hidden", out var hiddenValue) && (bool)hiddenValue == true)
                    continue;

                if (NavigationController.HasPermission(view.Key))
                {
                    var icon = new PackIcon
                    {
                        Kind = view.Value.TryGetValue("Icon", out var iconValue) ? (PackIconKind)iconValue : PackIconKind.None,
                        Foreground = App.Current.Resources["MaterialDesignBody"] as Brush,
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(0, 0, 5, 0)
                    };

                    var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            icon,
                            new TextBlock
                            {
                                Text = view.Value["Title"].ToString(),
                                Foreground = App.Current.Resources["MaterialDesignBody"] as Brush,
                                Width = 150,
                            },
                        }
                    };

                    var button = new Button
                    {
                        Content = stackPanel,
                        Margin = new Thickness(0),
                        Padding = new Thickness(10),
                        Height = 40,
                        Style = Application.Current.FindResource("MaterialDesignFlatButton") as Style 
                    };

                    // Optional: Add a click event to handle navigation or actions
                    button.Click += (s, e) => NavigateTo(view.Key);

                    // Add the button to the stack panel
                    // LeftDrawerStackPanel.Children.Add(button);
                    // LeftDrawerStackPanel is now a ListView, so we need to add items to its Items property
                    LeftDrawerStackPanel.Items.Add(button);
                }
            }
        }

        // Clicking on the logo will open the GitHub page
        private void OpenGitHub(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/jere344/wisecorp-timesheet",
                UseShellExecute = true
            });
        }

        private void Next_Btn(object sender, RoutedEventArgs e)
        {
            if (this.NavigationController.CanGoForward)
                NavigationController.NavigateForward();
        }

        private void Back_Btn(object sender, RoutedEventArgs e)
        {
            if (this.NavigationController.CanGoBack)
                NavigationController.NavigateBack();
        }

        private void Home_Btn(object sender, RoutedEventArgs e)
        {
            NavigationController.NavigateHome();
        }

        private void Open_Settings(object sender, RoutedEventArgs e)
        {
            NavigateTo("Views/ViewSettings.xaml");
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            // check if the user is connected
            if (App.Current.ConnectedAccount == null)
            {
                MessageBox.Show((string)Application.Current.FindResource("must_be_connected"), (string)Application.Current.FindResource("error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NavigateTo("Views/ViewProfile.xaml");
        }

        private void Logout(object sender,RoutedEventArgs e)
        {
            NavigateTo("Views/Logout.xaml");
        }

        public void SetWindowInfos(string title, double minHeight = 0, double minWidth = 0, double maxHeight = double.PositiveInfinity, double maxWidth = double.PositiveInfinity)
        {
            Window window = GetWindow(this);
            if (window != null)
            {
                window.Title = title;
                window.MinHeight = minHeight;
                window.MinWidth = minWidth;
                window.MaxHeight = maxHeight;
                window.MaxWidth = maxWidth;
            }
        }
    }
}