using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace TermApp.Services
{
    public class MySqlService
    {
        private readonly string _connectionString =
            "server=localhost;user=root;password=pass;database=news_app;";

        public DataTable GetSummaries()
        {
            // DataTable はノンジェネリックな表形式オブジェクト
            var table = new DataTable();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql =
                  "SELECT news, created_at " +
                  "FROM day_news " +
                  "ORDER BY created_at DESC " +
                  "LIMIT 10;";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    // DataTable に reader の結果を丸ごと読み込む
                    table.Load(reader);
                }
            }

            return table;
        }
    }
}
