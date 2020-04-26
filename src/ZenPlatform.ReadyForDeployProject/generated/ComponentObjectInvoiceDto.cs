using System;
using ZenPlatform.Core;
using ZenPlatform.Core.Annotations;
using ZenPlatform.DocumentComponent;

namespace DefaultNamespace
{
    public class InvoiceDto
    {
        public Guid Id
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public Decimal SomeNumber
        {
            get;
            set;
        }

        public Guid Contractor_Ref
        {
            get;
            set;
        }
    }
}
