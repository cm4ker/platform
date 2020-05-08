using System;
using Aquila.Core;
using Aquila.Core.Annotations;
using Aquila.DocumentComponent;

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
