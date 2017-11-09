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
        private PSimpleObjectType CreateInvoice()
        {
            PSimpleObjectType invoice = new PSimpleObjectType("Invoice"); ;

            var prop1 = new PProperty(invoice);
            prop1.Types.Add(new PDateTime());
            prop1.Name = "StartDate";

            var prop2 = new PProperty(invoice);
            prop2.Types.Add(new PNumeric());
            prop2.Name = "SomeNumber";

            invoice.Propertyes.Add(prop1);
            invoice.Propertyes.Add(prop2);


            return invoice;
        }

        private PSimpleObjectType CreateContractor()
        {
            PSimpleObjectType contractor = new PSimpleObjectType("Contractor");

            var contractorNameProperty = new PProperty(contractor);
            contractorNameProperty.Types.Add(new PString());
            contractorNameProperty.Name = "Name";

            contractor.Propertyes.Add(contractorNameProperty);

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


            invoice.Propertyes.Add(contractorProperty);

            var nodeDto = deg.GenerateDtoClass(invoice);
            Console.WriteLine(nodeDto.ToString());
        }


        [TestMethod]
        public void DcoumentComponentGenerateEntity()
        {
            DocumentEntityGenerator deg = new DocumentEntityGenerator();

            var invoice = CreateInvoice();
            var contractor = CreateContractor();

            var contractorProperty = new PProperty(invoice);
            contractorProperty.Types.Add(contractor);
            contractorProperty.Name = "Contractor";


            invoice.Propertyes.Add(contractorProperty);

            var nodeEntity = deg.GenerateEntityClass(invoice);

            Console.WriteLine(nodeEntity.ToString());
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


            invoice.Propertyes.Add(contractorProperty);


            var helpers = deg.GenerateHelpersForEntity();
            Console.WriteLine(helpers.ToString());

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

        public string SomePropertyInDto2
        {
            get { return _someDto2.SomePropInDto2; }
            set
            {
                _someDto2.SomePropInDto2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SomeEntity1 : INotifyPropertyChanged
    {
        private SomeDto1 _dto;
        private SomeEntity2 _someEntity2;
        private Session _session;

        public SomeEntity1(SomeDto1 dto, Session session)
        {
            _dto = dto;
            _session = session;

        }

        public string Prop1
        {
            get { return _dto.Prop1; }
            set
            {
                _dto.Prop1 = value;
                OnPropertyChanged();
            }
        }


        public SomeEntity2 SomeEntity
        {
            get
            {
                throw new Exception("NEED REALIZATION"); /* _session.SomeManager.Load(_dto.SomeEntity_Ref);
                                                         �������� SomeManager ����������� ���
                                                         public static SomeEntityManager SomeManager(this Session session)
                                                         { 
                                                            return (SomeEntityManager)session.GetManager("SomeEntityMamanger");
                                                         }*/
                return _someEntity2;
            }
            set
            {
                _someEntity2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}