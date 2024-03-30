using BookLibrary.Domain.Exceptions;
using JetBrains.Annotations;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Filters;

namespace BookLibrary.Api.ProblemDetails;

/// <summary>
/// Example of problem details response.
/// </summary>
[UsedImplicitly]
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