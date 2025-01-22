using System.Windows;
using System.Windows.Controls;
using wisecorp.ViewModels;

namespace wisecorp.Views.Admin
{
    /// <summary>
    /// Logique d'interaction pour ViewAjoutAcc.xaml
    /// </summary>
    public partial class ViewAjoutAcc : Page
    {
        public ViewAjoutAcc()
        {
            InitializeComponent();

            DataContext = new VMAdminAjouts();

            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).SetWindowInfos("WiseCorp - Créer Compte", 800, 600);
        }
    }
}
