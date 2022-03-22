using System;
using System.IO;
using Aquila.Core.Instance;

namespace Aquila.Core;

public partial class AqContext
{
    public class AqConsoleContext : AqContext
    {
        private readonly Stream _output;

        public AqConsoleContext(IAqInstance instance, Stream output) : base(instance)
        {
            _output = output ?? Console.OpenStandardOutput();
            InitOutput(_output);
        }
    }

    public static AqContext CreateConsole() => CreateConsole(args: Array.Empty<string>());

    public static AqContext CreateConsole(params string[] args) => CreateConsole(null, args);

    public static AqContext CreateConsole(Stream output, params string[] args)
    {
        return new AqConsoleContext(new AqDummyInstance(), output);
    }
}