using Avalonia.Controls;
using DryIoc;

namespace ZenPlatform.ServerManagementTool
{
    public class IoC
    {
        static IContainer _container;

        static IoC()
        {
            _container = new Container();
        }

        public static T Resolve<T>() => _container.Resolve<T>();

        public static IContainer Container => _container;
    }
}