using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Core;
using ZenPlatform.DocumentComponent;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class ConfigurationTesting
    {
        private PComponent DocumentComponent()
        {
            var c = new PComponent();
            c.Name = "Document";


            var invoice = CreateInvoice();
            var contractor = CreateContractor();

            var contractorProperty = new PProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";


            invoice.Properties.Add(contractorProperty);

            c.RegisterObject(invoice);
            c.RegisterObject(contractor);

            return c;
        }

        private PSimpleObjectType CreateInvoice()
        {


            PSimpleObjectType invoice = new PSimpleObjectType("Invoice"); ;

            var prop1 = new PProperty(invoice);
            prop1.Types.Add(new PDateTime());
            prop1.Name = "StartDate";

            var prop2 = new PProperty(invoice);
            prop2.Types.Add(new PNumeric());
            prop2.Name = "SomeNumber";

            invoice.Properties.Add(prop1);
            invoice.Properties.Add(prop2);


            return invoice;
        }

        private PSimpleObjectType CreateContractor()
        {
            PSimpleObjectType contractor = new PSimpleObjectType("Contractor");

            var contractorNameProperty = new PProperty(contractor);
            contractorNameProperty.Types.Add(new PString());
            contractorNameProperty.Name = "Name";

            contractor.Properties.Add(contractorNameProperty);

            return contractor;
        }

        [TestMethod]
        public void SimpleTest()
        {
            PlatformEnvironment e = new PlatformEnvironment();

            var pcomponent = DocumentComponent();

            e.RegisterEntity(new EntityDefinition(pcomponent.Objects.ToArray()[1], typeof(ContractorEntity), typeof(ContractorDto)));
            e.RegisterEntity(new EntityDefinition(pcomponent.Objects.ToArray()[0], typeof(InvoiceEntity), typeof(InvoiceDto)));

            e.RegisterManager(typeof(ContractorEntity), new DocumentManager());
            e.RegisterManager(typeof(InvoiceEntity), new DocumentManager());

            var session = e.CreateSession();

            var invoice = session.Document().Invoice.Load("someKey");
        }
    }
}
