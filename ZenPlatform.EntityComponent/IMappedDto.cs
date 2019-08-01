using System.Data.Common;

namespace ZenPlatform.EntityComponent
{
    public interface IMappedDto
    {
        void Map(DbDataReader dataReader);
    }
}