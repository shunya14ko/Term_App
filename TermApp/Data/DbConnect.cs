using MySqlConnector;
using System;

namespace TermApp.Data
{
    class DbConnect
    {
        private static readonly string ConnectionString =
        "Server=localhost;Port=3306;Database=term_app;User ID=root;Password=pass;";

        public static async Task TestConnectionAsync(ILogger logger)
        {
            using var conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT VERSION()", conn);
            var version = (string?)await cmd.ExecuteScalarAsync();
            logger.LogInformation("MySQL Version = {Version}", version);
        }
    }
}