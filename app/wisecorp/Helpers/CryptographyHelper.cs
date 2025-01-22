using System.Security.Cryptography;
using System.Text;

namespace wisecorp.Helpers;

public static class CryptographyHelper
{
    /// <summary>
    /// Hache un mot de passe en utilisant PBKDF2 avec un sel aléatoire
    /// </summary>
    /// <param name="input">Le mot de passe à hacher</param>
    /// <returns>Le mot de passe haché sous forme de chaîne Base64</returns>
    public static string HashPassword(string input)
    {
        // Generate a random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Create the salted hash
        var pbkdf2 = new Rfc2898DeriveBytes(input, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(32);

        // Combine the salt and password bytes for later use
        byte[] hashBytes = new byte[48]; // 16 for salt + 32 for hash
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        // Convert the combined bytes to a string and return it
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Vérifie si un mot de passe correspond à un hachage donné
    /// </summary>
    /// <param name="input">Le mot de passe à vérifier</param>
    /// <param name="hashedPassword">Le hachage du mot de passe à comparer</param>
    /// <returns>True si le mot de passe correspond, sinon False</returns>
    public static bool VerifyPassword(string input, string hashedPassword)
    {
        // Convert the hashed password to bytes
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        // Get the salt from the hashed password
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Create the salted hash
        var pbkdf2 = new Rfc2898DeriveBytes(input, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Compare the hashes
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Génère une chaîne aléatoire de caractères alphanumériques
    /// </summary>
    /// <param name="length">La longueur de la chaîne à générer</param>
    /// <returns>Une chaîne aléatoire de la longueur spécifiée</returns>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        using var rng = RandomNumberGenerator.Create();
        var data = new byte[length];
        rng.GetBytes(data);
        var result = new StringBuilder(length);
        foreach (var b in data)
        {
            result.Append(chars[b % chars.Length]);
        }
        return result.ToString();
    }
}