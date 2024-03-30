using BookLibrary.Domain.Exceptions;
using JetBrains.Annotations;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Api.ProblemDetails;

/// <summary>
/// Example of problem details response.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class ErrorCodeProblemDetailsExample : IExamplesProvider<ErrorCodeProblemDetails>
{
    public ErrorCodeProblemDetails GetExamples()
    {
        return new ErrorCodeProblemDetails(ErrorCodes.Default.GetDescription())
        {
            Status = 500,
        };
    }
}