using System;
using System.IO;
using Aquila.Core.Instance;

namespace Aquila.Core;

public class AqConsoleContext : AqContext
{
    private readonly Stream _output;

    public AqConsoleContext(AqInstance instance, Stream output) : base(instance)
    {
        _output = output;
        InitOutput(_output);
    }
}