namespace ZenPlatform.ServerClientShared.DI
{
    public interface IDependencyResolver
    {
        T Resolve<T>();

        IDependencyScope BeginScope();
    }
}