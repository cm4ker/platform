using System.Data.Common;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ICanMap
    {
        void Map(DbDataReader dataReader);
    }

    public interface ICanSave
    {
        void Save(DbCommand command);
    }
}