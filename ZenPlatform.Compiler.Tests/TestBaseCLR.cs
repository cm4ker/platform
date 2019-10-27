using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using Module = ZenPlatform.Language.Ast.Definitions.Module;

namespace ZenPlatform.Compiler.Tests
{
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

        public void ImportRef()
        {
            var asm = ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));
            asm.ImportWithCopy(asm.TypeSystem.GetSystemBindings().Client);
            var asmName = $"test.bll";

            if (File.Exists(asmName))
                File.Delete(asmName);

            asm.Write(asmName);
        }

        public object CompileAndRun(string funcScript)
        {
            var asm = ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));

            Function node = (Function) funcScript.Parse(x => _zlv.VisitFunctionDeclaration(x.functionDeclaration()));

            CompilationUnit cu = new CompilationUnit(null, null, new List<TypeEntity>
            {
                new Module(null,
                    new TypeBody(null, new List<Function> {node}, new List<Field>(), new List<Property>(),
                        new List<Constructor>()), "Test")
            });


            AstScopeRegister.Apply(cu);

            var gp = new GeneratorParameters(cu, asm, CompilationMode.Server);

            var gen = new Generator(gp);


            var asmName = $"test.bll";

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
}