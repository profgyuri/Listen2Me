using Listen2Me.MVVM.Settings;
using Microsoft.EntityFrameworkCore;

namespace Listen2Me.MVVM.Persistence;

public sealed class PostgresContextFactory
{
    private readonly ISettings _settings;
    private readonly IConnectionStringBuilder _connectionStringBuilder;

    public PostgresContextFactory(ISettings settings, IConnectionStringBuilder connectionStringBuilder)
    {
        _settings = settings;
        _connectionStringBuilder = connectionStringBuilder;
    }

    public PostgresDataContext Create()
    {
        var postgresSettings = _settings.Storage.PostgresStorage;
        var conn = _connectionStringBuilder.Build(postgresSettings);
        var options = new DbContextOptionsBuilder<PostgresDataContext>()
            .UseNpgsql(conn)
            .Options;

        return new PostgresDataContext(options);
    }
}