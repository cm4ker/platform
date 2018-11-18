using System.Collections.Generic;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Sessions
{
    /// <summary>
    /// Абстракция сессии
    /// </summary>
    public abstract class Session<TPlatformEnv> : ISession
        where TPlatformEnv : PlatformEnvironment
    {
        protected Session(TPlatformEnv env, int id)
        {
            Environment = env;
            Id = id;
            UserManager = new UserManager(this);
            DataContextManger =
                new DataContextManger(env.StartupConfig.DatabaseType, env.StartupConfig.ConnectionString);
        }

        public int Id { get; }

        protected DataContextManger DataContextManger { get; }
        internal TPlatformEnv Environment { get; }

        protected UserManager UserManager { get; }

        public UserManager GetUserManager()
        {
            return UserManager;
        }

        public DataContext GetDataContext()
        {
            return DataContextManger.GetContext();
        }
    }
}