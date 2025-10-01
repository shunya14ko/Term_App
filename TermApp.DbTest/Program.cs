using System;
using System.Globalization;
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;

class Program
{
    private const string ConnectionStringVariable = "TERMAPP_CONNECTION_STRING";

    static void Main()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        string logPath = @"C:\\appl\\NewsApp\\DbTest_log.txt";

        try
        {
            string? connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                File.AppendAllText(logPath, $"⚠️ 環境変数 {ConnectionStringVariable} が設定されていません。\n");
                return;
            }

            using var con = new MySqlConnection(connectionString);
            con.Open();

            File.AppendAllText(logPath, $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ} ✅ MySQL接続成功！\n");
        }
        catch (Exception ex)
        {
            File.AppendAllText(logPath, $"❌ 接続エラー：{ex.Message}\n");
        }
    }
}
