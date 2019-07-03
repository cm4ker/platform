using System;
using System.Runtime.CompilerServices;
using Xunit;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Compiler.Sre;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Tests
{
    public class SimpleTests : TestBase
    {
        [Fact]
        public void SimpleTypeTest()
        {
            var zs = @"type Test {}";
            var expect =
                @"
class Test
{
}";
            var res = Transpile(zs);

            Assert.Equal(expect.Trim(), res);
        }

        [Fact]
        public void SimpleModuleTest()
        {
            var zs = @"module Test {}";
            var expect =
                @"
static public class Test
{
}";
            var res = Transpile(zs);

            Assert.Equal(expect.Trim(), res);
        }

        [Fact]
        public void ExpressionTest()
        {
            var zs = @"module Test { int Sum(int a, int b) {return a + b;}}";
            var expect =
                @"
static public class Test
{
    int Sum(int a, int b)
    {
        return (a + b);
    }
}";
            var res = Transpile(zs);

            Assert.Equal(expect.Trim(), res);
        }

        [Fact]
        public void MultiTypeTest()
        {
            var zs = @"module Test { int Sum(<int, string> a, int b) {return ((int)a) + b;}}";
            var expect =
                @"
static public class Test
{
    int Sum(UnionTypeStorage a, int b)
    {
        return ((int)a.Value + b);
    }
}";
            var res = Transpile(zs);

            Assert.Equal(expect.Trim(), res);
        }

        [Fact]
        public void IfFullTest()
        {
            var zs = @"
module Test 
{ 
    void IfTest()
    {
        bool r = false;
        if(6 > 1)
        {
            r = true;
        }
        else
        {
            r = false;
        }
    }
}";
            var expect =
                @"
static public class Test
{
    void IfTest()
    {
        bool r = false;
        if ((6 > 1))
        {
            r = true;
        }
        else
        {
            r = false;
        }
    }
}";
            var res = Transpile(zs);

            Assert.Equal(expect.Trim(), res);
        }

        [Fact]
        public void IfWithoutElseTest()
        {
            var zs = @"
module Test 
{ 
    void IfTest()
    {
        bool r = false;
        if(6 > 1)
        {
            r = true;
        }
    }
}";
            var expect =
                @"
static public class Test
{
    void IfTest()
    {
        bool r = false;
        if ((6 > 1))
        {
            r = true;
        }
    }
}";
            var res = Transpile(zs);

            Assert.Equal(expect.Trim(), res);
        }
    }
}