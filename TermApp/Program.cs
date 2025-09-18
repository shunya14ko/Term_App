using Microsoft.EntityFrameworkCore;
using System.Text;
using TermApp.Components;
using TermApp.Dbconn;
using TermApp.Models;
using TermApp.Service.Repository;

class Program
{
    //app settings

    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //utf8？？
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        //db connect
        try
        {
            var conn = DbConnect.BuildConnectionString();
            //EFの設定、ALLDbの
            builder.Services.AddDbContext<AllDbContext>(options =>
            {
                options.UseMySql(conn, ServerVersion.AutoDetect(conn));
            });
            Console.WriteLine("\n---DB接続成功---\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n---DB接続失敗---\n{ex}");
            throw;
        }

        //Razor Components @inject DIで使用
        builder.Services.AddScoped<ICrudRepository<Term>, TermRepository>();
        builder.Services.AddScoped<ICrudRepository<Note>, NoteRepository>();
        builder.Services.AddScoped<ICrudRepository<Group>, GroupRepository>();
        builder.Services
          .AddRazorComponents()
          .AddInteractiveServerComponents(o => { o.DetailedErrors = true; });

        //Service in
        var app = builder.Build();

        //HTTP request pipeline
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

