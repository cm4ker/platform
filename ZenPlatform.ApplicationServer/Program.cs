using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZenPlatform.ApplicationServer
{
    /*
     * Проект сервера приложений
     * Сервер по
     */
    class Program
    {
        static volatile int value = 1;

        static void Main()
        {

            var t1 = Task.Run(() =>
            {
                if (value == 1)
                {
                    Thread.Sleep(2000);
                    value = 2;
                }
            });

            var t2 = Task.Run(() =>
            {
                int i = 1;
                Thread.Sleep(1);
                value = i + i + i;
                // Thread.VolatileWrite(ref value, 3);
            });

            Task.WaitAll(t1, t2);
            Console.WriteLine(value);
            Console.ReadKey();
        }
    }
}
