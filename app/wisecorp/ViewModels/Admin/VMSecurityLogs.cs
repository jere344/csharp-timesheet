using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace wisecorp.ViewModels;

public partial class VMSecurityLogs : ObservableObject
{
    private readonly WisecorpContext context;

    private ObservableCollection<SecurityLog> _securityLogs = new();
    public ObservableCollection<SecurityLog> SecurityLogs
    {
        get => _securityLogs;
        set => SetProperty(ref _securityLogs, value);
    }

    private int? _filterAccountId;
    public string? FilterAccountId
    {
        get => _filterAccountId.ToString();
        set
        {
            int? newValue;
            if (string.IsNullOrWhiteSpace(value))
            {
                newValue = null;
            }
            else {
                newValue = int.TryParse(value, out var result) ? result : null;
                if (newValue == null) return;
            }
            if (_filterAccountId != newValue)
            {
                _filterAccountId = newValue;
                OnPropertyChanged();
                ResetPageAndReload();
            }
        }
    }

    private DateTime? _filterDate;
    public DateTime? FilterDate
    {
        get => _filterDate;
        set
        {
            if (_filterDate != value)
            {
                _filterDate = value;
                OnPropertyChanged();
                ResetPageAndReload();
            }
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private int _currentPage = 1;
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            var newPage = value < 1 ? 1 : value;
            if (_currentPage != newPage)
            {
                _currentPage = newPage;
                OnPropertyChanged();
                LoadSecurityLogs();
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CanGoToNextPage));
                OnPropertyChanged(nameof(CanGoToPreviousPage));
            }
        }
    }

    private int _itemsPerPage = 10;
    public int ItemsPerPage
    {
        get => _itemsPerPage;
        set
        {
            var newItemsPerPage = value < 1 ? 1 : value;
            if (_itemsPerPage != newItemsPerPage)
            {
                _itemsPerPage = newItemsPerPage;
                OnPropertyChanged();
                LoadSecurityLogs();
            }
        }
    }

    private string _filterCode = string.Empty;
    public string FilterCode
    {
        get => _filterCode;
        set
        {
            if (_filterCode != value)
            {
                _filterCode = value;
                OnPropertyChanged();
                ResetPageAndReload();
            }
        }
    }

    private string _filterIp = string.Empty;
    public string FilterIp
    {
        get => _filterIp;
        set
        {
            if (_filterIp != value)
            {
                _filterIp = value;
                OnPropertyChanged();
                ResetPageAndReload();
            }
        }
    }

    private int TotalItems { get; set; }
    private int TotalPages => (int)Math.Ceiling(TotalItems / (double)ItemsPerPage);
    public bool CanGoToNextPage => CurrentPage < TotalPages;
    public bool CanGoToPreviousPage => CurrentPage > 1;

    public ICommand NextPageCommand => new RelayCommand(
        () => CurrentPage++);

    public ICommand PreviousPageCommand => new RelayCommand(
        () => CurrentPage--);

    // Add a delay for a feedback since it's so damn fast
    public ICommand RefreshCommand => new RelayCommand(() =>
    {
        SecurityLogs.Clear();
        Task.Delay(100).ContinueWith(_ => LoadSecurityLogs());
    });

    public ICommand ClearFiltersCommand => new RelayCommand(ClearFilters);

    public VMSecurityLogs()
    {
        if (App.Current.ConnectedAccount == null || App.Current.ConnectedAccount.Role.Name != "Admin")
        {
            throw new UnauthorizedAccessException("Vous n'avez pas les droits pour acceder a cette page.");
        }

        context = new WisecorpContext();
        LoadSecurityLogs();
    }

    public Account? GetAccountById(int accountId)
    {
        return context.Accounts.FirstOrDefault(a => a.Id == accountId);
    }

    /// <summary>
    /// Charge les logs de sécurité en appliquant les filtres et la pagination
    /// </summary>
    private void LoadSecurityLogs()
    {
        // Base query
        var query = context.SecurityLogs.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(FilterCode))
            query = query.Where(log => log.Code.Contains(FilterCode));
        if (!string.IsNullOrEmpty(FilterIp))
            query = query.Where(log => log.Ip.Contains(FilterIp));
        if (FilterDate.HasValue)
            query = query.Where(log => log.Date.Date == FilterDate.Value.Date);
        if (_filterAccountId != null) {
            if (_filterAccountId == 0)
                query = query.Where(log => log.AccountId == null);
            else
                query = query.Where(log => log.AccountId == _filterAccountId);
        }

        // Fetch total items for paging
        TotalItems = query.Count();

        // Adjust current page if out of bounds
        if (CurrentPage > TotalPages)
            CurrentPage = TotalPages;

        // Fetch paged results
        var logs = query
            .OrderByDescending(log => log.Date)
            .Skip((CurrentPage - 1) * ItemsPerPage)
            .Take(ItemsPerPage)
            .ToList();

        SecurityLogs = new ObservableCollection<SecurityLog>(logs);

        // Notify pagination properties
        OnPropertyChanged(nameof(CanGoToNextPage));
        OnPropertyChanged(nameof(CanGoToPreviousPage));

        IsLoading = false;
    }

    /// <summary>
    /// Met à jour le nombre d'éléments par page en fonction de la hauteur de la fenêtre
    /// </summary>
    /// <param name="windowHeight">La hauteur actuelle de la fenêtre</param>
    public void UpdateItemsPerPage(int windowHeight)
    {
        int newItemsPerPage = (windowHeight - 170) / 33; // Adjust these values as needed
        ItemsPerPage = newItemsPerPage > 0 ? newItemsPerPage : 1;
    }

    /// <summary>
    /// Réinitialise la page courante à 1 et recharge les logs de sécurité
    /// </summary>
    private void ResetPageAndReload()
    {
        CurrentPage = 1; // Reset page to first
        LoadSecurityLogs();
    }

    /// <summary>
    /// Efface tous les filtres appliqués aux logs de sécurité
    /// </summary>
    private void ClearFilters()
    {
        FilterCode = string.Empty;
        FilterIp = string.Empty;
        FilterAccountId = null;
        FilterDate = null;
    }
}
