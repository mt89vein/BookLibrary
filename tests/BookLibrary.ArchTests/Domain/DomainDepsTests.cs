using ArchUnitNET.NUnit;

namespace BookLibrary.ArchTests.Domain;

public class DomainDepsTests
{
    [Test]
    public void Domain_Should_Not_DependOn_OuterLayers()
    {
        Types().That().Are(ProjectAssemblies.DomainLayer)
            .Should()
            .OnlyDependOnTypesThat()
            .Are(
                Types().That()
                    .Are(ProjectAssemblies.DomainLayer).Or()
                    .ResideInNamespace("FluentResults").Or()
                    .ResideInNamespace("System").Or()
                    .ResideInNamespace("Seedwork").Or()
                    .ResideInNamespace("Mediator")
            )
            .Check(ProjectAssemblies.Architecture);
    }
}