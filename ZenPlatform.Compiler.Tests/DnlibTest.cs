using System.Reflection;
using Xunit;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Dnlib;

namespace ZenPlatform.Compiler.Tests
{
    public class DnlibTest
    {
        private IAssemblyPlatform _ap = new DnlibAssemblyPlatform();

        [Fact]
        public void TestArgumentSequence()
        {
            _ap = new DnlibAssemblyPlatform();

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
    }
}