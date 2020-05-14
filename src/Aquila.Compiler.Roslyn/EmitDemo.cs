using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;

namespace Aquila.Compiler.Roslyn
{
    public static class EmitDemo
    {
        public static void GenerateAssembly(string code, string path, string[] assemblies = null)
        {
            // 1. Generate AssemblyInfo.cs-like C# code and parse syntax tree
            StringBuilder asmInfo = new StringBuilder();

            asmInfo.AppendLine("using System.Reflection;");
            asmInfo.AppendLine("[assembly: AssemblyTitle(\"Test\")]");
            asmInfo.AppendLine("[assembly: AssemblyVersion(\"1.1.0\")]");
            asmInfo.AppendLine("[assembly: AssemblyFileVersion(\"1.1.0\")]");
            // Product Info
            asmInfo.AppendLine("[assembly: AssemblyProduct(\"Foo\")]");
            asmInfo.AppendLine("[assembly: AssemblyInformationalVersion(\"1.3.3.7\")]");

            var asmInfoTree = CSharpSyntaxTree.ParseText(asmInfo.ToString(), encoding: Encoding.Default);
            var tree = SyntaxFactory.ParseSyntaxTree(code);


            var dir = Path.GetDirectoryName(path);
            var fileName = "dumped.cs";

            using (var sw = new StreamWriter(Path.Combine(dir, fileName)))
                sw.Write(tree.GetRoot().NormalizeWhitespace().ToFullString());


            // Detect the file location for the library that defines the object type
            var systemRefLocation = typeof(object).GetTypeInfo().Assembly.Location;

            var pathToFw = "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\3.1.0\\ref\\netcoreapp3.1\\";

            var fwFiles = Directory.GetFiles(pathToFw, "*.dll");

            MetadataReference[] _ref;

            if (assemblies == null)
                _ref =
                    fwFiles
                        .Select(asm => MetadataReference.CreateFromFile(asm))
                        .ToArray();
            else
                _ref =
                    assemblies
                        .Select(asm => MetadataReference.CreateFromFile(asm))
                        .ToArray();

            // Create a reference to the library
            var systemReference = MetadataReference.CreateFromFile(systemRefLocation,
                new MetadataReferenceProperties(MetadataImageKind.Assembly));
            // A single, immutable invocation to the compiler
            // to produce a library

            var optimizationLevel = OptimizationLevel.Release;

            var compilation = CSharpCompilation.Create(Path.GetFileName(path))
                .WithOptions(
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(
                        optimizationLevel))
                .AddReferences(_ref)
                .AddSyntaxTrees(tree, asmInfoTree);

            // string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            EmitResult compilationResult = compilation.Emit(path);
            if (compilationResult.Success)
            {
                // Load the assembly
                // Assembly asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            }

            else
            {
                foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
                {
                    string issue = $@"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()},
                    Location: {codeIssue.Location.GetLineSpan()},
                    Severity: {codeIssue.Severity}";
                    Console.WriteLine(issue);
                }
            }
        }
    }
}