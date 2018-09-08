using ZenPlatform.Core.Sessions;
using ZenPlatform.EntityComponent;

namespace DefaultNamespace
{
    public class DocumentInterface
    {
        public DocumentInterface(UserSession session)
        {
            Session = session;
        }

        private UserSession Session
        {
            get;
        }

        public DocumentEntityManager<ТестоваяСущностьEntity> ТестоваяСущность
        {
            get
            {
                return new DocumentEntityManager<ТестоваяСущностьEntity>(Session);
            }
        }
    }
}