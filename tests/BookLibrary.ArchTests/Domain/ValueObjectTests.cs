using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.NUnit;

namespace BookLibrary.ArchTests.Domain;

public sealed class ValueObjectTests
{
    [Test]
    public void ValueObjects_Should_ResideInDomainLayer()
    {
        Classes().That().Are(ProjectAssemblies.ValueObjects)
            .Should()
            .ResideInAssembly(ProjectAssemblies.DomainAssembly)
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void ValueObjects_Should_BeSealed()
    {
        Classes().That().Are(ProjectAssemblies.ValueObjects)
            .Should()
            .BeSealed()
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void ValueObjects_Should_UseOnlyPrivateSetters()
    {
        Classes()
            .That()
            .Are(ProjectAssemblies.ValueObjects)
            .Should()
            .FollowCustomCondition((c) =>
            {
                foreach (var member in c.GetPropertyMembers())
                {
                    if (member.Visibility == Visibility.Private || member.Setter is null)
                    {
                        continue;
                    }

                    if (member.Setter.Visibility != Visibility.Private)
                    {
                        return new ConditionResult(c, false, $"property {member.Name} should not have public property");
                    }
                }

                return new ConditionResult(c, true);
            }, "have no public setter on publicly accessible properties")
            .Check(ProjectAssemblies.Architecture);
    }
}