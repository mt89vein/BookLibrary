using BookLibrary.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BookLibrary.Api.Extensions;

/// <summary>
/// Extensions methods for <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Returns userId.
    /// </summary>
    /// <param name="ctx">HttpContext.</param>
    /// <returns>UserId.</returns>
    /// <exception cref="BookLibraryException">
    /// When userId is not defined.
    /// </exception>
    public static Guid GetUserId(this HttpContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        if (ctx.User.Identity?.IsAuthenticated == true &&
            Guid.TryParse(ctx.User.FindFirstValue("sub"), out var userId))
        {
            return userId;
        }

        throw ErrorCodes.UserUndefined.ToException();
    }
}