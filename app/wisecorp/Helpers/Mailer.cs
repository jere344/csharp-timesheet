using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace wisecorp.Helpers;

public class Mailer
{
    private static readonly Lazy<Mailer> _instance = new(() => new Mailer());

    private readonly MailAddress SenderEmail;
    private readonly string SenderPassword;

    /// <summary>
    /// Constructeur privé pour initialiser les informations d'authentification de l'expéditeur
    /// </summary>
    private Mailer()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        SenderPassword = configuration["mailer:auth:pass"];
        SenderEmail = new(configuration["mailer:auth:user"], "Wiscorp");
    }

    public static Mailer Instance => _instance.Value;

    /// <summary>
    /// Envoie un email au destinataire spécifié avec le sujet et le corps fournis
    /// </summary>
    /// <param name="to">Adresse email du destinataire</param>
    /// <param name="subject">Sujet de l'email</param>
    /// <param name="body">Corps de l'email</param>
    public void SendMail(string to, string subject, string body)
    {
        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(SenderEmail.Address, SenderPassword)
        };
        using var message = new MailMessage(SenderEmail, new MailAddress(to))
        {
            Subject = subject,
            Body = body
        };
        smtp.Send(message);
    }
}
