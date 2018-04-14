using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Core;
using ZenPlatform.Core.Entity;
using ZenPlatform.CSharpCodeBuilder;
using ZenPlatform.DataComponent;
using ZenPlatform.DocumentComponent;
using ZenPlatform.Tests.Annotations;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class EntityGeneratorTest
    {


        [TestMethod]
        public void DocumentComponentGenerateDto()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();
            var com = ConfigurationFactory.CreateDocumentComponent();
            var invoice = com.Objects.ToArray()[0];
            var contractor = com.Objects.ToArray()[1];


            var nodeDto = deg.GenerateDtoClass(invoice);
            Console.WriteLine(nodeDto.ToString());
        }


        [TestMethod]
        public void DcoumentComponentGenerateEntity()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var com = ConfigurationFactory.CreateDocumentComponent();
            var invoice = com.Objects.ToArray()[0];
            var contractor = com.Objects.ToArray()[1];


            var invoiceClass = deg.GenerateEntityClass(invoice);
            var contractorClass = deg.GenerateEntityClass(contractor);

            Console.WriteLine(contractorClass.ToString());
            Console.WriteLine(invoiceClass.ToString());
        }

        [TestMethod]
        public void DocumentSessionExtensionGeneration()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var com = ConfigurationFactory.CreateDocumentComponent();

            var extension = deg.GenerateExtension(com);
            var inface = deg.GenerateInterface(com);

            Console.WriteLine(extension.ToString());
            Console.WriteLine(inface.ToString());
        }


        [TestMethod]
        public void DcoumentComponentGenerateHelpers()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var helpers = deg.GenerateHelpersForEntity();
            Console.WriteLine(helpers.ToString());

        }

        [TestMethod]
        public void GenerateSolutionTest()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var com = ConfigurationFactory.CreateDocumentComponent();

            CodeBuilder cb = new CodeBuilder();
            cb.Generate(deg, com);

        }
    }
}