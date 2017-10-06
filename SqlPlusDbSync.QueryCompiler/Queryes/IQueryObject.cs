namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public interface IQueryObject
    {
        string Compile();
        string Compile(CompileOptions options);
    }
}