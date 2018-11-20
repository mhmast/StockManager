using System.Diagnostics;
using System.Text;
using NLog;
using NLog.LayoutRenderers;

namespace Jarvis.Infrastructure.Logging.Nlog
{
    [LayoutRenderer("activityid")]
    public class ActivityIdLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(Trace.CorrelationManager.ActivityId.ToString());
        }
    }
}
