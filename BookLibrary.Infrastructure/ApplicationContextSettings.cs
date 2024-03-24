using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Configuration of <see cref="ApplicationContext"/>.
/// </summary>
public sealed class ApplicationContextSettings
{
    /// <summary>
    /// Connection string.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "ConnectionString is not set")]
    public string ConnectionString { get; set; } = null!;
}

/// <summary>
/// Configures <see cref="ApplicationContextSettings"/>.
/// </summary>
internal sealed class ConfigureApplicationContextSettings : IConfigureOptions<ApplicationContextSettings>
{
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Creates new instance of <see cref="ConfigureApplicationContextSettings"/>
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public ConfigureApplicationContextSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Invoked to configure a ApplicationContextSettings instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(ApplicationContextSettings options)
    {
        options.ConnectionString = _configuration.GetConnectionString("DefaultConnection")!;
    }
}