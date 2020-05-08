using System.Collections.Generic;

namespace Aquila.Compiler
{
    public static class ExtensionManager
    {
        public static readonly Dictionary<string, IAstLanguageExtension> Managers =
            new Dictionary<string, IAstLanguageExtension>();

        public static void Register<T>(string extensionKey, T instance) where T : IAstLanguageExtension
        {
            Managers.Add(extensionKey, instance);
        }
    }
}