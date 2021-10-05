using System;

namespace Aquila.Core.Contracts.Authentication
{
    public interface IUserManager
    {
        bool Authenticate(string userName, string password);
        IUser Create();
        void Delete(IUser user);
        IUser FindUserByName(string name);
        IUser Get(Guid id);
        void Update(IUser user);
    }
}