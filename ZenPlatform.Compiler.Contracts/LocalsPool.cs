using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public class LocalsPool
    {
        private readonly IEmitter _emitter;

        private readonly List<(IType type, ILocal local)> _localsPool =
            new List<(IType, ILocal)>();

        public sealed class PooledLocal : IDisposable
        {
            public ILocal Local { get; private set; }
            private readonly LocalsPool _parent;
            private readonly IType _type;

            public PooledLocal(LocalsPool parent, IType type, ILocal local)
            {
                Local = local;
                _parent = parent;
                _type = type;
            }

            public void Dispose()
            {
                if (Local == null)
                    return;
                _parent._localsPool.Add((_type, Local));
                Local = null;
            }
        }

        public LocalsPool(IEmitter emitter)
        {
            _emitter = emitter;
        }

        public PooledLocal GetLocal(IType type)
        {
            for (var c = 0; c < _localsPool.Count; c++)
            {
                if (_localsPool[c].type.Equals(type))
                {
                    var rv = new PooledLocal(this, type, _localsPool[c].local);
                    _localsPool.RemoveAt(c);
                    return rv;
                }
            }

            return new PooledLocal(this, type, _emitter.DefineLocal(type));
        }
    }
}