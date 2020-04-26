using System;
using System.Collections.Generic;

namespace ZenPlatform.Pidl.Xaml
{
    public class PlatformExtensionResolverImpl : IPlatformExtensionsResolver
    {
        private Dictionary<(string, string), Type> _dict = new Dictionary<(string, string), Type>();

        public Type Resolve(string platformNamespace, string name)
        {
            if (_dict.TryGetValue((platformNamespace, name), out var type))
            {
                return type;
            }

            return null;
        }

        public void Register(string platformNamespace, string name, Type type)
        {
            _dict.TryAdd((platformNamespace, name), type);
        }
    }
}