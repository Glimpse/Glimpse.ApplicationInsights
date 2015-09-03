using System;
using System.Diagnostics;

using Glimpse.ApplicationInsights.Model;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Glimpse.Core.Message;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace Glimpse.ApplicationInsights
{
    public class GlimpseTelemetryChannel : ITelemetryChannel
    {
        [ThreadStatic]
        private static Stopwatch fromLastWatch;

        private IMessageBroker messageBroker;
        internal Func<IExecutionTimer> TimerStrategy { get; set; }

        internal IMessageBroker MessageBroker
        {
            get { return messageBroker ?? (messageBroker = GlimpseConfiguration.GetConfiguredMessageBroker()); }
            set { messageBroker = value; }
        }

        public GlimpseTelemetryChannel()
        {
            MessageBroker = GlimpseConfiguration.GetConfiguredMessageBroker();
            TimerStrategy = GlimpseConfiguration.GetConfiguredTimerStrategy();
        }

        public bool? DeveloperMode
        {
            get;
            set;
        }

        public string EndpointAddress
        {
            get;
            set;
        }

        public void Flush()
        {

        }

        public void Send(ITelemetry item)
        {
            var timer = TimerStrategy();

            if (timer == null || MessageBroker == null)
            {
                return;
            }

            if (item is DependencyTelemetry)
            {
                var dependency = item as DependencyTelemetry;
                var timelineMessage = new DependencyTelemetryTimelineMessage(dependency);
                timelineMessage.Offset = timer.Point().Offset.Subtract(dependency.Duration);
                MessageBroker.Publish(timelineMessage);
            }

            if (item is TraceTelemetry)
            {
                TraceTelemetry t = new TraceTelemetry();

                var model = new TraceMessage
                {
                    Category = "Application Insights",
                    Message = t.SeverityLevel == null ? t.Message : t.SeverityLevel + ": " + t.Message,
                    FromFirst = timer.Point().Offset,
                    FromLast = CalculateFromLast(timer),
                    IndentLevel = 0
                };

                MessageBroker.Publish(model);
            }

            messageBroker.Publish(item);
        }

        public void Dispose()
        {
            //do nothing
        }

        private TimeSpan CalculateFromLast(IExecutionTimer timer)
        {
            if (fromLastWatch == null)
            {
                fromLastWatch = Stopwatch.StartNew();
                return TimeSpan.FromMilliseconds(0);
            }

            // Timer started before this request, reset it
            if (DateTime.Now - fromLastWatch.Elapsed < timer.RequestStart)
            {
                fromLastWatch = Stopwatch.StartNew();
                return TimeSpan.FromMilliseconds(0);
            }

            var result = fromLastWatch.Elapsed;
            fromLastWatch = Stopwatch.StartNew();
            return result;
        }

    }
}