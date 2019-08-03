using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Antlr4.Runtime;
using Xunit.Sdk;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Generation.NewGenerator;
using ZenPlatform.Compiler.Preprocessor;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using Module = ZenPlatform.Language.Ast.Definitions.Module;
using SyntaxNodeExtensions = Microsoft.CodeAnalysis.SyntaxNodeExtensions;

namespace ZenPlatform.Compiler.Tests
{
    public abstract class TestBase
    {
        private VRoslyn _r;
        private ZLanguageVisitor _zlv;
        IAssemblyPlatform ap = new CecilAssemblyPlatform();

        public TestBase()
        {
            _r = new VRoslyn(new CompilationOptions() {Mode = CompilationMode.Client});
            _zlv = new ZLanguageVisitor();
        }

        public int A(object i)
        {
            return (int) i + 1;
        }

        public string Transpile(string text)
        {
            var parser = Parse(text);
            var result = (CompilationUnit) _zlv.Visit(parser.entryPoint());

            var glob = new Root(null, new List<CompilationUnit> {result});

//            AstSymbolVisitor sv = new AstSymbolVisitor();
//            sv.Visit(glob);

            var t = _r.Visit(result);
            return SyntaxNodeExtensions.NormalizeWhitespace(t).ToFullString();
        }

        private ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }

        private ZSharpParser Parse(string text)
        {
            using TextReader tr = new StringReader(text);
            return Parse(tr);
        }

        public ZSharpParser Parse(TextReader input)
        {
            return Parse(CreateInputStream(input));
        }

        private ITokenStream CreateInputStream(Stream input)
        {
            return PreProcessor.Do(new AntlrInputStream(input));
        }

        private ITokenStream CreateInputStream(TextReader reader)
        {
            return PreProcessor.Do(new AntlrInputStream(reader));
        }
    }


    // This is a collectible (unloadable) AssemblyLoadContext that loads the dependencies

    // of the plugin from the plugin's binary directory.

    class HostAssemblyLoadContext : AssemblyLoadContext
    {
        // Resolver of the locations of the assemblies that are dependencies of the

        // main plugin assembly.

        private AssemblyDependencyResolver _resolver;


        public HostAssemblyLoadContext(string pluginPath) : base(isCollectible: true)

        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }


        // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded

        // into the HostAssemblyLoadContext together with the plugin assembly itself.

        // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would

        // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.

        // The types present on the host and plugin side would then not match even though they would have the same names.

        protected override Assembly Load(AssemblyName name)

        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);

            if (assemblyPath != null)

            {
                Console.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");

                return LoadFromAssemblyPath(assemblyPath);
            }


            return null;
        }
    }


    public abstract class TestBaseCLR : IDisposable
    {
        private ZLanguageVisitor _zlv;
        IAssemblyPlatform ap = new CecilAssemblyPlatform();


        public TestBaseCLR()
        {
            _zlv = new ZLanguageVisitor();
        }

        object ExecuteAndUnload(string assemblyPath, string methodName, out WeakReference alcWeakRef)
        {
            var alc = new HostAssemblyLoadContext(assemblyPath);
            alcWeakRef = new WeakReference(alc);

            Assembly a = alc.LoadFromAssemblyPath(assemblyPath);

            var ns = "CompileNamespace";
            var cName = "Test";

            Type execClass = a.GetType($"{ns}.{cName}");

            MethodInfo method = execClass.GetMethod(methodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            object result = method.Invoke(null, null);

            alc.Unload();


            return result;
        }


        public void Clear()
        {
        }

        public object CompileAndRun(string funcScript)
        {
            var asm = ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));

            Function node = (Function) funcScript.Parse(x => _zlv.VisitFunctionDeclaration(x.functionDeclaration()));

            CompilationUnit cu = new CompilationUnit(null, null, new List<TypeEntity>
            {
                new Module(null,
                    new TypeBody(null, new List<Function> {node}, new List<Field>(), new List<Property>()), "Test")
            });


            AstScopeRegister.Apply(cu);

            var gp = new GeneratorParameters(cu, asm, CompilationMode.Server);

            var gen = new Generator(gp);


            var asmName = $"{Guid.NewGuid().ToString()[28..]}.bll";

            if (File.Exists(asmName))
                File.Delete(asmName);

            try
            {
                asm.Write(asmName);
                var asmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, asmName);
                var result = ExecuteAndUnload(asmPath, node.Name, out var wr);

                for (int i = 0; wr.IsAlive && (i < 10); i++)
                {
                    GC.Collect();

                    GC.WaitForPendingFinalizers();
                }


                return result;
            }

            finally
            {
                if (File.Exists(asmName))
                    File.Delete(asmName);
            }
        }

        public void Dispose()
        {
//            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.bll");
//
//            foreach (var file in files)
//            {
//                File.Delete(file);
//            }
        }
    }


    public static class TransformZSharpHelper
    {
        public static SyntaxNode Parse(this string text, Func<ZSharpParser, SyntaxNode> a)
        {
            return a(Parse(text));
        }

        private static ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }

        private static ZSharpParser Parse(string text)
        {
            using TextReader tr = new StringReader(text);
            return Parse(tr);
        }

        public static ZSharpParser Parse(TextReader input)
        {
            return Parse(CreateInputStream(input));
        }

        private static ITokenStream CreateInputStream(Stream input)
        {
            return PreProcessor.Do(new AntlrInputStream(input));
        }

        private static ITokenStream CreateInputStream(TextReader reader)
        {
            return PreProcessor.Do(new AntlrInputStream(reader));
        }
    }
}