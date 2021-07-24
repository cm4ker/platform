using Aquila.Configuration.Structure;
using Aquila.Core.Contracts;

namespace Aquila.Migrations
{
    public interface IMigrationManager
    {
        bool CheckMigration(IProject old, IProject actual);
        void Migrate(IProject old, IProject actual);
    }
}