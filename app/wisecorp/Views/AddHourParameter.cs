using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wisecorp.Views
{
    public class AddHourParameter : DependencyObject
    {
        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(int), typeof(AddHourParameter), new PropertyMetadata(0));

        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(decimal), typeof(AddHourParameter), new PropertyMetadata(0m));

        public static readonly DependencyProperty WorkIdProperty =
            DependencyProperty.Register("WorkId", typeof(int), typeof(AddHourParameter), new PropertyMetadata(0));

        public int Day
        {
            get { return (int)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public decimal Hour
        {
            get { return (decimal)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }

        public int WorkId
        {
            get { return (int)GetValue(WorkIdProperty); }
            set { SetValue(WorkIdProperty, value); }
        }
    }
}
