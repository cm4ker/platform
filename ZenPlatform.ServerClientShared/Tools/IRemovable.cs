using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.Tools
{
    public interface IRemovable: IDisposable
    {
        void SetRemover(IDisposable remover);
    }
}
