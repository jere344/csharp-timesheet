using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Security.Principal;
using System.Windows.Input;
using wisecorp.Context;
using wisecorp.Models;
using wisecorp.Models.DBModels;
using System.Security;
using System.Runtime.InteropServices;
using wisecorp.Helpers;
using System.Windows.Threading;

namespace wisecorp.ViewModels;

public class VMForgotPassword : ObservableObject
{
    private string? email;
    public string? Email
    {
        get { return email; }
        set { 
            email = value; 
            OnPropertyChanged(nameof(CanSendCodeAndEmailFilled));
            OnPropertyChanged(nameof(CanResetPassword));
        }
    }

    private string? verificationCode;
    public string? VerificationCode
    {
        get { return verificationCode; }
        set { verificationCode = value; OnPropertyChanged(nameof(CanResetPassword));}
    }

    private SecureString? newPassword;
    public SecureString? NewPassword
    {
        get { return newPassword; }
        set { newPassword = value; OnPropertyChanged(nameof(CanResetPassword));}
    }

    private SecureString? repeatNewPassword;
    public SecureString? RepeatNewPassword
    {
        get { return repeatNewPassword; }
        set { repeatNewPassword = value; OnPropertyChanged(nameof(CanResetPassword));}
    }

    private string errorMessage;
    public string ErrorMessage
    {
        get { return errorMessage; }
        set { 
            errorMessage = value; 
            OnPropertyChanged();
        }
    }

    private readonly WisecorpContext context;

    public VMForgotPassword()
    {
        ErrorMessage = "";
        context = new WisecorpContext();

        lastCodeSentTime = DateTime.MinValue;
        CanSendCode = true;
        timer = new DispatcherTimer{ Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += Timer_Tick;
        OnPropertyChanged(nameof(CanResetPassword));
    }

    public ICommand SendVerificationCodeCommand => new RelayCommand(SendVerificationCode);

    /// <summary>
    /// Envoie un code de vérification à l'adresse email spécifiée
    /// </summary>
    private void SendVerificationCode()
    {
        if (string.IsNullOrEmpty(Email))
        {
            ErrorMessage = "Email is required";
            return;
        }

        var account = context.Accounts.FirstOrDefault(a => a.Email == Email);
        if (account == null)
        {
            ErrorMessage = "Email not found";
            return;
        }

        var code = context.GenerateVerificationCode(account);
        
        Mailer.Instance.SendMail(account.Email, "Verification code", code.Code);
        MessageBox.Show((string)Application.Current.FindResource("check_email_verification"));

        lastCodeSentTime = DateTime.Now;
        CanSendCode = false;
        timer.Start();

        App.Current.LogAction(SecurityLog.SendSecurityCode, "Verification code sent to " + account.Email);
    }

    public ICommand ResetPasswordCommand => new RelayCommand(ResetPassword);


    /// <summary>
    /// Réinitialise le mot de passe de l'utilisateur après vérification du code
    /// </summary>
    private void ResetPassword()
    {
        ErrorMessage = "";
        if (string.IsNullOrEmpty(Email))
        {
            ErrorMessage = "Email is required";
            return;
        }
        else if (string.IsNullOrEmpty(VerificationCode))
        {
            ErrorMessage = "Verification code is required";
            return;
        }
        else if (NewPassword == null)
        {
            ErrorMessage = "New password is required";
            return;
        }
        else if (RepeatNewPassword == null)
        {
            ErrorMessage = "Repeat new password is required";
            return;
        }

        var account = context.Accounts.FirstOrDefault(a => a.Email == Email);
        if (account == null)
        {
            ErrorMessage = "Email not found";
            return;
        }

        var code = context.VerificationCodes.FirstOrDefault(c => c.Code == VerificationCode && c.AccountId == account.Id);
        if (code == null)
        {
            ErrorMessage = "Invalid verification code";
            return;
        }

        if (code.ExpirationDate < DateTime.Now)
        {
            ErrorMessage = "Verification code expired";
            return;
        }
        
        var pass1 = new System.Net.NetworkCredential(string.Empty, NewPassword).Password;
        var pass2 = new System.Net.NetworkCredential(string.Empty, RepeatNewPassword).Password;
        if (pass1 != pass2)
        {
            ErrorMessage = "Passwords do not match ";
            return;
        }

        account.Password = CryptographyHelper.HashPassword(pass1);
        context.SaveChanges();

        MessageBox.Show((string)Application.Current.FindResource("password_updated"));

        ((MainWindow)App.Current.MainWindow).NavigateTo("Views/ViewLogin.xaml");
        App.Current.LogAction(SecurityLog.SendSecurityCode, "Password reset for " + account.Email);
    }

    public bool CanResetPassword => !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(VerificationCode) && NewPassword != null && RepeatNewPassword != null;

    // region timer to prevent sending multiple codes
    // every second we call TimerTick which update the CanSendCode property and the timer on the button
    // if the timer reach 5 minutes, the CanSendCode property is set to true
    // sending a code restarts the timer


    private DateTime lastCodeSentTime;
    private bool canSendCode;
    private DispatcherTimer timer;
    public bool CanSendCode
    {
        get => canSendCode;
        set
        {
            canSendCode = value;
            OnPropertyChanged(nameof(CanSendCode));
            OnPropertyChanged(nameof(SendCodeText));
            OnPropertyChanged(nameof(CanSendCodeAndEmailFilled));
        }
    }

    public bool CanSendCodeAndEmailFilled => CanSendCode && !string.IsNullOrEmpty(Email);

    private string sendCodeText = (string)Application.Current.FindResource("sendCode");
    public string SendCodeText
    {
        get => sendCodeText + (CanSendCode ? "" : $" ({4 - (DateTime.Now - lastCodeSentTime).Minutes:00}:{59 - (DateTime.Now - lastCodeSentTime).Seconds:00})");
    }

    /// <summary>
    /// Gère l'événement de tic du minuteur pour contrôler l'envoi de code
    /// </summary>
    /// <param name="sender">L'objet qui a déclenché l'événement</param>
    /// <param name="e">Les arguments de l'événement</param>

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if ((DateTime.Now - lastCodeSentTime).TotalMinutes >= 5)
        {
            CanSendCode = true;
            timer.Stop();
        }
        else
        {
            CanSendCode = false;
        }
    }
}
