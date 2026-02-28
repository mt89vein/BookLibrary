using System.Runtime.CompilerServices;

namespace BookLibrary.IntegrationTests.NUnit.Fixtures;

internal static class VerifyInit
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyHttp.Initialize();
    }
}