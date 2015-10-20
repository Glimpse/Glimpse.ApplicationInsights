//-----------------------------------------------------------------------
// <copyright file="GlimpseTelemetryChannel.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Glimpse.ApplicationInsights
{
    using System;
    using System.Diagnostics;
    using Glimpse.ApplicationInsights.Model;
    using Glimpse.Core.Extensibility;
    using Glimpse.Core.Framework;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Telemetry channel that will send Application Insights telemetry
    /// to Glimpse message broker and to the Application Insights channel.
    /// </summary>
    public class GlimpseTelemetryChannel : ITelemetryChannel
    {
        /// <summary>
        /// Stopwatch from the last telemetry
        /// </summary>
        [ThreadStatic]
        private static Stopwatch fromLastWatch;

        /// <summary>
        /// MessageBroker for subscribing the messages to Glimpse
        /// </summary>
        private IMessageBroker messageBroker;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GlimpseTelemetryChannel" /> class.
        /// </summary>
        public GlimpseTelemetryChannel()
        {
            this.MessageBroker = GlimpseConfiguration.GetConfiguredMessageBroker();
            this.TimerStrategy = GlimpseConfiguration.GetConfiguredTimerStrategy();
        }

        /// <summary>
        /// Gets the channel from ApplicationInsights.config file that
        /// will send the telemetry to Application Insights.
        /// </summary>
        public ITelemetryChannel Channel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether developer mode 
        /// of the telemetry transmission is enabled.
        /// </summary>
        public bool? DeveloperMode
        {
            get
            {
                if (this.Channel != null)
                {
                    return this.Channel.DeveloperMode;
                }
                else
                {
                    return true;
                }
            }

            set
            {
                if (this.Channel != null)
                {
                    this.Channel.DeveloperMode = value;
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
                if (this.Channel != null)
                {
                    return this.Channel.EndpointAddress;
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (this.Channel != null)
                {
                    this.Channel.EndpointAddress = value;
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
        /// Gets or sets IExecutionTimer
        /// </summary>
        internal Func<IExecutionTimer> TimerStrategy { get; set; }

        /// <summary>
        /// Flushes the configured telemetry channel.
        /// </summary>
        public void Flush()
        {
            if (this.Channel != null)
            {
                this.Channel.Flush();
            }
        }

        /// <summary>
        /// Dispose the channel. Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Channel != null)
            {
                this.Channel.Dispose();
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
                if (request.Url != null && request.Url.AbsolutePath != null)
                {
                    if (request.Url.AbsolutePath.ToLower().EndsWith("glimpse.axd"))
                    {
                        return;
                    }
                }
            }

            if (item is TraceTelemetry)
            {
                var trace = item as TraceTelemetry;
                var traceMessage = new TraceTelemetryTraceMessage(trace);
                traceMessage.FromFirst = timer.Point().Offset;
                traceMessage.FromLast = this.CalculateFromLast(timer);
                this.MessageBroker.Publish(traceMessage);
            }

            if (item is EventTelemetry)
            {
                // Send it to TraceTab
                var eventT = item as EventTelemetry;
                var eventMessage = new EventTelemetryTraceMessage(eventT);
                eventMessage.FromFirst = timer.Point().Offset;
                eventMessage.FromLast = this.CalculateFromLast(timer);
                this.MessageBroker.Publish(eventMessage);

                // Send it to TimelineTab
                var timelineMessage = new EventTelemetryTimelineMessage(eventT);
                timelineMessage.Offset = timer.Point().Offset;
                this.MessageBroker.Publish(timelineMessage);
            }

            if (item is ExceptionTelemetry)
            {
                // Send it to TraceTab
                var trace = item as ExceptionTelemetry;
                var traceMessage = new ExceptionTelemetryTraceMessage(trace);
                traceMessage.FromFirst = timer.Point().Offset;
                traceMessage.FromLast = this.CalculateFromLast(timer);
                this.MessageBroker.Publish(traceMessage);

                // Send it to TimelineTab
                var timelineMessage = new ExceptionTelemetryTimelineMessage(trace);
                timelineMessage.Offset = timer.Point().Offset;
                this.MessageBroker.Publish(timelineMessage);
            }

            if (item is RequestTelemetry)
            {
                var request = item as RequestTelemetry;
                var timelineMessage = new RequestTelemetryTimelineMessage(request);
                timelineMessage.Offset = timer.Point().Offset.Subtract(request.Duration);
                this.MessageBroker.Publish(timelineMessage);
            }

            if (item is DependencyTelemetry)
            {
                var dependency = item as DependencyTelemetry;
                var timelineMessage = new DependencyTelemetryTimelineMessage(dependency);
                timelineMessage.Offset = timer.Point().Offset.Subtract(dependency.Duration);
                this.MessageBroker.Publish(timelineMessage);
            }

            // Filter telemetry with empty instrumentation key
            if (item.Context.InstrumentationKey != null && !item.Context.InstrumentationKey.Equals("00000000-0000-0000-0000-000000000000"))
            {
                this.Channel.Send(item);
            }

            this.messageBroker.Publish(item);
        }

       /// <summary>
       /// Calculates the elapsed time from the current trace message and the preceding one.
       /// </summary>
       /// <param name="timer">Timer that keeps track of the time the executions take. </param>
       /// <returns> Calculate From Last </returns>
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