using BookLibrary.Domain.Exceptions;
using FluentResults;
using NUnit.Framework.Constraints;

namespace BookLibrary.UnitTests;

/// <summary>
/// Constraints for easier assertions using error codes.
/// </summary>
internal static class ErrorCodesConstraints
{
    /// <summary>
    /// Expects that throws exception with <paramref name="errorCode"/>.
    /// </summary>
    /// <param name="errorCode">Expected error code.</param>
    public static Constraint Expect(this ErrorCodes errorCode)
    {
        return Is.AssignableTo<IResultBase>()
            .And
            .Property(nameof(IResultBase.Errors))
            .Some.Matches<DomainErrorResult>(x => x.ErrorCode == errorCode);
    }
}