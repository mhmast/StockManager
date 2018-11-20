using Jarvis.Infrastructure.Logging;
using Jarvis.Infrastructure.Logging.Nlog;
using Jarvis.Infrastructure.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jarvis.Api.Service
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseJarvisLogger();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger(_config,_log);
        }
    }
}
