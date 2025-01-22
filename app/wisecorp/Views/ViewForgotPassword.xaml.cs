using System.Windows;
using System.Windows.Controls;
using System;
using wisecorp.ViewModels;
using System.Windows.Navigation;
using System.Threading;
using System.Globalization;

namespace wisecorp.Views;

public partial class ViewForgotPassword : Page
{
    public ViewForgotPassword()
    {
        InitializeComponent();
        this.DataContext = new VMForgotPassword();
        
        var parameter = SimpleNavigationService.Instance.Parameter;

        if (parameter != null && parameter is string email)
        {
            ((VMForgotPassword)DataContext).Email = email;
        }
        SimpleNavigationService.Instance.Parameter = null;
        this.Loaded += Page_Loaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).SetWindowInfos("WiseCorp - Reset Password", 600, 600);
    }

    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is VMForgotPassword viewModel && sender is PasswordBox newPasswordBox)
        {
            viewModel.NewPassword = newPasswordBox.SecurePassword;
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is VMForgotPassword viewModel && sender is PasswordBox confirmPasswordBox)
        {
            viewModel.RepeatNewPassword = confirmPasswordBox.SecurePassword;
        }
    }
}
