using System.Security.Cryptography;
using System.Text;

namespace Listen2Me.MVVM.Settings.Storage.Credentials;

/// <inheritdoc cref="ICredentialSafe"/>
public class CredentialSafe : ICredentialSafe
{
    private readonly byte[] _entropy = "Listen2Me.Postgres"u8.ToArray();
    
    /// <inheritdoc/>
    public string Encrypt(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        
        var bytes = Encoding.UTF8.GetBytes(password);
        var encrypted = ProtectedData.Protect(bytes, _entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    /// <inheritdoc/>
    public string Decrypt(string encryptedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedPassword);
        
        var bytes = Convert.FromBase64String(encryptedPassword);
        var decrypted = ProtectedData.Unprotect(bytes, _entropy, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(decrypted);
    }
}