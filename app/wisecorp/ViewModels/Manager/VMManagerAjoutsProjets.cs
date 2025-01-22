using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wisecorp.Context;
using wisecorp.Helpers;
using wisecorp.Models.DBModels;

namespace wisecorp.ViewModels.Manager
{
    public partial class VMManagerAjoutsProjets : ObservableObject
    {
        private WisecorpContext context;
        [ObservableProperty]
        string errorMessage;

        [ObservableProperty]
        bool tache = false;

        [ObservableProperty]
        string nom;

        [ObservableProperty]
        Project? projetParents;

        [ObservableProperty]
        string description;

        [ObservableProperty]
        DateTime startTime = DateTime.Now.Date;

        [ObservableProperty]
        DateTime endDate = DateTime.Now.Date;

        [ObservableProperty]
        double budget;

        [ObservableProperty]
        int nbhours;

        [ObservableProperty]
        ObservableCollection<Project> lesProjets;

        [ObservableProperty]
        Project projetSelect;

        public VMManagerAjoutsProjets()
        {
            context = new WisecorpContext();
            _ = InitialiseAsync();
        }

        private async Task InitialiseAsync()
        {
            await GetAllActiveProjects();
        }

        private async Task GetAllActiveProjects()
        {
            int id = App.Current.ConnectedAccount.Id;
            List<Project> lesprojs = await context.Projects.Where(p => p.IsActive == true).Where(i => i.CreatorId == id).ToListAsync();
            lesProjets = new ObservableCollection<Project>(lesprojs);
        }

        [RelayCommand]
        public async Task SaveProject()
        {

            //Série de if qui viennent indiquer a l'utilisateur si le formulaire est bien remplis
            if (nom == null) { errorMessage = "Le Nom ne peut pas être vide."; }
            if (budget == null || budget <= 0) { errorMessage = "Le Budget ne peut pas être vide."; }
            if (description == null) { errorMessage = "La description ne peut pas être vide."; }
            if (Nbhours == null || nbhours <= 0) { errorMessage = "Le nombre d'heure ne peut pas être vide."; }
            if (startTime.Date <= endDate.Date) { errorMessage = "La date de début doit etre antérieur a la date de fin"; }

            //Va chercher le compte qui a le meme courriel si y'en a un qui existe
            Project? p = await context.Projects.Where(p => p.Name == nom).FirstOrDefaultAsync();

            //Viens mettre le message a null si tout est valide
            if (Nom != null &&
                budget > 0 &&
                description != null &&
                Nbhours > 0)
            {
                errorMessage = String.Empty;
            }

            //Si error message est null ou empty effectue la sauvegarde
            if (String.IsNullOrEmpty(errorMessage))
            {
                //Check que le compte qui a soit disant le meme courriel est null
                if (p == null)
                {
                    Project leProj = null;

                    if (!tache)
                    {
                        //Sauvegarde seulement les informations importante a l'administration
                        leProj = new Project
                        {
                            Name = nom,
                            NbHour = nbhours,
                            Description = description,
                            Budget = budget,
                            StartDate = startTime,
                            EndDate = endDate,
                            IsActive = true,
                            CreatorId = App.Current.ConnectedAccount.Id,
                        };

                        await context.Projects.AddAsync(leProj);
                        await context.SaveChangesAsync();
                        RedirectToList();
                    }
                    else if (projetSelect != null)
                    {
                        //Sauvegarde seulement les informations importante a l'administration
                        leProj = new Project
                        {
                            Name = nom,
                            NbHour = nbhours,
                            Description = description,
                            Budget = budget,
                            StartDate = startTime,
                            EndDate = endDate,
                            IsActive = true,
                            CreatorId = App.Current.ConnectedAccount.Id,
                            ParentProjectId = projetSelect.Id,
                        };
                        await context.Projects.AddAsync(leProj);
                        await context.SaveChangesAsync();
                        RedirectToList();

                    }
                    else
                        errorMessage = "Aucun projet parents est selectionné";




                }
                else
                    errorMessage = "Un projets du même nom est deja présent dans la base de donnée";

            }
            OnPropertyChanged(nameof(ErrorMessage));

        }

        /// <summary>
        /// Redirection vers la liste des comptes présent dans la base de donnée
        /// </summary>
        protected virtual void RedirectToList()
        {
            var mainWindow = (MainWindow)App.Current.MainWindow;
            mainWindow.NavigateTo("Views/Manager/ViewAssignProjects.xaml");
        }
    }
}
