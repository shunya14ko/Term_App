using MySqlConnector;
using Microsoft.Extensions.Logging;

namespace TermApp.Data;

public static class DbConnect
{
    public static string BuildConnectionString()
    {
        var b = new MySqlConnectionStringBuilder
        {
            Server = DbParameters.Host,
            Port = DbParameters.Port,
            Database = DbParameters.Database,
            UserID = DbParameters.UserId,
            Password = DbParameters.Password,
            // 必要なら:
            // SslMode = MySqlSslMode.Required,
            // AllowUserVariables = true,
        };
        return b.ConnectionString;
    }


    //下記DB診断用
    public static async Task TestConnectionAsync(ILogger logger)
    {
        await using var conn = new MySqlConnection(BuildConnectionString());
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand("SELECT VERSION()", conn);
        var version = (string?)await cmd.ExecuteScalarAsync();

        logger.LogInformation("MySQL Version = {Version}", version);
    }
}