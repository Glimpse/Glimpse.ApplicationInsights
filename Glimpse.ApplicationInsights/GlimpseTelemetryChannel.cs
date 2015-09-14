using System;
using System.Diagnostics;

using Glimpse.ApplicationInsights.Model;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Glimpse.Core.Message;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Glimpse.ApplicationInsights
{
    /// <summary>
    /// Telemetry channel that will send Application Insights telemetry
    /// to Glimpse message broker and to the Application Insights channel.
    /// </summary>
    public class GlimpseTelemetryChannel : ITelemetryChannel
    {
        [ThreadStatic]
        private static Stopwatch fromLastWatch;

        /// <summary>
        /// Gets the channel from ApplicationInsights.config file that
        /// will send the telemetry to Application Insights.
        /// </summary>
        public ITelemetryChannel ApplicationInsightsChannel { get; private set; }

        private IMessageBroker messageBroker;

        internal Func<IExecutionTimer> TimerStrategy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlimpseTelemetryChannel" /> class.
        /// </summary>
        public GlimpseTelemetryChannel()
        {
            this.MessageBroker = GlimpseConfiguration.GetConfiguredMessageBroker();
            this.TimerStrategy = GlimpseConfiguration.GetConfiguredTimerStrategy();
        }

        /// <summary>
        /// Gets or sets a value indicating whether developer mode 
        /// of the telemetry transmission is enabled.
        /// </summary>
        public bool? DeveloperMode
        {
            get
            {
                if (this.ApplicationInsightsChannel != null)
                {
                    return this.ApplicationInsightsChannel.DeveloperMode;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (this.ApplicationInsightsChannel != null)
                {
                    this.ApplicationInsightsChannel.DeveloperMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the HTTP address where the telemetry is sent.
        /// </summary>
        public string EndpointAddress
        {
            get
            {
                if (this.ApplicationInsightsChannel != null)
                {
                    return this.ApplicationInsightsChannel.EndpointAddress;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (this.ApplicationInsightsChannel != null)
                {
                    this.ApplicationInsightsChannel.EndpointAddress = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the messageBroker
        /// </summary>
        internal IMessageBroker MessageBroker
        {
            get { return this.messageBroker ?? (this.messageBroker = GlimpseConfiguration.GetConfiguredMessageBroker()); }
            set { this.messageBroker = value; }
        }

        /// <summary>
        /// Flushes the configured telemetry channel.
        /// </summary>
        public void Flush()
        {
            if (this.ApplicationInsightsChannel != null)
            {
                this.ApplicationInsightsChannel.Flush();
            }
        }

        /// <summary>
        /// Dispose the channel. Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.ApplicationInsightsChannel != null)
            {
                this.ApplicationInsightsChannel.Dispose();
            }
        }

        /// <summary>
        /// Sends telemetry data item to the configured channel if the instrumentation key
        /// is not empty. Also publishes the telemetry item to the MessageBroker. Filters
        /// out the requests to Glimpse handler.
        /// </summary>
        /// <param name="item">Item to send.</param>
        public void Send(ITelemetry item)
        {
            var timer = this.TimerStrategy();

            if (item == null || timer == null || this.MessageBroker == null)
            {
                return;
            }

            // Filter the request telemetry to glimpse.axd
            if (item is RequestTelemetry)
            {
                var request = item as RequestTelemetry;
                if (request.Url.AbsolutePath != null)
                {
                    if (request.Url.AbsolutePath.ToLower().EndsWith("glimpse.axd"))
                    {
                        return;
                    }
                }
            }

            if (item is DependencyTelemetry)
            {
                var dependency = item as DependencyTelemetry;
                var timelineMessage = new DependencyTelemetryTimelineMessage(dependency);
                timelineMessage.Offset = timer.Point().Offset.Subtract(dependency.Duration);
                this.MessageBroker.Publish(timelineMessage);
            }

            if (item is TraceTelemetry)
            {
                TraceTelemetry t = new TraceTelemetry();

                var model = new TraceMessage
                {
                    Category = "Application Insights",
                    Message = t.SeverityLevel == null ? t.Message : t.SeverityLevel + ": " + t.Message,
                    FromFirst = timer.Point().Offset,
                    FromLast = this.CalculateFromLast(timer),
                    IndentLevel = 0
                };
                this.MessageBroker.Publish(model);
            }

            // Filter telemetry with empty instrumentation key
            if (!item.Context.InstrumentationKey.ToString().Equals("00000000-0000-0000-0000-000000000000"))
            {
                this.ApplicationInsightsChannel.Send(item);
            }

            this.messageBroker.Publish(item);
        }

        /// <summary>
        /// Calculates the elapsed time from the current trace message and the preceding one.
        /// </summary>
        /// <param name="timer">Timer that keeps track of the time the executions take. </param>
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