using System;

namespace Aquila.Core.Contracts.Network
{
    public interface IRemovable: IDisposable
    {
        void SetRemover(IDisposable remover);
    }
}
