using System.IO;
using System.Windows.Media.Imaging;

namespace wisecorp.Models.DBModels;

public class Account : BaseModel
{
    public int RoleId { get; set; }
    public string Email { get; set; } // we login with email
    public string Password { get; set; }
    public int DepartementId { get; set; }
    public int TitleId { get; set;}
    public DateTime EmploymentDate { get; set; }
    public DateTime? DisableDate { get; set; }
    public string FullName { get; set; }
    public double Salary { get; set; }
    public bool IsEnabled { get; set; }
    public string Phone { get; set; }
    public double NbHour { get; set; }
    public double HourBank { get; set; }
    public string Pseudo { get; set; }
    public string PersonalEmail { get; set; }
    // base64
    public string Picture { get; set; }


    //Nav propreties
    public virtual Role Role { get; set; }
    public virtual Departement Departement { get; set; }
    public virtual Title Title { get; set; }

    // computed properties
    public bool IsDisabled => DisableDate != null;
    public bool IsAdmin => RoleId == 1;
    public bool IsManager => RoleId == 3;
    public bool IsEmployee => RoleId == 4;

    /// <summary>
    /// Crée une copie profonde de l'objet Account
    /// </summary>
    /// <returns>Une nouvelle instance d'Account avec les mêmes valeurs</returns>
    public Account DeepCopy()
    {
        Account accountCopy = new()
        {
            RoleId = RoleId,
            DepartementId = DepartementId,
            TitleId = TitleId,
            EmploymentDate = EmploymentDate,
            DisableDate = DisableDate,
            FullName = FullName,
            Phone = Phone,
            Salary = Salary,
            NbHour = NbHour,
            HourBank = HourBank,
            Email = Email,
            IsEnabled = IsEnabled,
            Password = Password,
            Picture = Picture,
            Role = Role,
            Departement = Departement,
            Title = Title
        };

        return accountCopy;
    }

    /// <summary>
    /// Obtient l'image de profil de l'utilisateur
    /// </summary>
    public BitmapImage ProfilePicture
    { 
        get {
            if (string.IsNullOrEmpty(Picture))
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/DefaultProfilePicture.webp"));
            }
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(Convert.FromBase64String(Picture));
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }

    /// <summary>
    /// Détermine si l'objet spécifié est égal à l'objet actuel
    /// </summary>
    /// <param name="obj">L'objet à comparer avec l'objet actuel</param>
    /// <returns>true si les objets sont égaux, sinon false</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Account account = (Account)obj;
        return account.Id == Id;
    }

    /// <summary>
    /// Sert de fonction de hachage par défaut
    /// </summary>
    /// <returns>Un code de hachage pour l'objet actuel</returns>
    public override int GetHashCode()
    {
		return Id;
	}
}
