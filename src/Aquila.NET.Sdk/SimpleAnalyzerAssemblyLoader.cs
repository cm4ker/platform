using System;
using Microsoft.CodeAnalysis;

namespace Aquila.NET.Sdk
{
    class SimpleAnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
    {
        public void AddDependencyLocation(string fullPath)
        {
            throw new NotImplementedException();
        }

        public System.Reflection.Assembly LoadFromPath(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
