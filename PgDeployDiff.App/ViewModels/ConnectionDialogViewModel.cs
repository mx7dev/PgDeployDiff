using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PgDeployDiff.App.Models;
using PgDeployDiff.Data;

namespace PgDeployDiff.App.ViewModels;

public partial class ConnectionDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _server = "localhost";
    [ObservableProperty] private int _port = 5432;
    [ObservableProperty] private string _database = string.Empty;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private bool _savePassword = true;
    [ObservableProperty] private bool _isTesting;
    [ObservableProperty] private string _testStatus = string.Empty;

    // Sincronizado desde code-behind vía PasswordChanged (PasswordBox no soporta binding)
    public string Password { get; set; } = string.Empty;

    public ConnectionProfile ToProfile() => new()
    {
        Server = Server,
        Port = Port,
        Database = Database,
        Username = Username,
        Password = SavePassword ? Password : string.Empty,
        SavePassword = SavePassword
    };

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        IsTesting = true;
        TestStatus = "Conectando...";
        try
        {
            var version = await ConnectionTester.TestAsync(ToProfile().ToConnectionString());
            TestStatus = $"Conexion exitosa - PostgreSQL {version}";
        }
        catch (Exception ex)
        {
            TestStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsTesting = false;
        }
    }
}
