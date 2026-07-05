using Listen2Me.MVVM.Settings.Storage;
using Listen2Me.MVVM.Settings.Storage.Credentials;
using Npgsql;

namespace Listen2Me.MVVM.Persistence;

public class PostgresConnectionStringBuilder : IConnectionStringBuilder
{
    private readonly ICredentialSafe _credentialSafe;
    
    public PostgresConnectionStringBuilder(ICredentialSafe credentialSafe)
    {
        _credentialSafe = credentialSafe;
    }

    public string Build(PostgresStorageSettings settings)
    {
        var password = _credentialSafe.Decrypt(settings.EncryptedPassword);
        return Build(settings, password);
    }

    public string Build(PostgresStorageSettings settings, string password)
    {
        var builder = new NpgsqlConnectionStringBuilder()
        {
            Host = settings.Host,
            Port = settings.Port,
            Database = settings.Database,
            Username = settings.Username,
            Password = password,
            Pooling = true,
            Timeout = 15
        };
        
        return builder.ConnectionString;
    }
}