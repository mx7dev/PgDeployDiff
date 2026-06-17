namespace PgDeployDiff.App.Models;

public class ConnectionProfile
{
    public string Server { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool SavePassword { get; set; } = true;

    public string DisplayName => $"{Username}@{Server}:{Port}/{Database}";

    public string ToConnectionString() =>
        $"Host={Server};Port={Port};Database={Database};Username={Username};Password={Password}";
}
