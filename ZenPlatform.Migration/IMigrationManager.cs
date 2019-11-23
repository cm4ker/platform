using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Migration
{
    public interface IMigrationManager
    {
        bool CheckMigration(XCRoot old, XCRoot actual);
        void Migrate(XCRoot old, XCRoot actual);
    }
}