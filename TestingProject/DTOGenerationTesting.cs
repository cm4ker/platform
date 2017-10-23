using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.EntityGenerator;

namespace SqlPlusDbSync.UnitTest
{
    [TestClass]
    public class DTOGenerationTesting
    {
        private DTOGenerator _gen;

        public DTOGenerationTesting()
        {
            _gen = new DTOGenerator(TypeFabric());
        }

        private List<PObjectType> TypeFabric()
        {
            var result = new List<PObjectType>();

            var item = new PSimpleObjectType("SimpleObject");

            var prop1 = new PProperty(item)
            {
                Name = "HereColName",
                Alias = "RenaimingPart"
            };

            prop1.Types.Add(new PNumeric());
            prop1.Types.Add(new PBoolean());
            prop1.Types.Add(new PDateTime());
            prop1.Types.Add(new PString());
            prop1.Types.Add(new PGuid());
            prop1.Types.Add(new PSimpleObjectType("SomeSimpleObject2"));

            item.Propertyes.Add(prop1);

            result.Add(item);
            return result;
        }

        [TestMethod]
        public void SimpleDTOGeneration()
        {
            Console.WriteLine(_gen.GetModuleText());
        }

        public void ComplexDTOGeneration()
        {

        }
    }
}
