using System;

namespace ZenPlatform.Core.Authentication
{
    public interface IUserManager
    {
        bool Authenticate(string userName, string password);
        User Create();
        void Delete(User user);
        IUser FindUserByName(string name);
        IUser Get(Guid id);
        void Update(User user);
    }
}