using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Mono.Cecil;

namespace ZenPlatform.Compiler
{
    public static class CliBuilder
    {
        public static int Build(params string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");

            var optionVersion = app.Option("-v|--version", "Version of the program",
                CommandOptionType.NoValue);

            return app.Execute(args);
        }


        private static void RegisterCompileCommand(CommandLineApplication app)
        {
            app.Command("compile", conf =>
            {
                conf.HelpOption(true);

                var fileArg = conf.Argument("file", "File for compilation", true);
                var outArg = conf.Argument("out", "Output path", true);
                var assemblyName = conf.Argument("asmName", "Name of assembly");


                conf.OnExecute(() =>
                {
                    CompilationBackend cb = new CompilationBackend();
                    var ad = AssemblyDefinition.CreateAssembly(
                        new AssemblyNameDefinition(assemblyName.Name, Version.Parse("1.0.0.0")), assemblyName.Value,
                        ModuleKind.Dll);


                    foreach (var file in fileArg.Values)
                    {
                        using (var fs = File.OpenRead(file))
                            cb.Compile(fs, ad);
                    }

                    ad.Write(outArg.Value);
                });
            });
        }
    }
}