namespace wisecorp;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;


public class SimpleNavigationService
{
    private static SimpleNavigationService _instance;
    public static SimpleNavigationService Instance => _instance ??= new SimpleNavigationService();

    public object? Parameter { get; set; }
}


public class NavigationController : INotifyPropertyChanged
{
    private readonly Stack<Uri> _backStack = new();
    private readonly Stack<Uri> _forwardStack = new();
    protected NavigationService NavigationService { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;


    public NavigationController(NavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public NavigationService GetNavigationService()
    {
        return NavigationService;
    }

    public bool HasPermission(string uri)
    {
        string permission = App.Current.ConnectedAccount?.Role?.Name ?? "Guest";
        return !Routes.ViewInfos.ContainsKey(uri) || (int)Routes.ViewInfos[uri]["Permission"] <= Routes.PermissionsLevels[permission];
    }

    public void ClearHistory()
    {
        _backStack.Clear();
        _forwardStack.Clear();

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }


    // Navigate to a view with permission checks and stack management
    public void _NavigateTo(string uri, object? parameter = null)
    {
        // check if the current user has the permission to access the view
        if (!HasPermission(uri))
        {
            MessageBox.Show((string)Application.Current.FindResource("no_permission"), (string)Application.Current.FindResource("error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var currentUri = GetCurrentViewUri();
        if (currentUri != null)
        {
            // we do not want the same page multiple times next to each other
            // if the current page is the same as the target page, do not add it to the stack
            if (currentUri.ToString() != uri)
            {
                _backStack.Push(currentUri);
            }
        }
        _forwardStack.Clear();

        SimpleNavigationService.Instance.Parameter = parameter;
        
        NavigationService.Navigate(new Uri(uri, UriKind.Relative));

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    public void NavigateBack()
    {
        if (CanGoBack)
        {
            // Move the current page to the forward stack
            var currentUri = GetCurrentViewUri();
            if (currentUri != null)
            {
                _forwardStack.Push(currentUri);
            }

            // Navigate to the previous page from the back stack
            var previousUri = _backStack.Pop();
            NavigationService.Navigate(previousUri);

            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));
        }
    }

    // Go forward in the navigation history
    public void NavigateForward()
    {
        if (CanGoForward)
        {
            // Move the current page to the back stack
            var currentUri = GetCurrentViewUri();
            if (currentUri != null)
            {
                _backStack.Push(currentUri);
            }

            // Navigate to the next page from the forward stack
            var nextUri = _forwardStack.Pop();
            NavigationService.Navigate(nextUri);

            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));
        }
    }

    public void NavigateHome()
    {
        string permission = App.Current.ConnectedAccount?.Role?.Name ?? "Guest";
        _NavigateTo(Routes.HomeViews[permission]);

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    // Utility method to get the current view's URI
    private Uri? GetCurrentViewUri()
    {
        var content = NavigationService.Content as Page;
        return content?.NavigationService?.CurrentSource;
    }

    public bool CanGoBack => _backStack.Count > 0;

    public bool CanGoForward => _forwardStack.Count > 0;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
