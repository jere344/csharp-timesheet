using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using wisecorp.Views;

namespace wisecorp.ViewModels;

public class VMApproveTS : ObservableObject
{
    private readonly WisecorpContext context;
    public ICommand ApproveTimeSheetCommand { get; }
    public ICommand RefuseTimeSheetCommand { get; }

    public VMApproveTS()
    {
        context = new WisecorpContext();
        FetchWorksToApprove();
        ApproveTimeSheetCommand = new RelayCommand(ApproveTimeSheet);
        RefuseTimeSheetCommand = new RelayCommand(RefuseTimeSheet);
    }

    private ObservableCollection<ObservableCollection<Work>> _timeSheets = new ObservableCollection<ObservableCollection<Work>>();
    public ObservableCollection<ObservableCollection<Work>> TimeSheets
    {
        get => _timeSheets;
        set => SetProperty(ref _timeSheets, value);
    }

    private ObservableCollection<Work> _selectedTimeSheet = new();
    public ObservableCollection<Work> SelectedTimeSheet
    {
        get => _selectedTimeSheet;
        set => SetProperty(ref _selectedTimeSheet, value);
    }

    /// <summary>
    /// Récupère les feuilles de temps soumises mais non encore approuvées
    /// </summary>
    public async void FetchWorksToApprove()
    {
        var works = await context.Works.Where(w => w.IsSubmitted && !w.IsApproved).ToListAsync();
        // one time sheet is all the works with the same week start date and the same account id
        TimeSheets = new(works.GroupBy(w => new { w.WeekStartDate, w.AccountId }).Select(g => new ObservableCollection<Work>(g.ToList())).ToList());
    }

    /// <summary>
    /// Approuve la feuille de temps sélectionnée
    /// </summary>
    private async void ApproveTimeSheet()
    {
        foreach (var work in SelectedTimeSheet)
        {
            work.IsApproved = true;
        }
        await context.SaveChangesAsync();
        // remove the approved time sheet from the list
        TimeSheets.Remove(SelectedTimeSheet);
        SelectedTimeSheet = null;
    }

    /// <summary>
    /// Refuse la feuille de temps sélectionnée en demandant une raison
    /// </summary>
    private async void RefuseTimeSheet()
    {
        var reasonWindow = new ReasonWindow();
        if (reasonWindow.ShowDialog() == true)
        {
            string reason = reasonWindow.Reason;
            foreach (var work in SelectedTimeSheet)
            {
                work.IsRejected = true;
                work.IsSubmitted = false;
                work.RejectedReason = reason;
            }
            await context.SaveChangesAsync();
            // remove the refused time sheet from the list
            TimeSheets.Remove(SelectedTimeSheet);
            SelectedTimeSheet = null;
        }
    }
}
