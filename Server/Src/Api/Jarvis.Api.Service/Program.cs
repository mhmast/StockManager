using Jarvis.Infrastructure.Logging.Nlog;
using Microsoft.AspNetCore.Hosting;

namespace Jarvis.Api.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var webHostBuilder = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args);
            webHostBuilder
                .ConfigureServices(c=>c.AddJarvisLogger())
                .UseKestrel(o=>o.ListenAnyIP(5000))
                .UseStartup<Startup>()
                .Build().Run();
        }
    }
}
