using Aquila.QueryBuilder;

namespace Aquila.Core.Contracts.Data
{
    public interface IXCCompiller
    {
        void Build(IProject configuration, CompilationMode mode, SqlDatabaseType targetDatabaseType);
    }
}
