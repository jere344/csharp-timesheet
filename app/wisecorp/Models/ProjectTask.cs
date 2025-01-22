using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using wisecorp.Context;
using wisecorp.Models.DBModels;

namespace wisecorp.Models
{
    public class ProjectTask : ObservableObject
    {
        private Project _mainProject;
        public Project MainProject
        {
            get => _mainProject;
            set => SetProperty(ref _mainProject, value);
        }

        private ObservableCollection<Project> _tasks;
        public ObservableCollection<Project> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        private ObservableCollection<Work> _works;
        public ObservableCollection<Work> Works
        {
            get => _works;
            set => SetProperty(ref _works, value);
        }


        public ProjectTask(Project project)
        {
            MainProject = project;
            Tasks = new ObservableCollection<Project>();
            Works = new ObservableCollection<Work>();
        }

        /// <summary>
        /// Récupère récursivement les tâches (projets actifs sans sous-projets actifs) à partir d'un projet donné
        /// </summary>
        /// <param name="project">Le projet à partir duquel récupérer les tâches</param>
        public void FetchTasks(Project project)
        {
            if (project == null) return;
            if (project.IsActive == false) return;

            if (project.SubProjects != null && !project.SubProjects.Any(p => p.IsActive == true))
            {
                Tasks.Add(project);
            }
            else
            {
                foreach (Project subProject in project.SubProjects ?? new List<Project>())
                {
                    FetchTasks(subProject);
                }
            }
        }

        /// <summary>
        /// Permet de remplir la liste de work
        /// </summary>
        /// <returns></returns>
        public void FetchWorks(WisecorpContext context, Account account, DateTime currentWeek)
        {
            foreach (Project task in Tasks)
            {
                Work? work = context.Works.Include(w=> w.Project).Where(w => w.AccountId == account.Id && w.ProjectId == task.Id && w.WeekStartDate == currentWeek).FirstOrDefault();
                if (work == null)
                {
                    work = new()
                    {
                        ProjectId = task.Id,
                        AccountId = account.Id,
                        WeekStartDate = currentWeek
                    };
                    context.Works.Add(work);
                    context.SaveChanges();
                    work = context.Works.Include(w=> w.Project).Where(w => w.AccountId == account.Id && w.ProjectId == task.Id && w.WeekStartDate == currentWeek).First();
                }
                RoundHourWorked(work);
                Works.Add(work);
            }
        }

        /// <summary>
        /// Arrondit les heures travaillées pour chaque jour de la semaine à deux décimales
        /// </summary>
        /// <param name="work">L'objet Work dont les heures doivent être arrondies</param>
        private static void RoundHourWorked(Work work)
        {
            work.HourWorkedSun = Math.Round(work.HourWorkedSun ?? 0, 2);
            work.HourWorkedMon = Math.Round(work.HourWorkedMon ?? 0, 2);
            work.HourWorkedTue = Math.Round(work.HourWorkedTue ?? 0, 2);
            work.HourWorkedWed = Math.Round(work.HourWorkedWed ?? 0, 2);
            work.HourWorkedThur = Math.Round(work.HourWorkedThur ?? 0, 2);
            work.HourWorkedFri = Math.Round(work.HourWorkedFri ?? 0, 2);
            work.HourWorkedSat = Math.Round(work.HourWorkedSat ?? 0, 2);
        }

        /// <summary>
        /// Rafraîchit la collection observable des travaux
        /// </summary>
        public void RefreshWorks()
        {
            Works = new ObservableCollection<Work>(Works);
        }
    }
}
