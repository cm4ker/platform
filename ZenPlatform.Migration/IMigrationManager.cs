using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Migration
{
    public interface IMigrationManager
    {
        bool CheckMigration(IProject old, IProject actual);
        void Migrate(IProject old, IProject actual);
    }
}