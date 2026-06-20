using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Npgsql;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Tabs.Settings;

public partial class StorageTabViewModel : ViewModelBase
{
    [ObservableProperty] private bool _usePostgres = false;
    [ObservableProperty] private string _postgresIp = "localhost";
    [ObservableProperty] private int _postgresPort = 5432;
    [ObservableProperty] private string _database = "postgres";
    [ObservableProperty] private string _userName = "postgres";
    [ObservableProperty] private ConnectionState _connectionState;
    
    public StorageTabViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
    }
    
    [RelayCommand]
    private async Task TestConnection(string password = "")
    {
        var connectionString = new NpgsqlConnectionStringBuilder()
        {
            Host = PostgresIp,
            Port = PostgresPort,
            Database = Database,
            Username = UserName,
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
}