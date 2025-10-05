// TermApp/Dbconn/DbConnect.cs
using Microsoft.Extensions.Configuration;

namespace TermApp.Dbconn;

public static class DbConnect
{
    public static string BuildConnectionString(IConfiguration cfg)
    {
        // appsettings.json から設定を取得、無ければデフォルト値を使用
        var host = cfg["Db:Host"] ?? "localhost";
        var port = cfg["Db:Port"] ?? "3306";
        var db = cfg["Db:Database"] ?? "term_app";
        var user = cfg["Db:User"] ?? "termapp_user";
        var pass = cfg["Db:Password"] ?? cfg["DB_PASSWORD"];

        // 環境変数やuser-secretsでパスワードが設定されていない場合は例外をスロー
        if (string.IsNullOrWhiteSpace(pass))
            throw new InvalidOperationException("DBパスワードが未設定です（user-secrets か DB_PASSWORD を設定）。");

        return $"Server={host};" +
               $"Port={port};" +
               $"Database={db};" +
               $"User Id={user};" +
               $"Password={pass};" +
               $"SslMode=None;" +
               $"AllowPublicKeyRetrieval=True;";
    }
}
