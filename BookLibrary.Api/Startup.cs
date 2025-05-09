using BookLibrary.Api.Auth;
using BookLibrary.Api.Extensions;
using BookLibrary.Api.HealthChecks;
using BookLibrary.Api.ProblemDetails;
using BookLibrary.Api.Swagger;
using BookLibrary.Application;
using BookLibrary.Infrastructure;
using BookLibrary.Infrastructure.OpenTelemetry;
using FluentResults;
using Sstv.DomainExceptions.Extensions.DependencyInjection;

namespace BookLibrary.Api;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwagger();
        services.AddInfrastructure();
        services.AddApiControllers();
        services.AddMockAuthentication();

        services.AddBookLibraryException();
        services.AddApplication();
        services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddEntityFramework();
        services.AddTelemetry();
        services.AddHealthChecking();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        Result.Setup(b =>
        {
            b.Logger = new FluentLogger(app.ApplicationServices.GetRequiredService<ILogger<FluentLogger>>());
        });

        app.UseErrorCodesDebugView();
        app.UseExceptionHandler();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireAuthorization();
            endpoints.MapSwaggerUI(app);
            endpoints.MapHealthCheckingEndpoints();
        });
    }
}