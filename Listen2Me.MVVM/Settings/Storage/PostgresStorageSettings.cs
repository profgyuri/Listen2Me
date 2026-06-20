namespace Listen2Me.MVVM.Settings.Storage;

/// <summary>
/// Holds settings to build a Postgres connection string.
/// </summary>
public record PostgresStorageSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to use Postgres at all.
    /// </summary>
    public bool UsePostgres { get; init; }
    
    /// <summary>
    /// Postgres host IP address.
    /// </summary>
    public string Host { get; init; } = "localhost";
    
    /// <summary>
    /// Postgres port.
    /// </summary>
    public int Port { get; init; } = 5432;
    
    /// <summary>
    /// Postgres database name.
    /// </summary>
    public string Database { get; init; } = "postgres";
    
    /// <summary>
    /// Postgres username.
    /// </summary>
    public string Username { get; init; } = "postgres";
    
    /// <summary>
    /// Encrypted password.
    /// </summary>
    public string EncryptedPassword { get; init; } = "";
}