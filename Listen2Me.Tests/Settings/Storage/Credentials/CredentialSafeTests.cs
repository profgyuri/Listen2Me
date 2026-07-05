using Listen2Me.MVVM.Settings.Storage.Credentials;

namespace Listen2Me.Tests.Settings.Storage.Credentials;

[TestClass]
public class CredentialSafeTests
{
    private readonly ICredentialSafe _sut = new CredentialSafe();
    
    [TestMethod]
    public void ValidPassword_ReturnsEncryptedText()
    {
        var password = "password123";
        var encrypted = _sut.Encrypt(password);
        
        Assert.IsNotNull(encrypted);
        Assert.AreNotEqual(password, encrypted);
        Assert.AreNotEqual(password.ToLower(), encrypted.ToLower());
    }
    
    [TestMethod]
    public void DecryptedPassword_ReturnsOriginalText()
    {
        var password = "password123";
        var encrypted = _sut.Encrypt(password);
        
        var decrypted = _sut.Decrypt(encrypted);
        
        Assert.AreEqual(password, decrypted);
    }
    
    [TestMethod]
    public void InvalidEncryption_ThrowsFormatException()
    {
        var encrypted = _sut.Encrypt("password123");

        Assert.Throws<FormatException>(() => _sut.Decrypt(encrypted + "invalid"));
    }
    
    [TestMethod]
    public void EmptyPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.Encrypt(""));
        Assert.Throws<ArgumentException>(() => _sut.Decrypt(""));
    }
}