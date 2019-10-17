using System;

namespace ZenPlatform.Core.Authentication
{
    public interface IUserManager
    {
        bool Authenticate(string userName, string password);
        PlatformUser Create();
        void Delete(PlatformUser user);
        IUser FindUserByName(string name);
        IUser Get(Guid id);
        void Update(PlatformUser user);
    }
}