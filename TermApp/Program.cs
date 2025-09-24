using Microsoft.EntityFrameworkCore;
using System.Text;
using TermApp.Components;
using TermApp.Dbconn;
using TermApp.Models;
using TermApp.Service.Repository;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 文字化け対策（コンソール）
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // --- 設定読み込みの追加 ---
        // Development の時だけ user-secrets も読む（init 済みなら自動だが明示しておく）
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }
        // 環境変数も読む（launchSettings.json の environmentVariables や OS 環境変数）
        builder.Configuration.AddEnvironmentVariables();

        // --- DB 接続 ---
        try
        {
            // DbConnect は IConfiguration から値を読み、接続文字列を生成する実装にしておく
            var conn = DbConnect.BuildConnectionString(builder.Configuration);

            builder.Services.AddDbContext<AllDbContext>(options =>
                options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

            Console.WriteLine("\n---DB接続成功---\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n---DB接続失敗---\n{ex}\n");
            throw;
        }

        // DI
        builder.Services.AddScoped<ICrudRepository<Term>, TermRepository>();
        builder.Services.AddScoped<ICrudRepository<Note>, NoteRepository>();
        builder.Services.AddScoped<ICrudRepository<Group>, GroupRepository>();

        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents(o => { o.DetailedErrors = true; });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();

        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode();

        app.Run();
    }
}
