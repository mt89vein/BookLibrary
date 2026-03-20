using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.NUnit;
using Sstv.DomainExceptions;
using Enum = System.Enum;

namespace BookLibrary.ArchTests.Domain;

public class ErrorCodeTests
{
    [Test]
    public void ErrorCodes_Should_ResideInDomainLayer()
    {
        Types().That().Are(ProjectAssemblies.ErrorCodes)
            .Should()
            .ResideInAssembly(ProjectAssemblies.DomainAssembly)
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void ErrorDescription_Should_BePlacedOnEnum()
    {
        Types().That().Are(ProjectAssemblies.ErrorCodes)
            .Should()
            .BeEnums()
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void ErrorDescription_Should_HavePrefix_When_PlacedOnEnum()
    {
        Types().That().Are(ProjectAssemblies.ErrorCodes)
            .Should()
            .FollowCustomCondition(c =>
            {
                var attribute = Enumerable.Single(c.AttributeInstances,
                    x => x.Type.Name == nameof(ErrorDescriptionAttribute));

                if (!attribute.HasAttributeArguments)
                {
                    return new ConditionResult(c, false, "Empty error description");
                }

                var prefix = Enumerable.FirstOrDefault(attribute.AttributeArguments, x => x is AttributeNamedArgument
                {
                    Name: nameof(ErrorDescriptionAttribute.Prefix)
                });

                if (string.IsNullOrWhiteSpace(prefix?.Value as string))
                {
                    return new ConditionResult(c, false, "have no error code prefix");
                }

                return new ConditionResult(c, true);
            }, "have error code prefix")
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void ErrorCodes_Should_BeWithErrorDescriptionAttribute()
    {
        Types().That().Are(ProjectAssemblies.ErrorCodes)
            .Should()
            .FollowCustomCondition(c =>
            {
                foreach (var member in Enumerable.Where(c.Members, x => x.IsStatic is true))
                {
                    var attribute = Enumerable.Single(member.AttributeInstances,
                        x => x.Type.Name == nameof(ErrorDescriptionAttribute));

                    if (!attribute.HasAttributeArguments)
                    {
                        return new ConditionResult(c, false,
                            $"{member.Name} must be decorated with [ErrorDescriptionAttribute]");
                    }

                    var description = Enumerable.FirstOrDefault(attribute.AttributeArguments, x =>
                        x is AttributeNamedArgument
                        {
                            Name: nameof(ErrorDescriptionAttribute.Description)
                        });

                    if (string.IsNullOrWhiteSpace(description?.Value as string))
                    {
                        return new ConditionResult(c, false,
                            $"{member.Name} has no description on [ErrorDescriptionAttribute]");
                    }

                    if (description.Value is string { Length: > 100 or < 3 })
                    {
                        return new ConditionResult(c, false,
                            $"{member.Name} Description in [ErrorDescriptionAttribute] must be between 3 and 100 characters");
                    }

                    if (description.Value is string s && s.EndsWith('.'))
                    {
                        return new ConditionResult(c, false,
                            $"{member.Name} Description in [ErrorDescriptionAttribute] should end with a period");
                    }

                    if (description.Value is string ss && ss.Trim().Length != ss.Length)
                    {
                        return new ConditionResult(c, false, $"{member.Name} description has whitespaces");
                    }
                }

                return new ConditionResult(c, true);
            }, "error description must be filled with [ErrorDescriptionAttribute]")
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void ErrorCodes_Should_HaveUniqueErrorCodes()
    {
        Types().That().Are(ProjectAssemblies.ErrorCodes)
            .Should()
            .FollowCustomCondition(c =>
            {
                var enumType = Enumerable.Where(c.GetFieldMembers(), x => x.IsStatic is true)
                    .Select(x => x.Type)
                    .FirstOrDefault()!;

                var qualifiedName = enumType.FullName + ", " + enumType.Assembly.Name;
                var type = Type.GetType(qualifiedName, throwOnError: true)!;
                var values = Enum.GetValuesAsUnderlyingType(type);
                var hashSet = new HashSet<object>();

                foreach (var value in values)
                {
                    if (!hashSet.Add(value))
                    {
                        return new ConditionResult(c, false, $"contains duplicated value {value}");
                    }
                }

                return new ConditionResult(c, true);
            }, "have unique error codes")
            .Check(ProjectAssemblies.Architecture);
    }
}