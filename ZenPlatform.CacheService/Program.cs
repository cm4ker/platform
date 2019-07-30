using System;
using ServiceStack.Redis;


namespace ZenPlatform.CacheService
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new RedisManagerPool("localhost:6379");
            using (var client = manager.GetClient())
            {
                client.Set("foo", "bar");
            }
        }
    }
}