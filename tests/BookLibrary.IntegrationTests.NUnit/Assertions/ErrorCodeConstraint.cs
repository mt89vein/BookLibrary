using BookLibrary.IntegrationTests.NUnit.Fixtures;
using NUnit.Framework.Constraints;

namespace BookLibrary.IntegrationTests.NUnit.Assertions;

public class ProblemDetailsConstraintResult : ConstraintResult
{
    private readonly HttpResponseMessage _httpResponseMessage;

    public ProblemDetailsConstraintResult(
        IConstraint constraint,
        object? actualValue,
        bool isSuccess,
        HttpResponseMessage httpResponseMessage
    ) : base(constraint, actualValue, isSuccess)
    {
        _httpResponseMessage = httpResponseMessage;
    }

    public override void WriteMessageTo(MessageWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        var (problem, code) = _httpResponseMessage.ReadAsProblemDetails();

        writer.WriteLine("Response was:");

        writer.WriteLine($"  Status: {_httpResponseMessage.StatusCode}");
        writer.WriteLine($"  Content-Type: {_httpResponseMessage.Content.Headers.ContentType?.MediaType}");

        if (problem != null)
        {
            writer.WriteLine($"  Title: {problem.Title}");
            writer.WriteLine($"  Detail: {problem.Detail}");
            writer.WriteLine($"  Code: {code}");
        }
        else
        {
            writer.WriteLine("  Body: not parsed as ProblemDetails");
        }
    }
}