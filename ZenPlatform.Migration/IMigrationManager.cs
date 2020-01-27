using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Migration
{
    public interface IMigrationManager
    {
        bool CheckMigration(IRoot old, IRoot actual);
        void Migrate(IRoot old, IRoot actual);
    }
}