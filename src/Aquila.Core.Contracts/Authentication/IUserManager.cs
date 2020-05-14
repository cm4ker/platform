using System;

namespace Aquila.Core.Authentication
{
    public interface IUserManager
    {
        bool Authenticate(string userName, string password);
        IPlatformUser Create();
        void Delete(IPlatformUser user);
        IUser FindUserByName(string name);
        IUser Get(Guid id);
        void Update(IPlatformUser user);
    }
}