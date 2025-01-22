
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wisecorp.Views
{
    /// <summary>
    /// Logique d'interaction pour AddHour.xaml
    /// </summary>
    public partial class AddHour : Window
    {
        public AddHour()
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;
        }

        private void NumberValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            e.Handled = !regex.IsMatch(e.Text);


        }

        private void NumericMaxValue(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            int value;
            if (int.TryParse(textBox.Text, out value) && textBox != null)
            {
                if (value > 12)
                    value = 12;

                if (value < 0)
                    value = 0;

                textBox.Text = value.ToString();
            }
        }

        private void SelectAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox != null)
            {
                e.Handled = true;
                textBox.Focus();
            }

        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox != null)
                textBox.SelectAll();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var vm = (wisecorp.ViewModels.VMAddHour)this.DataContext;
            vm.Save();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HourBox.Focus();
            GotFocus(HourBox, null); 
        }

        private void HourBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Update the binding (save the input)
                if (sender is TextBox textBox)
                {
                    var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                    bindingExpression?.UpdateSource();
                }
                // Save
                Save_Click(sender, e);
            }
        }
    }
}
