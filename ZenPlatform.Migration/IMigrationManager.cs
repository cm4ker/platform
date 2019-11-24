using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Migration
{
    public interface IMigrationManager
    {
        bool CheckMigration(IXCRoot old, IXCRoot actual);
        void Migrate(IXCRoot old, IXCRoot actual);
    }
}