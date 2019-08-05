using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Tools
{
    public interface IRemovable: IDisposable
    {
        void SetRemover(IDisposable remover);
    }
}
