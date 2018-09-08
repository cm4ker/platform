using ZenPlatform.Core.Sessions;

namespace DefaultNamespace
{
    public static class DocumentComponentSessionExtension
    {
        public static DocumentInterface Entity(this UserSession session)
        {
            return new DocumentInterface(session);
        }
    }
}