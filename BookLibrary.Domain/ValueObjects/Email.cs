using BookLibrary.Domain.Exceptions;
using Seedwork;
using System.Net.Mail;

namespace BookLibrary.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    public Email(string email)
    {
        if (!MailAddress.TryCreate(email, out _))
        {
            throw ErrorCodes.InvalidEmail
                .ToException()
                .WithDetailedMessage($"'{email}' is not correct email");
        }

        Value = email;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}