using BookLibrary.Api.Extensions;
using BookLibrary.Api.ProblemDetails;
using BookLibrary.Api.Swagger;
using BookLibrary.Application;
using BookLibrary.Infrastructure;
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

        services.AddBookLibraryException();
        services.AddApplication();
        services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddEntityFramework();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler();
        app.UseRouting();

        app.UseErrorCodesDebugView();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwaggerUI(app);
        });
    }
}