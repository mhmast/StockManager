using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Stockmanager.Infrastructure.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Stockmanager.Infrastructure.WebApi
{
    public static class SwaggerModule
    {
        public static void AddSwagger(this IServiceCollection services, IConfiguration config, ILog log)
        {
            if (config.GetValue<bool>("Swagger:EnableSwagger"))
            {
                log.LogInfo("Adding swagger support");
                services.AddSwaggerGen(o =>
                {
                    var pathToDoc = config["Swagger:SwaggerDocsFileName"];
                    log.LogInfo("Swagger docs file: {Path}", pathToDoc);

                    var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, pathToDoc);
                    o.IncludeXmlComments(filePath);
                    o.DescribeAllEnumsAsStrings();
                    var info = new Info
                    {
                        Title = config["Swagger:ApiName"],
                        Version = "v1",
                        Description = config["Swagger:ApiDescription"],
                        TermsOfService = "None"
                    };
                    log.LogInfo("Swagger config: {Config} ", JsonConvert.SerializeObject(info));
                    o.SwaggerDoc("v1", info);
                });
            }
        }

        public static void UseSwagger(this IApplicationBuilder app, IConfiguration config, ILog logger)
        {
            if (config.GetValue<bool>("Swagger:EnableSwagger"))
            {
                logger.LogInfo("Enabeling swagger");
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                });

                app.UseSwaggerUI(c =>
                {
                    var prefix = config["Swagger:VirtualAppPath"];
                    c.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "V1 Docs");
                });
            }
        }
    }
}
