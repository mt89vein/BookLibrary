using BookLibrary.Domain.Exceptions;
using NUnit.Framework.Constraints;
using System.Net;

namespace BookLibrary.IntegrationTests.NUnit.Assertions;

public static class Response
{
    public static NotProblemDetailsConstraint IsNotProblemDetails => new();

    public static ProblemDetailsErrorCodeConstraint HasErrorCode(ErrorCodes code)
    {
        return new ProblemDetailsErrorCodeConstraint(code);
    }

    public static Constraint AsProblemDetails(this ErrorCodes errorCode, HttpStatusCode statusCode)
    {
        return new ProblemDetailsErrorCodeConstraint(
            errorCode
        ).WithStatusCode(statusCode);
    }
}