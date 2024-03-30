using BookLibrary.Domain.Exceptions;
using System.Collections.Frozen;

namespace BookLibrary.Api.ProblemDetails;

/// <summary>
/// Mappings.
/// </summary>
public static class ErrorCodeMapping
{
    /// <summary>
    /// Mapping from error code to http status code.
    /// </summary>
    private static readonly FrozenDictionary<string, int> _statusCodeMap = new Dictionary<ErrorCodes, int>
    {
        // 2xx
        [ErrorCodes.AbonentNotFound] = StatusCodes.Status200OK,
        [ErrorCodes.BookNotFound] = StatusCodes.Status200OK,
        [ErrorCodes.CannotReturnNotBorrowedBook] = StatusCodes.Status200OK,
        [ErrorCodes.BookNotBorrowedByAnyone] = StatusCodes.Status200OK,
        [ErrorCodes.BookNotBorrowedByAbonent] = StatusCodes.Status200OK,
        [ErrorCodes.EmailAlreadyExists] = StatusCodes.Status200OK,
        [ErrorCodes.ThereNoBookThatCanBeBorrowed] = StatusCodes.Status200OK,
        [ErrorCodes.TooManyBooksBorrowedAlready] = StatusCodes.Status200OK,

        // 4xx
        [ErrorCodes.InvalidData] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidIsbn] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBookId] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBorrowerAbonentId] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidAbonentId] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidEmail] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBookPublishYear] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidAbonentSurname] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidAbonentName] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBookTitle] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBookReturnAbonentId] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBookAuthorName] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidBookAuthorSurname] = StatusCodes.Status400BadRequest,
        [ErrorCodes.BookMustHaveAnAuthors] = StatusCodes.Status400BadRequest,
        [ErrorCodes.UserUndefined] = StatusCodes.Status400BadRequest,

        [ErrorCodes.BookAlreadyBorrowed] = StatusCodes.Status409Conflict,

        // 5xx
        [ErrorCodes.Default] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.BookAddingFailed] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.BookGettingFailed] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.AbonentRegisteringFailed] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.BookReturningFailed] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.InvalidBorrowerBooksCount] = StatusCodes.Status500InternalServerError,
    }.ToFrozenDictionary(x => x.Key.GetErrorCode(), x => x.Value);

    /// <summary>
    /// Map error code to status code.
    /// </summary>
    /// <param name="errorCode">ErrorCode.</param>
    /// <returns>HTTP status code</returns>
    public static int MapToStatusCode(string errorCode)
    {
        ArgumentNullException.ThrowIfNull(errorCode);

        if (!_statusCodeMap.TryGetValue(errorCode, out var statusCode))
        {
            return StatusCodes.Status500InternalServerError;
        }

        return statusCode;
    }

    /// <summary>
    /// Is error code caused by exceptional case.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <returns>True, when error occured.</returns>
    public static bool IsError(string errorCode)
    {
        return MapToStatusCode(errorCode) >= 400;
    }
}