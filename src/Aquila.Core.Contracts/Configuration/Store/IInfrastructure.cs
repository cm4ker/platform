using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Core.Contracts.Configuration.Store
{
    public interface IInfrastructure
    {
        IUniqueCounter Counter { get; }

        ITypeManager TypeManager { get; }

        ISettingsManager Settings { get; }
    }
}