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

namespace wisecorp.ViewModels;

public class VMLogin : ObservableObject
{
    private string? email;
    public string? Email
    {
        get { return email; }
        set { email = value; }
    }

    //Partie pour le password pas en clair
    private SecureString? securePassword;

    /// <summary>
    /// Set le password en SecureString
    /// </summary>
    /// <param name="password">Mots de passe</param>
    public void SetPassword(SecureString password)
    {
        securePassword = password;
    }

    private string? password;
    public string? Password
    {
        get { return password; }
        set { password = value; }
    }

    private string errorMessage;
    public string ErrorMessage
    {
        get { return errorMessage; }
    }

    private readonly WisecorpContext context;

    public VMLogin()
    {
        errorMessage = "";
        context = new WisecorpContext();
        Login = new RelayCommand(Login_Execute);
        ForgotPasswordCommand = new RelayCommand(ForgotPassword_Execute);
    }

    public VMLogin(WisecorpContext context)
    {
        errorMessage = "";
        this.context = context;
        Login = new RelayCommand(Login_Execute);
        ForgotPasswordCommand = new RelayCommand(ForgotPassword_Execute);
    }

    public ICommand ForgotPasswordCommand { get; }
    private void ForgotPassword_Execute()
    {
        ((MainWindow)Application.Current.MainWindow).NavigateTo("Views/ViewForgotPassword.xaml", email);
    }


    /// <summary>
    /// Commande pour gérer le processus de connexion de l'utilisateur.
    /// Vérifie les champs de saisie, valide les informations d'identification et
    /// effectue la connexion si les informations sont correctes.
    /// </summary>
    public ICommand Login { get; }
    private void Login_Execute()
    {
        if(Email == null ) { errorMessage = "Le courriel ne peut pas etre vide."; }
        if(Email == null && Password == null) { errorMessage = "Le mot de passe et le courriel ne peuvent pas etre vide."; }
        if (securePassword == null || securePassword.Length == 0)
        {
            errorMessage = "Le mot de passe ne peut pas etre vide.";
        }


        if (Email != null && securePassword != null)
        {
            errorMessage = "";

            Account? account = context.Login(Email, ConvertToUnsecureString(securePassword));

            if(account == null) { errorMessage = "Le mot de passe ou le courriel sont incorrect"; BadLogin(); }

            if(account != null) 
            {
                SuccessfullLogin(account);
                MainWindow main = (MainWindow)Application.Current.MainWindow;
                if (rememberMe)
                {
                    SaveCredentials(account);
                }
                main.NavigationController.NavigateHome();
            } 
        }



        OnPropertyChanged(nameof(ErrorMessage));
    }
    //Converti le mots de passe une string
    private static string ConvertToUnsecureString(SecureString securePassword)
    {
        IntPtr unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
            return Marshal.PtrToStringUni(unmanagedString);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
    }

    private bool rememberMe = false;
    public bool RememberMe
    {
        get { return rememberMe; }
        set { rememberMe = value; }
    }
    /// <summary>
    /// Enregistre les informations d'identification de l'utilisateur dans la base de données.
    /// Crée un nouveau jeton de session avec un identifiant de compte, un jeton unique et une date d'expiration.
    /// </summary>
    /// <param name="account">L'objet Account contenant les informations du compte utilisateur.</param>
    private void SaveCredentials(Account account)
    {
        SessionToken sessionToken = new() {
            AccountId = account.Id,
            Token = Guid.NewGuid().ToString(),
            ExpirationDate = DateTime.Now.AddDays(5) 
        };

        context.SessionTokens.Add(sessionToken);
        context.SaveChanges();

        ((App)Application.Current).SavedSettings["SessionToken"] = sessionToken.Token;
    }

    /// <summary>
    /// Logs un login réussi dans la base de donnée
    /// </summary>
    /// <param name="account">le compte qui a reussi le login</param>
    private void SuccessfullLogin(Account account)
    {
       SecurityLog log = new SecurityLog 
       { 
            Account = account,
            Code = SecurityLog.LoginSuccess,
            Date = DateTime.Now,
            Ip = App.GetIPAddress(),
            Description = ""
       };
        context.SecurityLogs.Add(log);
        context.SaveChanges();
    }

    /// <summary>
    /// Log une mauvaise connexion dans la base de donnée de logs
    /// </summary>
    private void BadLogin()
    {
        SecurityLog log = new SecurityLog
        {
            Account = null,
            Code = SecurityLog.LoginFailed,
            Date = DateTime.Now,
            Ip = App.GetIPAddress(),
            Description = $"Tentative de connexion avec le courriel {Email}"
        };
        context.SecurityLogs.Add(log);
        context.SaveChanges();
    }
}
