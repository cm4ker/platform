using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.SimpleRealization;

namespace ZenPlatform.Tests
{
    public class ConfigurationFactory
    {
        public PComponent CreateDocumentComponent()
        {
            var c = new PComponent();
            c.Name = "Document";
            c.ComponentPath = "ZenPlatform.DocumentComponent.dll";

            var invoice = CreateInvoice(c);
            var contractor = CreateContractor(c);

            var contractorProperty = new PSimpleProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";


            invoice.Properties.Add(contractorProperty);

            return c;
        }

        public PSimpleObjectType CreateInvoice(PComponent component)
        {
            PSimpleObjectType invoice = component.CreateObject<PSimpleObjectType>("Invoice");

            invoice.TableName = "Invoices";
            invoice.Description = "Some description of document";

            var prop1 = new PSimpleProperty(invoice);
            prop1.Types.Add(new PDateTime());
            prop1.Name = "StartDate";

            var prop2 = new PSimpleProperty(invoice);
            prop2.Types.Add(new PNumeric());
            prop2.Name = "SomeNumber";

            invoice.Properties.Add(prop1);
            invoice.Properties.Add(prop2);


            return invoice;
        }

        public PSimpleObjectType CreateContractor(PComponent component)
        {
            PSimpleObjectType contractor = component.CreateObject<PSimpleObjectType>("Contractor"); ;
            contractor.TableName = "Contractors";
            contractor.Description = "Simple reference";

            var contractorNameProperty = new PSimpleProperty(contractor);
            contractorNameProperty.Types.Add(new PString());
            contractorNameProperty.Name = "Name";

            contractor.Properties.Add(contractorNameProperty);

            return contractor;
        }
    }
}
