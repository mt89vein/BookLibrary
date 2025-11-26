using Microsoft.AspNetCore.Builder;

namespace BookLibrary.Consumers;

// use this assembly for consumers. e.g. kafka consumers

public class ConsumersModule
{
    public static void Register(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // register consumers here
    }
}