using System.IO;

namespace Aquila.LanguageServer.IntegrationTests;

public static class TestConstant
{
    public static string RootPath = Path.Combine(GetRootDirectory(), "Build", "library");

    public static string TestFilePath = Path.Combine(RootPath, "test.aq");

    static string GetRootDirectory()
    {
        var d = Directory.GetCurrentDirectory();
        while (!File.Exists(Path.Combine(d!, "Aquila.sln")))
        {
            d = Path.GetDirectoryName(d);
        }

        return d;
    }
}