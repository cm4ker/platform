using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Builder
{
    public class XCCompiller
    {
        private readonly XCRoot _root;
        private readonly string _outputDirectory;

        public XCCompiller(XCRoot root, string outputDirectory)
        {
            _root = root;
            _outputDirectory = outputDirectory;
        }

        public void Build()
        {
            BuildData();
        }

        private void BuildData()
        {
            var sources = new List<SyntaxTree>();

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Core").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System").Location),
                MetadataReference.CreateFromFile(Assembly.Load("ZenPlatform.Core").Location),
                MetadataReference.CreateFromFile(Assembly.Load("ZenPlatform.DataComponent").Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(void).Assembly.Location),
            };


            foreach (var c in _root.Data.Components)
            {
                var files = c.ComponentImpl.Generator.GenerateFilesFromComponent();

                foreach (var file in files)
                {
                    sources.Add(SyntaxFactory.ParseSyntaxTree(file.Value, new CSharpParseOptions()));
                    using (var sw = new StreamWriter(file.Key))
                    {
                        sw.WriteLine(file.Value);
                    }

                }

                references.Add(MetadataReference.CreateFromFile(c.ComponentAssembly.Location));
            }


            var compilation = CSharpCompilation.Create("Build.dll", sources, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var emitResult = compilation.Emit("Data.dll");

            if (emitResult.Success)
            {
            }
        }
    }
}