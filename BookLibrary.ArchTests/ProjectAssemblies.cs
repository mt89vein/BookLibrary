using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Seedwork;
using Sstv.DomainExceptions;
using Assembly = System.Reflection.Assembly;

namespace BookLibrary.ArchTests;

internal static class ProjectAssemblies
{
    public static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            Assembly.Load("BookLibrary.Api"),
            Assembly.Load("BookLibrary.Infrastructure"),
            Assembly.Load("BookLibrary.Application"),
            Assembly.Load("BookLibrary.Domain")
        )
        .Build();

    public static ArchUnitNET.Domain.Assembly DomainAssembly { get; } =
        Architecture.Assemblies.First(x => x.Name.StartsWith("BookLibrary.Domain"));

    public static ArchUnitNET.Domain.Assembly ApplicationAssembly { get; } =
        Architecture.Assemblies.First(x => x.Name.StartsWith("BookLibrary.Application"));

    public static ArchUnitNET.Domain.Assembly InfrastructureAssembly { get; } =
        Architecture.Assemblies.First(x => x.Name.StartsWith("BookLibrary.Infrastructure"));

    public static ArchUnitNET.Domain.Assembly ApiAssembly { get; } =
        Architecture.Assemblies.First(x => x.Name.StartsWith("BookLibrary.Api"));

    public static IObjectProvider<IType> DomainLayer { get; } =
        Types().That().ResideInAssembly(DomainAssembly).As("Domain layer");

    public static IObjectProvider<Class> DomainEvents { get; } =
        Classes().That().ImplementInterface(typeof(IDomainEvent)).As("Domain events");

    public static IObjectProvider<Class> Entities { get; } =
        Classes().That().ImplementInterface(typeof(IEntity)).And().AreNotAbstract().As("Entities");

    public static IObjectProvider<Class> ValueObjects { get; } =
        Classes().That().AreAssignableTo(typeof(ValueObject)).And().AreNotAbstract().As("Value objects");

    public static IObjectProvider<IType> ErrorCodes { get; } =
        Types().That().AreEnums().And().HaveAnyAttributes(typeof(ErrorDescriptionAttribute)).As("Error codes");

    public static IObjectProvider<IType> ApplicationLayer { get; } =
        Types().That().ResideInAssembly(ApplicationAssembly).As("Application Layer");

    public static IObjectProvider<Class> UseCases { get; } =
        Classes().That().Are(".*UseCase", true).As("UseCases");

    public static IObjectProvider<IType> FrameworkDependencies { get; } =
        Types().That()
            .ResideInNamespace("System.Net.Http")
            .Or()
            .ResideInNamespace("Microsoft.AspNetCore")
            .As("Framework dependencies");
}