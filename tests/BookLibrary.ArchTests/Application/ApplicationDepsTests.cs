using ArchUnitNET.NUnit;

namespace BookLibrary.ArchTests.Application;

public class ApplicationDepsTests
{
    [Test]
    public void Application_Should_Not_DependOn_OuterLayers()
    {
        Types().That().Are(ProjectAssemblies.ApplicationLayer)
            .Should()
            .NotDependOnAnyTypesThat()
            .ResideInAssembly(ProjectAssemblies.InfrastructureAssembly, ProjectAssemblies.ApiAssembly)
            .Check(ProjectAssemblies.Architecture);
    }

    [Test]
    public void Application_Should_Not_DependOn_Frameworks()
    {
        Types().That().Are(ProjectAssemblies.ApplicationLayer)
            .Should()
            .NotDependOnAnyTypesThat()
            .Are(ProjectAssemblies.FrameworkDependencies)
            .Check(ProjectAssemblies.Architecture);
    }
}