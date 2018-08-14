using ZenPlatform.Core.Authentication;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Sessions
{
    public interface ISession
    {
        int Id { get; }

        DataContext GetDataContext();
        UserManager GetUserManager();
    }
}