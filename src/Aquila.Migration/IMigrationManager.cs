using Aquila.Configuration.Contracts;
using Aquila.Configuration.Structure;

namespace Aquila.Migration
{
    public interface IMigrationManager
    {
        bool CheckMigration(IProject old, IProject actual);
        void Migrate(IProject old, IProject actual);
    }
}