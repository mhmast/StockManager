using System;
using Microsoft.AspNetCore.Hosting;
using Stockmanager.Infrastructure.Logging.Nlog;

namespace Stockmanager.StockApi.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(c=>c.AddStockManagerLogger())
                .UseKestrel(o=>o.ListenAnyIP(5000))
                .UseStartup<Startup>()
                .Build().Start();
            Console.Read();
        }
    }
}
