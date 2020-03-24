using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Compiler.Platform
{
    /// <summary>
    /// Компилятор конфигурации - преобразовывает метаданные в коды CLI
    /// </summary>
    public class XCCompiler : IXCCompiller
    {
        private readonly IAssemblyPlatform _platform;

        public XCCompiler(IAssemblyPlatform platform)
        {
            _platform = platform;
        }

        public IAssembly Build(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType)
        {
            var assemblyBuilder =
                _platform.CreateAssembly($"{configuration.ProjectName}{Enum.GetName(mode.GetType(), mode)}");

            var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, mode, targetDatabaseType,
                configuration));
            generator.BuildConf();

            return assemblyBuilder;
        }

        public Root BuildClientAst(IProject configuration)
        {
            var assemblyBuilder = _platform.CreateAssembly("Client_ast_assemble");
            var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, CompilationMode.Client,
                SqlDatabaseType.Unknown,
                configuration));
            return generator.BuildAst();
        }

        public Root BuildServerAst(IProject configuration)
        {
            var assemblyBuilder = _platform.CreateAssembly("Server_ast_assemble");
            var generator = new Generator(new GeneratorParameters(null, assemblyBuilder, CompilationMode.Server,
                SqlDatabaseType.Unknown,
                configuration));
            return generator.BuildAst();
        }
    }
}