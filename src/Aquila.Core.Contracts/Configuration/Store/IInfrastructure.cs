using SharpFileSystem;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Configuration.Contracts.Store
{
    public interface IInfrastructure
    {
        IUniqueCounter Counter { get; }

        ITypeManager TypeManager { get; }

        ISettingsManager Settings { get; }
    }
}