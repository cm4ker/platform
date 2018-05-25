using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZenPlatform.ApplicationServer
{
    class Program
    {
        static void Main()
        {
            WeakReference weakRef = new WeakReference(new Data());
            Console.WriteLine("run GC");

            GC.Collect();

            Thread.Sleep(1000);

            Console.WriteLine("IsAlive = {0}", weakRef.IsAlive);
            Console.ReadKey();
        }
    }
    
    public class Data
    {
        ~Data()
        {
            Console.WriteLine("Destructor call");
        }
    }

}
