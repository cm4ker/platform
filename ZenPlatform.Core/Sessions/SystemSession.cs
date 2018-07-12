using ZenPlatform.Core.Environment;

namespace ZenPlatform.Core.Sessions
{
    /// <summary>
    /// Системная сессия, нужна для того, чтобы выполнять операции, которые не может выполнить пользователь
    /// Например обновление схемы данных
    /// </summary>
    public class SystemSession : Session<PlatformEnvironment>
    {
        public SystemSession(PlatformEnvironment env, int id) : base(env, id)
        {
        }
        
        
    }
}