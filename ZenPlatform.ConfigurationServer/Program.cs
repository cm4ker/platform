using System;
using System.ServiceModel.Channels;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using NetMQ;
using NetMQ.Sockets;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.ConfigurationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Своеобразный план, как мы работаем:
             * 1) Необходимо загрузить конфигурацию
             * 2) При приёмке сообщения, его нужно зароутить на нужный компонент => нужен роутер
             * 3) Отправить ответ на конечный узел
             */

            using (var server = new ResponseSocket("@tcp://localhost:5556")) // bind
            using (var client = new RequestSocket(">tcp://localhost:5556")) // connect
            {
                byte[] data;

                var test = new Random(DateTime.Now.Second).Next(10);
                
                Console.ReadKey();
            }
        }
    }


    public interface ITransport
    {
        void Send(Message msg);
    }
}