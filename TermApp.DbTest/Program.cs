using System.IO;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Threading;

class Program
{
    static void Main()
    {
        //db接続のテスト用のプロジェクト
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

        string logPath = @"C:\appl\NewsApp\DbTest_log.txt"; 
        // ↑出力ログファイル パス 列挙されて登録されていきます。

        try
        {
            string cs = "server=localhost;user=root;password=pass;database=news_app;";
            using var con = new MySqlConnection(cs);
            con.Open();

            File.AppendAllText(logPath, "ok\n");
        }
        catch (Exception ex)
        {
            File.AppendAllText(logPath, $"❌ 接続エラー：{ex.Message}\n");
        }
    }
}
