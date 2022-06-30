﻿using System.IO;
using System.Reflection;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Compiler.Tests;

namespace Aquila.Library.Scripting
{
    sealed class AquilaCompilationFactory : AquilaCompilationFactoryBase
    {
        System.Runtime.Loader.AssemblyLoadContext AssemblyLoadContext =>
            System.Runtime.Loader.AssemblyLoadContext.Default;

        public AquilaCompilationFactory()
        {
            AssemblyLoadContext.Resolving += AssemblyLoadContext_Resolving;
        }

        private Assembly AssemblyLoadContext_Resolving(System.Runtime.Loader.AssemblyLoadContext assCtx,
            AssemblyName assName)
            => TryGetSubmissionAssembly(assName);

        protected override Assembly LoadFromStream(MemoryStream peStream, MemoryStream pdbStream)
            => AssemblyLoadContext.LoadFromStream(peStream, pdbStream);
    }
}