//-----------------------------------------------------------------------
// <copyright file="EventTelemetryTimelineMessage.cs" company="Glimpse">
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
    /// Convert class from Event Telemetry to Timeline Message
    /// </summary> 
    public class EventTelemetryTimelineMessage : ITimelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTelemetryTimelineMessage"/> class.
        /// </summary>
        /// <param name="telemetry">object telemetry</param>
        public EventTelemetryTimelineMessage(EventTelemetry telemetry)
        {
            this.EventName = telemetry.Name;
            this.EventCategory = new TimelineCategoryItem("Application Insights", "green", "yellow");
            this.EventSubText = "Device ID: " + telemetry.Context.Device.Id;
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
