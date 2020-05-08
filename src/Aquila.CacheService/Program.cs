using System;
using ServiceStack.Redis;


namespace Aquila.CacheService
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new RedisManagerPool("localhost:6379");
            using (var client = manager.GetClient())
            {
//                client.Set<int>("foo", 1);
//                var n = client.Get<int>("foo") + 1;

                client.Set("foo", new fieldedPoco() {id = 1, value = "fielded"});
                var c = client.As<Mypoco>();
                c.Store(new Mypoco() {Id = 1, value = "hello"});
                Console.WriteLine(c.GetById(1).value);
                Console.WriteLine(client.Get<fieldedPoco>("foo").value);
            }
        }
    }

    public class Mypoco
    {
        public int Id { get; set; }
        public string value { get; set; }
    }

    public class fieldedPoco
    {
        public int id;

        public string value { get; set; }
    }
}