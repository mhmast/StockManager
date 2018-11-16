using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Stockmanager.Infrastructure.Logging;
using Stockmanager.Infrastructure.WebApi;

namespace Stockmanager.StockApi.Service
{
    internal class Startup
    {
        private readonly ILog _log;
        private readonly IConfiguration _config;

        public Startup(ILog log, IConfiguration config)
        {
            _log = log;
            _log.LogInfo("Application starting");
            _config = config;
            foreach (var keyValuePair in _config.AsEnumerable())
            {
                log.LogInfo($"{keyValuePair.Key}=${{{keyValuePair.Key}}}", keyValuePair.Value);
            }
        }
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(typeof(StockController).Assembly);
            services.AddSwagger(_config,_log);
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
            app.UseSwagger(_config,_log);
        }
    }
}
