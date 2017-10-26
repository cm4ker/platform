using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DocumentComponent;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class EntityGeneratorTest
    {
        [TestMethod]
        public void DcoumentCodeGenerate()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();
            PSimpleObjectType o = new PSimpleObjectType("Test");

            var prop1 = new PProperty(o);
            prop1.Types.Add(new PDateTime());
            prop1.Name = "StartDate";
            o.Propertyes.Add(prop1);

            deg.GenerateEntityClass(null, o);
        }
    }
}