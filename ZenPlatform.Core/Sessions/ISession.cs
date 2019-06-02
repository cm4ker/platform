using System;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Sessions
{
    public interface ISession
    {
        Guid Id { get; }

        IUser User { get; }

        DataContext DataContext { get; }
        void SetSessionParameter(string key, object value);
        object GetSessionParameter(string key, object value);

        IEnvironment Environment { get; }
    }
}