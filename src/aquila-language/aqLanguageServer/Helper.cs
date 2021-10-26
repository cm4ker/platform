using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Aquila.LanguageServer
{
    internal static class DotNetSdkHelper
    {
        private static readonly Regex DotNet60Regex =
            new Regex(@"^3\.1\.\d+", RegexOptions.Multiline | RegexOptions.Compiled);

        public static bool? IsDotNet60Installed()
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--list-sdks",
                RedirectStandardOutput = true,
            });
            if (process?.WaitForExit(3000) != true || process.ExitCode != 0)
            {
                return null;
            }

            var sdks = process.StandardOutput.ReadToEnd();
            return DotNet60Regex.IsMatch(sdks);
        }
    }
}