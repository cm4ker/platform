using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    public class IndexerAccess : Expression
    {
        private readonly Expression _array;
        private readonly Expression _indexExpression;

        public IndexerAccess(Expression indexExpression, Expression array)
        {
            _array = array;
            _indexExpression = indexExpression;
        }

        public override void Dump(TextWriter tw)
        {
            _array.Dump(tw);
            using (tw.SquareBrace())
                _indexExpression.Dump(tw);
        }
    }
}