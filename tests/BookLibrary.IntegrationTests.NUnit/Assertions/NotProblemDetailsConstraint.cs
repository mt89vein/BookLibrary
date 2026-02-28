using NUnit.Framework.Constraints;
using System.Net.Mime;

namespace BookLibrary.IntegrationTests.NUnit.Assertions;

public class NotProblemDetailsConstraint : Constraint
{
    public override string Description => "response is not ProblemDetails";

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        if (actual is not HttpResponseMessage response)
        {
            return new ConstraintResult(this, actual, ConstraintStatus.Error);
        }

        if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.ProblemJson)
        {
            return new ConstraintResult(this, actual, true);
        }

        return new ProblemDetailsConstraintResult(
            this,
            actual,
            false,
            response
        );
    }
}