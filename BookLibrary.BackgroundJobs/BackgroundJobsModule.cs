using Microsoft.AspNetCore.Builder;

namespace BookLibrary.BackgroundJobs;

// use this assembly for background jobs. e.g. hangfire jobs, quartz, hosted services etc

public class BackgroundJobsModule
{
    public static void Register(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // register background jobs here
    }
}