using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Indexer : Expression
    {
        private readonly Expression _arr;
        private readonly Expression _index;

        public Indexer(Expression arr, Expression index)
        {
            _arr = arr;
            _index = index;
        }

        public override void Dump(TextWriter tw)
        {
            _arr.Dump(tw);
            using (tw.SquareBrace())
            {
                _index.Dump(tw);
            }
        }
    }
}