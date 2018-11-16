using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Layouts;

namespace Stockmanager.Infrastructure.Logging.Nlog
{
    public static class LogModule
    {
        public static void AddStockManagerLogger(this IServiceCollection services)
        {
            var config = new LoggingConfiguration();
            ConfigurationItemFactory.Default.LayoutRenderers
                .RegisterDefinition("activityid", typeof(ActivityIdLayoutRenderer));

            var layout = new SimpleLayout("${longdate} ${activityid} ${level} ${message} ${all-event-properties:format=[key]=[value]:separator=|:includeCallerInformation=true} ${exception:format=toString,Data:maxInnerExceptionLevel=10}");
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log.txt",Layout = layout };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole"){Layout = layout};

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
            LogManager.Configuration = config;
            services.AddScoped<ILog>(a=> new NlogLog(LogManager.GetCurrentClassLogger()));
        }

    }
}
