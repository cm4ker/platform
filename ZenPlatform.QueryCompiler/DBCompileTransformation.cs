namespace ZenPlatform.QueryCompiler
{

    public abstract class DBCompileTransformation
    {
        public abstract string Apply(string compileExpression);
    }
}