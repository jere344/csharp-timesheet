
using System.Windows;

namespace wisecorp.Views
{
    public partial class ReasonWindow : Window
    {
        public string Reason { get; private set; }

        public ReasonWindow()
        {
            InitializeComponent();
        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            Reason = ReasonTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}