using System;
using MemCache.Server;

namespace MemCache.ServerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new CacheServer();
            server.Start("tcp://*:4632");
            Console.WriteLine("MemCache server has started");
            Console.WriteLine("Press Enter to stop");
            Console.ReadLine();
        }
    }
}
