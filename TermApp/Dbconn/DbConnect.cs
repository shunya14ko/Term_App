using MySqlConnector;

namespace TermApp.Dbconn;

public static class DbConnect
{
    public static string BuildConnectionString()
    {
        var conn = new MySqlConnectionStringBuilder
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
        //ConnctionStringは、MySQLの接続の戻り値を返すUsingクラスのメソッド
        return conn.ConnectionString;
    }
}