using BookLibrary.Application.Extensions;
using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BookLibrary.Api.Auth;

// Warning: this is super simple variant of authentication !not for production!, just for showcase.

internal static class MockAuthenticationConstants
{
    public const string SCHEME_NAME = "Mock";
    public const string SESSION_COOKIE_NAME = "sid";
}

/// <summary>
/// Account controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/account")]
[Tags("Account")]
public sealed class AccountController : ControllerBase
{
    /// <summary>
    /// Log in user by it's email.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <param name="ctx">Application context.</param>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromQuery] string email, [FromServices] IApplicationContext ctx)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(ctx);

        var emailVo = new Email(email);

        var abonent = await ctx.Abonents
            .TagWithFileMember()
            .FirstOrDefaultAsync(x => x.Email == emailVo);

        if (abonent is null)
        {
            return Unauthorized();
        }

        await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new("email", emailVo.ToString()),
            new("sub", abonent.Id.ToString())
        }, authenticationType: MockAuthenticationConstants.SCHEME_NAME)));

        return Ok();
    }

    /// <summary>
    /// Logs user out.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(MockAuthenticationConstants.SCHEME_NAME);

        return Ok();
    }
}

/// <summary>
/// Extensions methods for <see cref="AddMockAuthentication"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds authentication mock.
    /// </summary>
    public static IServiceCollection AddMockAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication()
            .AddScheme<MockEmailAuthenticationOptions, MockEmailAuthenticationHandler>(
                MockAuthenticationConstants.SCHEME_NAME,
                MockAuthenticationConstants.SCHEME_NAME,
                null
            )
            .Services;
    }
}

internal sealed class MockEmailAuthenticationOptions : AuthenticationSchemeOptions;

internal sealed class MockEmailAuthenticationHandler : SignInAuthenticationHandler<MockEmailAuthenticationOptions>
{
    public MockEmailAuthenticationHandler(
        IOptionsMonitor<MockEmailAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Cookies.TryGetValue(MockAuthenticationConstants.SESSION_COOKIE_NAME, out var session))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = JsonSerializer.Deserialize<MyClaim[]>(session);

        if (claims is null or { Length: 0 })
        {
            Context.Response.Cookies.Delete(MockAuthenticationConstants.SESSION_COOKIE_NAME);

            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims.Select(x => new Claim(x.Type, x.Value)), Scheme.Name));

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
    }

    protected override Task HandleSignOutAsync(AuthenticationProperties? properties)
    {
        Context.Response.Cookies.Delete(MockAuthenticationConstants.SESSION_COOKIE_NAME);

        return Task.CompletedTask;
    }

    protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        Context.Response.Cookies.Append(
            key: MockAuthenticationConstants.SESSION_COOKIE_NAME,
            value: JsonSerializer.Serialize(user.Claims.Select(x => new MyClaim(x.Type, x.Value))),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

        return Task.CompletedTask;
    }

    private sealed record MyClaim(string Type, string Value);
}