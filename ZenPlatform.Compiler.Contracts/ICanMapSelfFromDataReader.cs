using System.Data.Common;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ICanMapSelfFromDataReader
    {
        void Map(DbDataReader dataReader);
    }
}