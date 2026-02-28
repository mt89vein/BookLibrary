using ArchUnitNET.NUnit;

namespace BookLibrary.ArchTests.Domain;

public class DomainEventTests
{
    [Test]
    public void DomainEvents_Should_ResideInDomainLayer()
    {
        Types().That().Are(ProjectAssemblies.DomainEvents)
            .Should()
            .ResideInAssembly(ProjectAssemblies.DomainAssembly)
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void DomainEvents_Should_BeSealed()
    {
        Classes().That().Are(ProjectAssemblies.DomainEvents)
            .Should()
            .BeSealed()
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void DomainEvents_Should_HaveEventPostfix()
    {
        Classes().That().Are(ProjectAssemblies.DomainEvents)
            .Should()
            .HaveNameEndingWith("Event")
            .Check(ProjectAssemblies.Architecture);
    }
}