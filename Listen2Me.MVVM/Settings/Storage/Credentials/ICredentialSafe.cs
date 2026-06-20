namespace Listen2Me.MVVM.Settings.Storage.Credentials;

/// <summary>
/// Encrypts and decrypts passwords.
/// </summary>
public interface ICredentialSafe
{
    /// <summary>
    /// Encrypts a password.
    /// </summary>
    /// <returns>Cryptographically encrypted password.</returns>
    string Encrypt(string password);
    
    /// <summary>
    /// Decrypts a password.
    /// </summary>
    /// <returns>Password in plain text.</returns>
    string Decrypt(string encryptedPassword);
}