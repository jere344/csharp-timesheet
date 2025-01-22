using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using wisecorp.Context;
using wisecorp.Models.DBModels;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Util;

namespace wisecorp.ViewModels
{
    public partial class VMAddHour : ObservableObject
    {
        private readonly Work Work;
        private readonly int Day;
        
        [ObservableProperty]
        string? comment;
        [ObservableProperty]
        decimal? hour;
        
        public bool Saving { get; set; } = false;


        public VMAddHour(Work work, int day)
        {
            Work = work;
            Day = day;

            switch (day)
            {
                case 1:
                    hour = work.HourWorkedSun;
                    comment = work.CommentSun;
                    break;
                case 2:
                    hour = work.HourWorkedMon;
                    comment = work.CommentMon;

                    break;
                case 3:
                     hour = work.HourWorkedTue;
                     comment = work.CommentTue;

                    break;
                case 4:
                    hour = work.HourWorkedWed;
                    comment = work.CommentWed;

                    break;
                case 5:
                    hour = work.HourWorkedThur;
                    comment = work.Commenthur;

                    break;
                case 6:
                    hour = work.HourWorkedFri;
                    comment = work.CommentFri;

                    break;
                case 7:
                    hour = work.HourWorkedSat;
                    comment = work.CommentSat;
                    break;
            }

        }

        /// <summary>
        /// Sauvegarde les heures travaillées et les commentaires pour le jour spécifié dans l'objet Work
        /// </summary>
        public void Save()
        {
            Saving = true;
            switch (Day)
            {
                case 1:
                    Work.HourWorkedSun = hour;
                    Work.CommentSun = comment;
                    break;
                case 2:
                    Work.HourWorkedMon = hour;
                    Work.CommentMon = comment;
                    break;
                case 3:
                    Work.HourWorkedTue = hour;
                    Work.CommentTue = comment;
                    break;
                case 4:
                    Work.HourWorkedWed = hour;
                    Work.CommentWed = comment;
                    break;
                case 5:
                    Work.HourWorkedThur = hour;
                    Work.Commenthur = comment;
                    break;
                case 6:
                    Work.HourWorkedFri = hour;
                    Work.CommentFri = comment;
                    break;
                case 7:
                    Work.HourWorkedSat = hour;
                    Work.CommentSat = comment;
                    break;
            }
        }

    }
}
