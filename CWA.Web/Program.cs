using CWA.Data;
using CWA.Data.Seed;
using CWA.Entities.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Program initialization logging
            SetBootstrapLogger();

            try
            {
                Log.Information("Iniciando proceso de la aplicación.");

                using var host = CreateHostBuilder(args).Build();

                // ToDo: Move seed call to private method

                // !!!!!! COMMENT BEFORE RELEASE AND COMMIT !!!!!!
                Log.Information("Procesando carga de datos semilla.");
                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var context = services.GetRequiredService<DataContext>();
                _ = await Seed.SeedAsync(userManager, context);
                //await Seed.SeedPROTPlantilla(context); DO NOT USE
                //await Seed.SeedPROTExcel(context); DO NOT USE
                // !!!!!! COMMENT BEFORE RELEASE !!!!!!

                Log.Information("Iniciando aplicación.");
                await host.RunAsync();

                // When stopping
                Log.Information("El proceso de la aplicación se ha detenido manualmente o por inactividad.");
            }
            catch (Exception any)
            {
                Log.Fatal($"El proceso de la aplicación se ha detenido inesperádamente: {any.Message} {any.StackTrace}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Use default builder
            var builder = Host.CreateDefaultBuilder(args);

            // Add additional configuration files here
            // Sample:
            // builder.ConfigureAppConfiguration(app => app.AddJsonFile("additionalSettings.json"));

            // Configure the Web Host
            builder.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

            // Add application logging
            builder.UseSerilog((hostContext, services, configuration) => configuration
                .ReadFrom.Configuration(hostContext.Configuration)
                .ReadFrom.Services(services));

            return builder;
        }

        private static void SetBootstrapLogger()
        {            
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new LoggerConfiguration();
            string logPath;

            if (environmentName is not null && environmentName.ToString().ToUpper() == "DEVELOPMENT")
            {
                // Development
                configuration.WriteTo.Console();
                logPath = @"C:\GIT\cndwebapps\Pruebas\Logs\cndwebapps-log-.txt";
            }
            else
            {
                // Production
                configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                logPath = @"E:\Logs\Web\cndwebapps-log-.txt";
            }

            // Complete
            configuration
                .WriteTo.File(path: logPath, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}")
                .Enrich.FromLogContext();

            Log.Logger = configuration.CreateBootstrapLogger();
        }
    }
}
