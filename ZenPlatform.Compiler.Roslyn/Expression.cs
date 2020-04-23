using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    public abstract class Expression
    {
        public abstract void Dump(TextWriter tw);


        public override string ToString()
        {
            var sw = new StringWriter();
            Dump(sw);
            return $"{GetType()} : {sw.GetStringBuilder()}";
        }
    }
}