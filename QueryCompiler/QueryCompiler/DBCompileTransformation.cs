namespace QueryCompiler
{

    public abstract class DBCompileTransformation
    {
        public abstract string Apply(string compileExpression);
    }
}