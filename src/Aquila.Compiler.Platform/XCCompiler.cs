using System;
using System.Collections.Generic;
using Aquila.Compiler.Cecil;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Dnlib;
using Aquila.Compiler.Generation;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Compiler.Visitor;
using Aquila.Configuration.Structure;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Data;
using Aquila.QueryBuilder;

namespace Aquila.Compiler.Platform
{
    /// <summary>
    /// Компилятор конфигурации - преобразовывает метаданные в коды CLI
    /// </summary>
    public class XCCompiler : IXCCompiller
    {
        private readonly RoslynAssemblyPlatform _platform;

        public XCCompiler()
        {
            _platform = new RoslynAssemblyPlatform();
        }
        
        public XCCompiler(RoslynAssemblyPlatform platform)
        {
            _platform = platform;
        }


        public RoslynAssemblyBuilder Build(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType)
        {
            var assemblyBuilder =
                _platform.AsmFactory.CreateAssembly(_platform.TypeSystem,
                    $"{configuration.ProjectName}{Enum.GetName(mode.GetType(), mode)}", Version.Parse("1.0.0.0"));

            var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, mode, targetDatabaseType,
                configuration));
            generator.BuildConf();

            return assemblyBuilder;
        }
        
        public RoslynAssemblyBuilder Build2(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType)
        {
            var assemblyBuilder =
                _platform.AsmFactory.CreateAssembly(_platform.TypeSystem,
                    $"{configuration.ProjectName}{Enum.GetName(mode.GetType(), mode)}", Version.Parse("1.0.0.0"));

            var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, mode, targetDatabaseType,
                configuration));
            generator.BuildConf();

            return assemblyBuilder;
        }

        // public Root BuildClientAst(IProject configuration)
        // {
        //     var assemblyBuilder = _platform.CreateAssembly("Client_ast_assemble");
        //     var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, CompilationMode.Client,
        //         SqlDatabaseType.Unknown,
        //         configuration));
        //     return generator.BuildAst();
        // }
        //
        // public Root BuildServerAst(IProject configuration)
        // {
        //     var assemblyBuilder = _platform.CreateAssembly("Server_ast_assemble");
        //     var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, CompilationMode.Server,
        //         SqlDatabaseType.Unknown,
        //         configuration));
        //     return generator.BuildAst();
        // }
    }
}