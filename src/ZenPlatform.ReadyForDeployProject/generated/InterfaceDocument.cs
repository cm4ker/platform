using System;
using ZenPlatform.Core;
using ZenPlatform.Core.Annotations;
using ZenPlatform.DocumentComponent;

namespace DefaultNamespace
{
    public class DocumentInterface
    {
        public DocumentInterface(Session session)
        {
            Session = session;
        }

        private Session Session
        {
            get;
        }

        public DocumentEntityManager<InvoiceEntity> Invoice
        {
            get
            {
                return new DocumentEntityManager<InvoiceEntity>(Session);
            }
        }

        public DocumentEntityManager<ContractorEntity> Contractor
        {
            get
            {
                return new DocumentEntityManager<ContractorEntity>(Session);
            }
        }
    }
}
