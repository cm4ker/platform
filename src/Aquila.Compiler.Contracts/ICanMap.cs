using System.Data.Common;

namespace Aquila.Compiler.Contracts
{
    public interface ICanMap
    {
        void Map(DbDataReader dataReader);
    }

    public interface ICanSave
    {
        void Save(DbCommand command);
    }

    public interface ICanReLoad
    {
        void ReLoad();
    }
}