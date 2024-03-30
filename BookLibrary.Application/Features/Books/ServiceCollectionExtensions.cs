using BookLibrary.Application.Features.Books.AddNewBook;
using BookLibrary.Application.Features.Books.BorrowBook;
using BookLibrary.Application.Features.Books.GetBook;
using BookLibrary.Application.Features.Books.GetBorrowedBooks;
using BookLibrary.Application.Features.Books.GetPagedBooks;
using BookLibrary.Application.Features.Books.ReturnBook;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Application.Features.Books;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register books features.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddBooksFeatures(this IServiceCollection services)
    {
        services.AddScoped<AddNewBookUseCase>();
        services.AddScoped<GetBookUseCase>();
        services.AddScoped<BorrowBookUseCase>();
        services.AddScoped<ReturnBookUseCase>();
        services.AddScoped<GetBorrowedBooksUseCase>();
        services.AddScoped<GetPagedBooksUseCase>();

        return services;
    }
}