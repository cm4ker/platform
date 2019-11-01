using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Crypto;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Compiler.Platform
{
    /// <summary>
    /// Компилятор конфигурации - преобразовывает метаданные в коды CLI
    /// </summary>
    public class XCCompiller : IXCCompiller
    {
        public XCCompiller()
        {
        }

        public IAssembly Build(XCRoot configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType)
        {
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var assemblyBuilder = pl.CreateAssembly($"{configuration.ProjectName}{Enum.GetName(mode.GetType(), mode)}");

            var root = new Root(null, new List<CompilationUnit>());

            foreach (var component in configuration.Data.Components)
            {
                foreach (var type in component.Types)
                {
                    if (mode == CompilationMode.Client)
                        component.ComponentImpl.Generator.StageClient(type, root);
                    else
                        component.ComponentImpl.Generator.StageServer(type, root);
                }
            }

            AstScopeRegister.Apply(root);

            var generator = new Generator(new GeneratorParameters(root.Units, assemblyBuilder, mode, targetDatabaseType));
            generator.Build();

            return assemblyBuilder;
        }
    }
}