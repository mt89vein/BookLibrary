using BookLibrary.Domain.Exceptions;
using Seedwork;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BookLibrary.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    [SuppressMessage("Design", "SYSLIB1045")]
    private static readonly Regex _emailRegex = new(
        @"^(?'localPart'((((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)| \\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c \u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t ]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|( ""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n )[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\ ([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+ )?|((\r\n)[ \t]+)+))*?)(\.(((\((((?'paren'\()|(?'-paren'\))| ([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n )[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\ u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+ ((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_ `{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001- \u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t ]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([ \u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+(( \r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))*))@(?'domain'((((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\ u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t ]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]| [\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?( ([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?| ((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e] |[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+ ((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\ r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?)(\ .(((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\ u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\ n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e- \u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?"")) ((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t ]+)+))*?))*)|(((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\ ([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+ )?|((\r\n)[ \t]+)+))*?\[(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t] +)+)?([!-Z^-~]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f ]))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?\]((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e ]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\ r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n \0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(pare n)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))\z");

    public string Value { get; }

    public Email(string email)
    {
        if (!_emailRegex.IsMatch(email) || !MailAddress.TryCreate(email, out _))
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

    public static Email From(string email)
    {
        return new Email(email);
    }

    public override string ToString()
    {
        return Value;
    }
}