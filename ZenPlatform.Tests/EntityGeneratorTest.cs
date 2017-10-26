using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DataComponent;
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

            var prop2 = new PProperty(o);
            prop2.Types.Add(new PNumeric());
            prop2.Name = "SomeNumber";

            o.Propertyes.Add(prop1);
            o.Propertyes.Add(prop2);

            deg.GenerateEntityClass(null, o);
        }
    }


    public class SomeDto1 : EntityBase
    {
        public string Prop1 { get; set; }

        public Guid SomeEntity_Ref { get; set; }
    }

    public class SomeDto2 : EntityBase
    {
        public string SomePropInDto2 { get; set; }
    }

    public class SomeEntity2 : INotifyPropertyChanged
    {
        private SomeDto2 _someDto2;

        public SomeEntity2(SomeDto2 dto)
        {
            _someDto2 = dto;
        }

        internal object Key
        {
            get { return _someDto2.Key; }
        }

        public string SomePropertyInDto2
        {
            get { return _someDto2.SomePropInDto2; }
            set { _someDto2.SomePropInDto2 = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SomeEntity1 : INotifyPropertyChanged
    {
        private SomeDto1 _dto;
        private SomeEntity2 _someEntity2;

        public SomeEntity1(SomeDto1 dto)
        {
            _dto = dto;
        }

        public string Prop1
        {
            get { return _dto.Prop1; }
            set { _dto.Prop1 = value; }
        }

        internal object Key
        {
            get { return _dto.Key; }
        }

        public SomeEntity2 SomeEntity
        {
            get
            {
                _someEntity2 =
                    _someEntity2 ??
                    throw new Exception("NEED REALIZATION"); // Data.SomeManager.Load(_dto.SomeEntity_Ref);
                return _someEntity2;
            }
            set
            {
                _dto.SomeEntity_Ref = (Guid) value.Key;
                _someEntity2 = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}