using Aquila.Syntax.Parser;

namespace Aquila.Compiler.Test2
{
    public record B(int A)
    {
    }

    public record Test : B
    {
        private int _a;


        public Test(int a) :base(a)
        {
            _a = a;
        }

        public int A
        {
            get { return _a; }
            //set { _a = value; }
            init { _a = value; }
        }
    }

    public static class Example
    {
        public static void E1()
        {
            var a = new Test(1);
            var b = a with
            {
                A = 2
            };
        }
    }
}