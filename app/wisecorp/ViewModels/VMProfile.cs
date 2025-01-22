using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace wisecorp.ViewModels;

public class VMProfile : ObservableObject
{
    private Account _account;
    public Account Account
    {
        get => _account;
        set => SetProperty(ref _account, value);
    }
    public string Email => Account.Email;
    public Role? Role => Account?.Role;
    public string FullName => Account.FullName;
    public double Salary => Account.Salary;
    public Departement? Departement => Account?.Departement;
    public string? Title => Account.Title?.Name;
    public bool IsEnabled => Account.IsEnabled;
    public DateTime EmploymentDate => Account.EmploymentDate;
    public DateTime? DisableDate => Account.DisableDate;
    public string Phone
    {
        get => Account.Phone;
        set
        {
            Account.Phone = value;
            OnPropertyChanged();
            Update();
        }
    }
    public string PersonalEmail
    {
        get => Account.PersonalEmail;
        set
        {
            Account.PersonalEmail = value;
            OnPropertyChanged();
            Update();
        }
    }
    public string Pseudo
    {
        get => Account.Pseudo;
        set
        {
            Account.Pseudo = value;
            OnPropertyChanged();
            Update();
        }
    }
    public double NbHour => Account.NbHour;
    public double HourBank => Account.HourBank;

    // profile picture, base64
    public string Base64Picture
    {
        get => Account.Picture;
        set
        {
            Account.Picture = value;
            OnPropertyChanged();
            Update();
        }
    }

    public BitmapImage ProfilePicture
    {
        get
        {
            if (string.IsNullOrEmpty(Base64Picture))
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/DefaultProfilePicture.webp"));
            }
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(Convert.FromBase64String(Base64Picture));
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }

    public ICommand UploadPictureCommand { get; }

    /// <summary>
    /// Permet à l'utilisateur de télécharger une image de profil.
    /// Ouvre une boîte de dialogue pour sélectionner une image, redimensionne
    /// et compresse l'image sélectionnée, puis la convertit en chaîne Base64.
    /// </summary>
    private void UploadPicture()
    {
        // Use FileDialog to choose a picture
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;

            // Load the image
            using (var image = new Bitmap(filePath))
            {
                // Resize and compress the image
                var resizedImage = ResizeImage(image, 200, 200); // Resize to 100x100 for example

                // Convert to base64
                using (var ms = new MemoryStream())
                {
                    resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();
                    Base64Picture = Convert.ToBase64String(imageBytes);
                }
            }

            OnPropertyChanged(nameof(ProfilePicture));
        }
    }

    // https://gist.github.com/kiichi/ba4b188d3f64ea5bc71b
    private static Bitmap ResizeImage(Bitmap image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
            {
                wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }

    /// <summary>
    /// Met à jour les informations du compte dans la base de données.
    /// Cette méthode utilise le contexte de la base de données pour
    /// modifier les données du compte actuellement chargé.
    /// </summary>
    public void Update()
    {
        using var db = new WisecorpContext();
        db.Accounts.Update(Account);
        db.SaveChanges();
    }

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    public VMProfile(Account account)
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    {
        Account = account;
        UploadPictureCommand = new RelayCommand(UploadPicture);
    }

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    public VMProfile()
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    {
        Account = new Account();
        UploadPictureCommand = new RelayCommand(UploadPicture);
        // If empty, we use the App.CurrentUser
        if (App.Current.ConnectedAccount  == null)
        {
            throw new Exception("No account found");
        }
        Account = App.Current.ConnectedAccount;
        UploadPictureCommand = new RelayCommand(UploadPicture);
    }
}
