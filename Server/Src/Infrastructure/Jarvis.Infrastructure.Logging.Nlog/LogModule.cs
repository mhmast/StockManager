using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Layouts;

namespace Jarvis.Infrastructure.Logging.Nlog
{
    public static class LogModule
    {
        public static void AddJarvisLogger(this IServiceCollection services)
        {
            var config = new LoggingConfiguration();
            ConfigurationItemFactory.Default.LayoutRenderers
                .RegisterDefinition("activityid", typeof(ActivityIdLayoutRenderer));

           // var layout = new SimpleLayout("${longdate} ${activityid} ${level} ${message} ${all-event-properties:format=[key]=[value]:separator=|:includeCallerInformation=true} ${exception:format=toString,Data:maxInnerExceptionLevel=10}");
            var layout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("longdate",new SimpleLayout("${longdate}")),
                    new JsonAttribute("activityid",new SimpleLayout("${activityid}")),
                    new JsonAttribute("level",new SimpleLayout("${level}")),
                    new JsonAttribute("message",new SimpleLayout("${message}")),
                    //new JsonAttribute("properties",new SimpleLayout("${all-event-properties:format=[key]=[value]:separator=|:includeCallerInformation=true}")),
                    new JsonAttribute("exception",new SimpleLayout("${exception:format=toString,Data:maxInnerExceptionLevel=10}"))

                },
                IncludeAllProperties = true
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole") { Layout = layout };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
            Console.WriteLine("Registering NLOG as ILog");
            services.AddSingleton<ILog>(a => new NlogLog(LogManager.GetCurrentClassLogger()));
        }

        public static void UseJarvisLogger(this IApplicationBuilder builder)
        {
            builder.Use(async (request, next) =>
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                await next();
            });
        }
    }
}
