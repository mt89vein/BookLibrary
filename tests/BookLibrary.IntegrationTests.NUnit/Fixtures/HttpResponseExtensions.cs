using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BookLibrary.IntegrationTests.NUnit.Fixtures;

public static class HttpResponseExtensions
{
    public static (ProblemDetails?, string? errorCode) ReadAsProblemDetails(this HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.ProblemJson)
        {
            return (null, null);
        }

        using var stream = response.Content.ReadAsStream();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(stream, JsonSerializerOptions.Web);
        string? code = null;
        if (problem?.Extensions.TryGetValue("code", out var c) is true)
        {
            code = c?.ToString();
        }

        return (problem, code);
    }

    public static SettingsTask VerifyResponseAsync(
        this HttpResponseMessage response,
        string? requestName = null,
        [CallerFilePath] string sourceFile = "",
        [CallerMemberName] string callerMemberName = ""
    )
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(response.RequestMessage);

        response.RequestMessage.RequestUri =
            !string.IsNullOrWhiteSpace(requestName) ? new Uri(requestName, UriKind.Relative) : null;

        var suffix = !string.IsNullOrWhiteSpace(requestName) ? "_" + requestName : "";

        return Verify(response, sourceFile: sourceFile)
            .AutoVerify(includeBuildServer: false, throwException: false)
            .UseMethodName($"{callerMemberName}{suffix}")
            .ScrubInlineGuids()
            .IgnoreMembers("traceId", "errorDetails", "Cookie");
    }
}