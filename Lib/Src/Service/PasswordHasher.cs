using System.Security.Cryptography;

namespace todocrud.Lib.Src.Service;

public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA256;
    private const char Delimiter = '.';
    
    public string Hash(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, hashAlgorithmName);
        var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        var salt = Convert.ToBase64String(algorithm.Salt);
        return $"{hashAlgorithmName.Name}{Delimiter}{Iterations}{Delimiter}{salt}{Delimiter}{key}";
    }
    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split(Delimiter);
        if (parts.Length != 4)
        {
            throw new FormatException("Unexpected hash format. " +
                                      "Should be formatted as `{hashAlgorithmName}{delimiter}{iterations}{delimiter}{salt}{delimiter}{key}`.");
        }
        var iterations = int.Parse(parts[1]);
        var salt = Convert.FromBase64String(parts[2]);
        var key = Convert.FromBase64String(parts[3]);
        using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, hashAlgorithmName);
        var keyToCheck = algorithm.GetBytes(KeySize);
        return keyToCheck.SequenceEqual(key);
    }
    
}