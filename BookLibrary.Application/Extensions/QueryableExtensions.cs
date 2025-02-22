using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace BookLibrary.Application.Extensions;

/// <summary>
/// Extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Adds tag to query, which contains the name of source file and method name where it called.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="filePath"></param>
    /// <param name="memberName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> TagWithFileMember<T>(
        this IQueryable<T> query,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null
    )
    {
        if (!string.IsNullOrWhiteSpace(filePath) && !string.IsNullOrWhiteSpace(memberName))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var queryTag = $"{fileName}:{memberName}";
            return query.TagWith(queryTag);
        }

        return query;
    }
}