using System;
using McMaster.Extensions.CommandLineUtils;

namespace ZenPlatform.Builder
{
    public class Program
    {
        public static int Main(params string[] args)
        {
            return CliBuilder.Build(args);
        }
    }
}