using ArchUnitNET.Domain;
using ArchUnitNET.NUnit;

namespace BookLibrary.ArchTests.Application;

public sealed class UseCaseTests
{
    [Test]
    public void UseCase_Should_ResideInApplicationLayer()
    {
        Classes().That().Are(ProjectAssemblies.UseCases)
            .Should()
            .ResideInAssembly(ProjectAssemblies.ApplicationAssembly)
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void UseCases_Should_NotDependOnOtherUseCases()
    {
        var userCases = Classes().That().Are(ProjectAssemblies.UseCases);

        foreach (var useCase in userCases.GetObjects(ProjectAssemblies.Architecture))
        {
            Classes()
                .That()
                .Are(userCases)
                .And()
                .AreNot(useCase)
                .Should()
                .NotDependOnAnyTypesThat().Are(useCase)
                .Check(ProjectAssemblies.Architecture);
        }
    }

    [Test]
    public void UseCase_Should_HaveSingleExecuteMethod()
    {
        Classes()
            .That()
            .Are(ProjectAssemblies.UseCases)
            .Should()
            .FollowCustomCondition(c =>
                {
                    var publicMethods = Enumerable.Where(c.Members, m => m.Visibility == Visibility.Public && m is MethodMember
                    {
                        MethodForm: MethodForm.Normal
                    }).ToArray();

                    if (publicMethods.Length != 1)
                    {
                        return false;
                    }

                    var publicMethod = publicMethods.Single();

                    return publicMethod.Name.StartsWith("Execute(") || publicMethod.Name.StartsWith("ExecuteAsync(");
                },
                description: "UseCase should have single public Execute or ExecuteAsync method",
                failDescription: "does not have single public Execute or ExecuteAsync method"
            ).Check(ProjectAssemblies.Architecture);
    }
}