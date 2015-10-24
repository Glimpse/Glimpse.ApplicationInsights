//-----------------------------------------------------------------------
// <copyright file="ExceptionTelemetryTimelineMessage.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Glimpse.ApplicationInsights.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Glimpse.Core.Message;
    using Microsoft.ApplicationInsights.DataContracts;
    
    /// <summary>
    /// Convert class from Exception Telemetry to Timeline Message
    /// </summary>
    public class ExceptionTelemetryTimelineMessage : ITimelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionTelemetryTimelineMessage"/> class.
        /// </summary>
        /// <param name="telemetry">object telemetry</param>
        public ExceptionTelemetryTimelineMessage(ExceptionTelemetry telemetry)
        {
            this.EventName = telemetry.Exception.Message;
            this.EventCategory = new TimelineCategoryItem("Application Insights", "green", "yellow");
            this.EventSubText = "Exception of type: " + telemetry.Exception.Message + "<br> Happened in: " + telemetry.Exception.StackTrace;
        }

        /// <summary>
        /// Gets or sets EventName
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets EventCategory
        /// </summary>
        public TimelineCategoryItem EventCategory { get; set; }

        /// <summary>
        /// Gets or sets EventSubText
        /// </summary>
        public string EventSubText { get; set; }

        /// <summary>
        /// Gets or sets Offset
        /// </summary>
        public TimeSpan Offset { get; set; }

        /// <summary>
        /// Gets or sets Duration
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets Start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets Id
        /// </summary>
        public Guid Id { get; private set; }
    }
}
