using BookLibrary.Domain.Aggregates.Books;
using Mediator;

namespace BookLibrary.Infrastructure.Books;

public sealed record BookBorrowDeleted(Book Book) : INotification;