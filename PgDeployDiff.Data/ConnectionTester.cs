using Npgsql;

namespace PgDeployDiff.Data;

public static class ConnectionTester
{
    public static async Task<string> TestAsync(string connectionString, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(ct);
        return conn.ServerVersion;
    }
}
