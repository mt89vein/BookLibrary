using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Application.Dto;

public static class PagedExtensions
{
    public static async Task<PageDto<T>> ToPagedListAsync<T>(
        this IOrderedQueryable<T> query,
        int pageNumber,
        int pageSize = 10,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(query);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number cannot be lesser than 1");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size cannot be lesser than 1");
        }

        var skip = (pageNumber - 1) * pageSize;

        var items = await query
            .Skip(skip)
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNextPage = false;
        if (items.Count > pageSize)
        {
            hasNextPage = true;
            items.RemoveAt(pageSize);
        }

        return new PageDto<T>
        {
            Items = items,
            HasNextPage = hasNextPage
        };
    }
}