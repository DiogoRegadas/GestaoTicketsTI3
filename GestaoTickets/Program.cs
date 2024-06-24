using GestaoTickets.Models;
using Microsoft.Extensions.Logging;
internal class Program
{
    public static string Conector = "";
    public static string SmtpIP = "";
    public static string SessionContainerName = "";


    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(20));

        builder.Services.AddMvc();

        var config = builder.Configuration.GetSection("Configuracao").Get<Configuracao>();
        Conector = config.Conexao;
        SmtpIP = config.SmtpIP;
        SessionContainerName = "contaAtiva";

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var app = builder.Build();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();

        app.MapControllerRoute(
           name: "default",
           pattern: "{controller=Conta}/{action=Login}/{id?}"
       );
        app.Run();
    }
}