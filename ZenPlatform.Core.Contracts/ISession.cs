using System;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Sessions
{
    public interface ISession: IRemovable
    {
        Guid Id { get; }

        IUser User { get; }

        //DataContext DataContext { get; }
        void SetSessionParameter(string key, object value);
        object GetSessionParameter(string key, object value);

        IEnvironment Environment { get; }

        void Close();
    }
}