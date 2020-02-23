using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.QueryBuilder;
using Module = ZenPlatform.Language.Ast.Definitions.Module;

namespace ZenPlatform.Compiler.Tests
{
    public abstract class TestBaseCLR : IDisposable
    {
        private ZLanguageVisitor _zlv;

        protected IAssemblyPlatform Ap = new DnlibAssemblyPlatform();
        //IAssemblyPlatform ap = new CecilAssemblyPlatform();

        public TestBaseCLR()
        {
            _zlv = new ZLanguageVisitor();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        object ExecuteAndUnload(string assemblyPath, string methodName, out WeakReference alcWeakRef)
        {
            var alc = new HostAssemblyLoadContext(assemblyPath);
            alcWeakRef = new WeakReference(alc);

            Assembly a = alc.LoadFromAssemblyPath(assemblyPath);

            var cName = "Test";

            Type execClass = a.GetType($"{cName}");

            MethodInfo method = execClass.GetMethod(methodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            object result = method.Invoke(null, null);

            alc.Unload();

            return result;
        }


        public void Clear()
        {
        }

        public void ImportRef()
        {
            var asm = Ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));
            asm.ImportWithCopy(asm.TypeSystem.GetSystemBindings().Client);
            var asmName = $"test.bll";

            if (File.Exists(asmName))
                File.Delete(asmName);

            asm.Write(asmName);
        }

        public void Compile(string unit)
        {
            var asm = Ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));

            var cunit = (CompilationUnit) unit.Parse(x => _zlv.VisitEntryPoint(x.entryPoint()));

            Root r = new Root(null, new CompilationUnitList {cunit});

            AstScopeRegister.Apply(r);

            var gp = new GeneratorParameters(r, asm, CompilationMode.Server,
                SqlDatabaseType.SqlServer, null);

            var gen = new Generator(gp);

            gen.Build();

            var asmName = $"test.bll";
            if (File.Exists(asmName))
                File.Delete(asmName);

            asm.Write(asmName);
        }

        public object CompileAndRun(string funcScript)
        {
            var asm = Ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));

            Function node = (Function) funcScript.Parse(x => _zlv.VisitFunctionDeclaration(x.functionDeclaration()));

            CompilationUnit cu = new CompilationUnit(null, null, new EntityList
            {
                new Module(null,
                    new TypeBody(new List<Member> {node}, null), "Test")
            }, new NamespaceDeclarationList());


            Root r = new Root(null, new CompilationUnitList {cu});

            AstScopeRegister.Apply(r);
            LoweringOptimizer.Apply(asm.TypeSystem, r);

            var gp = new GeneratorParameters(r, asm, CompilationMode.Server,
                SqlDatabaseType.SqlServer, null);

            var gen = new Generator(gp);

            gen.Build();

            var asmName = $"test.bll";
            if (File.Exists(asmName))
                File.Delete(asmName);

            try
            {
                asm.Write(asmName);
                var asmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, asmName);
                var result = ExecuteAndUnload(asmPath, node.Name, out var wr);

                for (int i = 0; i < 8 && wr.IsAlive; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                if (wr.IsAlive)
                {
                    throw new Exception("Unload failed");
                }


                if (File.Exists(asmName))
                    File.Delete(asmName);

                return result;
            }
            finally
            {
            }
        }

        public void Dispose()
        {
        }
    }
}