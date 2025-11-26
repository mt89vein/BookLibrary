
// this is the entrypoint for the whole booklibrary project
// when the need to scale comes, you can transform class libs: BookLibrary.Api, BookLibrary.BackgroundJobs and/or BookLibrary.Consumers to a host (add program.cs, dockerfile etc)
// but for now current entrypoint host all the stuff in the same process cause it much easier to manage and operate.

using BookLibrary.Api;
using BookLibrary.Application;
using BookLibrary.BackgroundJobs;
using BookLibrary.Consumers;
using BookLibrary.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .CreateBootstrapLogger();

Log.Information("Starting BookLibrary");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromSeconds(10));

    ApiModule.Register(builder);
    InfrastructureModule.Register(builder);
    BackgroundJobsModule.Register(builder);
    ConsumersModule.Register(builder);

    builder.Services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);
    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    ApiModule.MapEndpoints(app);
    InfrastructureModule.MapEndpoints(app);

    await app.MigrateAsync();

    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "BookLibrary terminated unexpectedly");

    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}