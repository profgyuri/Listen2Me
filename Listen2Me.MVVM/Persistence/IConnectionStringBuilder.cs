using Listen2Me.MVVM.Settings.Storage;

namespace Listen2Me.MVVM.Persistence;

public interface IConnectionStringBuilder
{
    /// <summary>
    /// Builds a connection string from the provided settings.
    /// </summary>
    /// <returns>Connection string to the postgres database.</returns>   
    string Build(PostgresStorageSettings settings);

    /// <summary>
    /// Builds a connection string from the provided settings and password.
    /// </summary>
    /// <returns>Connection string to the postgres database.</returns>  
    string Build(PostgresStorageSettings settings, string password);
}