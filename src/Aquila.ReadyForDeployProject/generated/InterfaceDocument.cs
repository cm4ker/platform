using System;
using Aquila.Core;
using Aquila.Core.Annotations;
using Aquila.DocumentComponent;

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
