using System;
using Avalonia;
using ZenPlatform.Avalonia.Wrapper;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Network;

namespace ZenPlatform.ThinClient.FakeAssembly
{
    public abstract class EntryPoint
    {
        public static void Main(object[] args)
        {
            GlobalScope.AddCommand("test", testUI);
        }

        public static void testUI()
        {
            IProtocolClient client = GlobalScope.Client;
            
        }
    }

    internal class StoreEditorForm : UXForm
    {
    }
}