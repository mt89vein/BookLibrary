namespace BookLibrary.Application.Dto;

public class PageDto<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();

    public bool HasNextPage { get; set; }
}