using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Core.Tools
{
    public class ListRemover<T> : IDisposable
    {
        private readonly IList<T> removers;
        private readonly T remover;

        public ListRemover(IList<T> removers, T remover)
        {
            this.removers = removers;
            this.remover = remover;
        }

        public void Dispose()
        {
            if (remover != null && removers.Contains(remover))
                removers.Remove(remover);
        }
    }

}
