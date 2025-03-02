using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuchShop.Framework.Data;
using BuchShop.Services;
using OfficeOpenXml;  // Für EPPlus (benötigt für Excel Generierung)

namespace BuchShop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // EPPlus-Lizenzkontext festlegen (wichtig, bevor ExcelPackage verwendet wird)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    // Stelle sicher, dass die Datenbank existiert
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.EnsureCreated();

                    // Starte das Hauptmenü (welches Login/Registrierung über AuthenticationService aufruft)
                    var mainMenuService = services.GetRequiredService<MainMenuService>();
                    mainMenuService.DisplayMainMenu();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
                }
            }
        }

        // Konfiguriere den Host und die DI-Container
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Connection String aus der Konfiguration laden
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Registriere alle benötigten Services
                    services.AddTransient<AuthenticationService>();  // Für Login/Registrierung
                    services.AddTransient<InventoryService>();         // Für Produkt-/Lagerverwaltung
                    services.AddTransient<ShoppingService>();            // Für Einkaufsfunktionen
                    services.AddTransient<MainMenuService>();            // Hauptmenü, das die Authentifizierung aufruft
                });
    }
}
