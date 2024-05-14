using BookLibrary.Api.HealthChecks;
using BookLibrary.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Sstv.DomainExceptions.Extensions.SerilogEnricher;
using System.Net;

namespace BookLibrary.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(new JsonFormatter())
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting BookLibrary");

            var host = CreateHostBuilder(args).Build();

            await host.MigrateAsync();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "BookLibrary terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(ctx =>
            {
                ctx.AddSingleton<IStartupFilter, GracefulShutdownStartupFilter>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseShutdownTimeout(TimeSpan.FromSeconds(10));
            })
            .UseSerilog((ctx, c) =>
            {
                c.ReadFrom.Configuration(ctx.Configuration);
                c.WriteTo.Console(new JsonFormatter());
                c.Enrich.FromLogContext();
                c.Enrich.WithProperty("Host", ctx.Configuration.GetValue("HOSTNAME", Dns.GetHostName()));
                // add here env etc
                c.Enrich.WithDomainException();
            });
}