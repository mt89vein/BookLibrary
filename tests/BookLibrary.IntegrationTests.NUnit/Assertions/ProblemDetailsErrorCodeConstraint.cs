using BookLibrary.Domain.Exceptions;
using BookLibrary.IntegrationTests.NUnit.Fixtures;
using NUnit.Framework.Constraints;
using System.Net;

namespace BookLibrary.IntegrationTests.NUnit.Assertions;

public sealed class ProblemDetailsErrorCodeConstraint : Constraint
{
    private readonly ErrorCodes _expectedCode;
    private HttpStatusCode? _expectedStatus;

    public override string Description =>
        $"HTTP {_expectedStatus} with application/problem+json and code '{_expectedCode}'";

    public ProblemDetailsErrorCodeConstraint(ErrorCodes expectedCode)
    {
        _expectedCode = expectedCode;
    }

    public ProblemDetailsErrorCodeConstraint WithStatusCode(HttpStatusCode status)
    {
        _expectedStatus = status;
        return this;
    }

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is not HttpResponseMessage response)
        {
            return new ConstraintResult(this, actual, false);
        }

        var (_, code) = response.ReadAsProblemDetails();

        var success = (response.StatusCode == _expectedStatus || _expectedStatus is null) && code == _expectedCode.GetErrorCode();

        return new ProblemDetailsConstraintResult(
            this,
            actual,
            success,
            response
        );
    }
}