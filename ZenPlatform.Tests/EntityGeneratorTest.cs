using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Core;
using ZenPlatform.Core.Entity;
using ZenPlatform.DataComponent;
using ZenPlatform.DocumentComponent;
using ZenPlatform.Tests.Annotations;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class EntityGeneratorTest
    {
        private PComponent DocumentComponent()
        {
            var c = new PComponent();
            c.Name = "Document";

            new DocumnetComponent(c);

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
        public void DocumentComponentGenerateDto()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var invoice = CreateInvoice();
            var contractor = CreateContractor();

            var contractorProperty = new PProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";


            invoice.Properties.Add(contractorProperty);

            var nodeDto = deg.GenerateDtoClass(invoice);
            Console.WriteLine(nodeDto.ToString());
        }


        [TestMethod]
        public void DcoumentComponentGenerateEntity()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var invoice = CreateInvoice();
            var contractor = CreateContractor();

            var component = DocumentComponent();

            invoice.OwnerComponent = component;
            contractor.OwnerComponent = component;

            var contractorProperty = new PProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";


            invoice.Properties.Add(contractorProperty);

            var invoiceClass = deg.GenerateEntityClass(invoice);
            var contractorClass = deg.GenerateEntityClass(contractor);

            Console.WriteLine(contractorClass.ToString());
            Console.WriteLine(invoiceClass.ToString());
        }

        [TestMethod]
        public void DocumentSessionExtensionGeneration()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var invoice = CreateInvoice();
            var contractor = CreateContractor();

            var component = DocumentComponent();

            var contractorProperty = new PProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";

            invoice.Properties.Add(contractorProperty);

            component.RegisterObject(invoice);
            component.RegisterObject(contractor);

            var extension = deg.GenerateExtension(component);
            var inface = deg.GenerateInterface(component);

            Console.WriteLine(extension.ToString());
            Console.WriteLine(inface.ToString());
        }


        [TestMethod]
        public void DcoumentComponentGenerateHelpers()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var invoice = CreateInvoice();
            var contractor = CreateContractor();

            var contractorProperty = new PProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";


            invoice.Properties.Add(contractorProperty);


            var helpers = deg.GenerateHelpersForEntity();
            Console.WriteLine(helpers.ToString());

        }
    }
}