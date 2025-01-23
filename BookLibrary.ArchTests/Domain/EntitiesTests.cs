using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.NUnit;

namespace BookLibrary.ArchTests.Domain;

public sealed class EntitiesTests
{
    [Test]
    public void Entities_Should_ResideInDomainLayer()
    {
        Classes().That().Are(ProjectAssemblies.Entities)
            .Should()
            .ResideInAssembly(ProjectAssemblies.DomainAssembly)
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void Entities_Should_BeSealed()
    {
        Classes().That().Are(ProjectAssemblies.Entities)
            .Should()
            .BeSealed()
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void Entities_Should_HavePrivateParameterlessConstructor()
    {
        Classes()
            .That()
            .Are(ProjectAssemblies.Entities)
            .Should()
            .FollowCustomCondition(c => Enumerable.Any(c.Members, m => m.Visibility == Visibility.Private &&
                                                                       m is MethodMember
                                                                       {
                                                                           MethodForm: MethodForm.Constructor
                                                                       }),
                description: "have private parameterless constructor",
                failDescription: "does not have private parameterless constructor"
            ).Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void Entities_Should_UseOnlyPrivateSetters()
    {
        Classes()
            .That()
            .Are(ProjectAssemblies.Entities)
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