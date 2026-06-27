using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.Settings.Storage;
using Listen2Me.MVVM.Settings.Storage.Credentials;
using Npgsql;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class StorageTabViewModel : ViewModelBase
{
    private readonly ISettings _settings;
    private readonly ICredentialSafe _credentialSafe;

    [ObservableProperty] private PostgresStorageSettings _postgres;
    [NotifyPropertyChangedFor(nameof(IsSaveButtonEnabled)), 
     ObservableProperty] private ConnectionState _connectionState;
    
    public bool IsSaveButtonEnabled => ConnectionState == ConnectionState.Open;
    
    public StorageTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, ISettings settings, 
        ICredentialSafe credentialSafe) 
        : base(errorHandler, logger, messenger)
    {
        _settings = settings;
        _credentialSafe = credentialSafe;
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _settings.Storage.LoadAsync(cancellationToken);
        Postgres = _settings.Storage.PostgresStorage ?? new PostgresStorageSettings();
    }

    [RelayCommand]
    private async Task TestConnection(string password = "")
    {
        var connectionString = new NpgsqlConnectionStringBuilder()
        {
            Host = Postgres.Host,
            Port = Postgres.Port,
            Database = Postgres.Database,
            Username = Postgres.Username,
            Password = password,
            Pooling = true,
            Timeout = 15
        }.ConnectionString;
        
        await using var connection = new NpgsqlConnection(connectionString);
        try
        {
            await connection.OpenAsync();
            ConnectionState = ConnectionState.Open;
            Logger.Information("Test connection succeeded.");
        }
        catch (NpgsqlException ex)
        {
            ConnectionState = ConnectionState.Broken;
            Logger.Error("Failed test connection: {0}", ex.Message);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    [RelayCommand]
    private async Task SaveConnection(string password = "")
    {
        if (ConnectionState is not ConnectionState.Open)
        {
            throw new InvalidOperationException("A successful test connection is required before saving.");
        }
        
        var encryptedPassword = _credentialSafe.Encrypt(password);
        _settings.Storage.PostgresStorage = Postgres with { EncryptedPassword = encryptedPassword };
        
        await _settings.Storage.SaveAsync();
        Logger.Information("Postgres settings saved.");
    }
}