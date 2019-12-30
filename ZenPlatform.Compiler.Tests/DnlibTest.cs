using System;
using System.IO;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.PE;
using Xunit;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;
using ZenPlatform.Language.Ast.Definitions;
using Attribute = System.Attribute;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ZenPlatform.Compiler.Tests
{
    public class DnlibTest
    {
        private IAssemblyPlatform _ap = new DnlibAssemblyPlatform();
        private IAssemblyPlatform _apCecil = new CecilAssemblyPlatform();

        [Fact]
        public void TestArgumentSequence()
        {
            var asm = _ap.CreateAssembly("test");
            var sb = asm.TypeSystem.GetSystemBindings();
            var t = asm.DefineType("Default", "One", TypeAttributes.Public | TypeAttributes.Abstract,
                sb.Object);

            var m = t.DefineMethod("M", true, true, false);

            var p = m.DefineParameter("A", sb.Int, false, false);

            Assert.Equal(0, p.ArgIndex);

            var m2 = t.DefineMethod("M2", true, false, false);

            var p2 = m.DefineParameter("A", sb.Int, false, false);

            Assert.Equal(1, p2.ArgIndex);
        }

        [Fact]
        public void TestBaseClassConstructorCall()
        {
            var asm = _ap.CreateAssembly("test");
            var sb = asm.TypeSystem.GetSystemBindings();

            var A = asm.DefineType("Default", "A", TypeAttributes.Public | TypeAttributes.Abstract,
                sb.Object);

            var B = asm.DefineType("Default", "B", TypeAttributes.Public | TypeAttributes.Abstract,
                A);

            A.DefineDefaultConstructor(false);
            B.DefineDefaultConstructor(false);

            Assert.Equal(A.Constructors.Count, 1);
            Assert.Equal(B.Constructors.Count, 1);

            var c = Assert.IsType<DnlibConstructorBuilder>(B.Constructors[0]);

            var m = Assert.IsType<MethodDefUser>(c.MethodDef.Body.Instructions[1].Operand);

            Assert.True(m.HasThis);


            asm.Write("TestBaseClassConstructorCall.bll");
        }


        [Fact]
        public void TestGenericInvoke()
        {
            var asm = _ap.CreateAssembly("test");
            var sb = asm.TypeSystem.GetSystemBindings();

            var A = asm.DefineType("Default", "A", TypeAttributes.Public | TypeAttributes.Abstract,
                sb.Object);

            var m = A.DefineMethod("MyMethod", true, true, false);
            m.WithReturnType(asm.TypeSystem.GetSystemBindings().Int);

            var g = m.Generator;

            var type = asm.TypeSystem.FindType<GenericTest>();
            var callingMethod = type.Methods[0].MakeGenericMethod(asm.TypeSystem.GetSystemBindings().Int);

            g.EmitCall(callingMethod);
            g.Ret();

            asm.Write("TestGenericInvoke.bll");


            var lib = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestGenericInvoke.bll"));

            var cmdType = lib.GetType("Default.A");

            var result = cmdType.GetMethod("MyMethod")
                .Invoke(null, BindingFlags.DoNotWrapExceptions, null, null, null);

            Assert.Equal(0, result);
        }


        [Fact]
        public void PureDnlibGenericTest()
        {
            AssemblyDef ad = new AssemblyDefUser("test");

            ModuleDefUser m = new ModuleDefUser("test");
            ad.Modules.Add(m);

            m.RuntimeVersion = MDHeaderRuntimeVersion.MS_CLR_40;
            m.Kind = ModuleKind.Dll;

            TypeDef td = new TypeDefUser("Default", "A", m.CorLibTypes.Object.TypeDefOrRef);

            td.Attributes |= dnlib.DotNet.TypeAttributes.Public | dnlib.DotNet.TypeAttributes.Abstract |
                             dnlib.DotNet.TypeAttributes.Sealed | dnlib.DotNet.TypeAttributes.AnsiClass;
            m.Types.Add(td);

            MethodDef md = new MethodDefUser("MyMethod", MethodSig.CreateStatic(m.CorLibTypes.Int32));
            md.Attributes |= MethodAttributes.Public | MethodAttributes.Static;
            td.Methods.Add(md);

            var gc = (TypeRefUser) m.Import(typeof(GenericTest));
            var gcTd = gc.ResolveTypeDef();
            var im = (IMethodDefOrRef) m.Import(typeof(GenericTest).GetMethod("InvokeMe"));

            var spec = new MethodSpecUser(im, new GenericInstMethodSig(m.CorLibTypes.Int32));

            md.Body = new CilBody();

            md.Body.Instructions.Add(Instruction.Create(OpCodes.Call, spec));
            md.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            ad.Write("Pure.bll", new ModuleWriterOptions(m)
            {
                PEHeadersOptions = new PEHeadersOptions()
                {
                    ImageBase = 0x00400000,
                    Subsystem = Subsystem.WindowsCui,
                    MajorLinkerVersion = 8
                },
                AddCheckSum = true,
                ModuleKind = ModuleKind.Dll
            });


            var lib = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pure.bll"));

            var cmdType = lib.GetType("Default.A");

            var result = cmdType.GetMethod("MyMethod")
                .Invoke(null, BindingFlags.DoNotWrapExceptions, null, null, null);

            Assert.Equal(0, result);
        }


        [Fact]
        public void TryCatchExceptionTest()
        {
            var asm = _ap.CreateAssembly("test");
            var sb = asm.TypeSystem.GetSystemBindings();

            var A = asm.DefineType("Default", "A", TypeAttributes.Public | TypeAttributes.Abstract,
                sb.Object);

            var m = A.DefineMethod("MyMethod", true, true, false);
            m.WithReturnType(sb.Int);
            var g = m.Generator;
            g.InitLocals = true;
            var loc = g.DefineLocal(sb.Int);
            g.BeginExceptionBlock();
            g.Throw(sb.Exception);
            g.BeginCatchBlock(sb.Exception);
            g.Pop();
            g.LdcI4(10);
            g.StLoc(loc);
            g.EndExceptionBlock();

            g.LdLoc(loc);

            g.Ret();

            asm.Write("TryCatchTest.bll");


            var lib = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TryCatchTest.bll"));

            var cmdType = lib.GetType("Default.A");

            var res = cmdType.GetMethod("MyMethod")
                .Invoke(null, BindingFlags.DoNotWrapExceptions, null, null, null);

            Assert.Equal(10, res);
        }

        [Fact]
        public void TestAttribute()
        {
            var asm = _ap.CreateAssembly("test");
            var sb = asm.TypeSystem.GetSystemBindings();
            var attr = asm.CreateAttribute<MyAttribute>(sb.String);

            attr.SetParameters("Test");

            var A = asm.DefineType("Default", "A", TypeAttributes.Public | TypeAttributes.Abstract,
                sb.Object);

            A.SetCustomAttribute(attr);

            asm.Write("CustomAttribute.bll");


            var lib = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CustomAttribute.bll"));

            var cmdType = lib.GetType("Default.A");

            var directAttribute = cmdType.GetCustomAttribute<MyAttribute>();

            Assert.NotNull(directAttribute);
        }
    }


    public class GenericTest
    {
        public static T InvokeMe<T>()
        {
            return default;
        }
    }

    public class MyAttribute : Attribute
    {
        public MyAttribute()
        {
        }

        public MyAttribute(string name)
        {
        }
    }
}