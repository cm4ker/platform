using System;
using System.Buffers.Binary;
using System.Dynamic;
using System.Globalization;
using System.Text;
using Grpc.Core;
using Newtonsoft.Json;
using PrototypeModelExchange.Client;

namespace PrototypeModelExchange
{
    /// <summary>
    /// Класс который показывает представление объекта
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IPlatformObject
    {
        /// <summary>
        /// Некий уникальный идентификатор, который позволяет определить сущность
        /// </summary>
        object Id { get; }

        /// <summary>
        /// Представление объекта
        /// </summary>
        string Present { get; }
    }

    public interface IDbObject
    {
    }

    /// <summary>
    /// Иммутабельный объект платформы. Получает данные из бд.
    /// Потоконезависимый
    /// </summary>
    /// <typeparam name="Value"></typeparam>
    public interface IReference<Value> : IPlatformObject
        where Value : IDbObject
    {
    }

    public static class InvoiceModelFactory
    {
        public static Invoice FromClientObject(InvoiceClient client)
        {
            return new Invoice(client.Dto);
        }
    }


    public interface IClientObject<Value> : IPlatformObject
        where Value : IDbObject
    {
    }

    public interface IModel<Value> : IPlatformObject
        where Value : IDbObject
    {
        IReference<Value> GetReference();
        IClientObject<Value> GetClientObject();
    }

    public class InvoiceDto : IDbObject
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public DateTime Date { get; set; }

        public Guid ContractorId { get; set; }

        public bool IsCool { get; set; }
    }

    public class InvoiceRef : IReference<InvoiceDto>
    {
        public InvoiceRef(InvoiceDto dto)
        {
            Id = dto.Id;
            Present = $"Invoice #{dto.Number} from {dto.Date}";
        }

        public object Id { get; }
        public string Present { get; }
    }

    public class ContranctorDto : IDbObject
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
    }

    public class ContractorRef : IReference<ContranctorDto>
    {
        private readonly ContranctorDto _dto;

        public ContractorRef(ContranctorDto dto)
        {
            _dto = dto;
        }

        public object Id { get; }
        public string Present { get; }
    }


    public class ContractorRefServerImpl : IReference<ContranctorDto>
    {
        public object Id { get; }
        public string Present { get; }


        public string Number { get; } // В ссылке можно только читать свойства
    }

    public class Invoice : IModel<InvoiceDto>
    {
        private readonly InvoiceDto _dto;

        public Invoice(InvoiceDto dto)
        {
            _dto = dto;
        }

        public object Id
        {
            get => _dto.Id;
        }

        public string Number
        {
            get => _dto.Number;
            set => _dto.Number = value;
        }

        public DateTime Date
        {
            get => _dto.Date;
            set => _dto.Date = value;
        }

        public bool IsCool
        {
            get => _dto.IsCool;
            set => _dto.IsCool = value;
        }

        public ContractorRef Contractor
        {
            get
            {
                //  На самом деле мы получаем контрагента по менеджеру
                return new ContractorRef();
            }
            set => _dto.ContractorId = (Guid) value.Id;
        }

        public string Present
        {
            get => $"Invoice #{_dto.Number} from {_dto.Date}";
        }

        public bool Check()
        {
            return IsCool;
        }

        public IReference<InvoiceDto> GetReference()
        {
            return new InvoiceRef(_dto);
        }

        public IClientObject<InvoiceDto> GetClientObject()
        {
            return new InvoiceClient(_dto);
        }

        public void Save()
        {
            Console.WriteLine("I'm store the object");
        }
    }

    public class InvoiceClient : IClientObject<InvoiceDto>
    {
        private readonly InvoiceDto _dto;

        public InvoiceClient()
        {
            _dto = new InvoiceDto();
        }

        public InvoiceClient(InvoiceDto dto)
        {
            _dto = dto;
        }

        internal InvoiceDto Dto => _dto;

        public object Id
        {
            get => _dto.Id;
        }

        public string Number
        {
            get => _dto.Number;
            set => _dto.Number = value;
        }

        public DateTime Date
        {
            get => _dto.Date;
            set => _dto.Date = value;
        }

        public bool IsCool
        {
            get => _dto.IsCool;
            set => _dto.IsCool = value;
        }

        public ContractorRef Contractor
        {
            get
            {
                //  На самом деле мы получаем контрактора по менеджеру
                return new ContractorRef();
            }
            set => _dto.ContractorId = (Guid) value.Id;
        }

        public string Present
        {
            get => $"Invoice #{_dto.Number} from {_dto.Date}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Main2();
            return;

            ServerRPC server = new ServerRPC();
            server.Start();

            Client.Model model = new Client.Model
            {
                Count = 10,
                Name = "preved",
            };


            var result = model.DoSomething("medved");

            Console.WriteLine($"Result: {result}");

            Console.WriteLine(JsonConvert.SerializeObject(model));

            Console.ReadLine();
        }

        static void Main2()
        {
            var settings = new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.All};
            var i = GetInvoice();
            // Шаг 1. Получить InvoiceClient и отправить его на клиент
            var clientObject = i.GetClientObject(); // this is completely serializeble
            Console.WriteLine($"Was contractor ref: {i.Contractor.Id}");
            // Эти данные мы можем отправить по транспортному протоколу
            var transferData = JsonConvert.SerializeObject(clientObject, settings);

            //... transfering

            //restore object

            var objectOnCclient =
                JsonConvert.DeserializeObject<InvoiceClient>(transferData, settings);

            // Шаг 2. Поменять Какие-то данные на InvoiceClient и отправить его на сервер + вызвать метод
            objectOnCclient.IsCool = true;

            objectOnCclient.Contractor = new ContractorRef();

            //transfer it again

            // Эти данные мы можем отправить по транспортному протоколу
            transferData = JsonConvert.SerializeObject(objectOnCclient, settings);

            var newObject = JsonConvert.DeserializeObject<InvoiceClient>(transferData, settings);

            var serverInvoiceObject = InvoiceModelFactory.FromClientObject(newObject);

            // Шаг 3 PROFIT
            Console.WriteLine($"Now contractor ref: {serverInvoiceObject.Contractor.Id}");
            if (serverInvoiceObject.Check())
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fault");
            }
        }


        public static Invoice GetInvoice()
        {
            var dto = new InvoiceDto();

            dto.Id = Guid.NewGuid();
            dto.Date = DateTime.Now;
            dto.Number = "000000001";
            dto.ContractorId = Guid.NewGuid();

            return new Invoice(dto);
        }
    }
}