//-----------------------------------------------------------------------
// <copyright file="EventTelemetryTraceMessage.cs" company="Glimpse">
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
    /// Convert class from Event Telemetry to Trace Message
    /// </summary>
    public class EventTelemetryTraceMessage : ITraceMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTelemetryTraceMessage" /> class.
        /// </summary>
        /// <param name="telemetry">Telemetry item to be converted. </param>
        public EventTelemetryTraceMessage(EventTelemetry telemetry)
        {
            this.Category = "info";
            this.Message = "Custom event: " + telemetry.Name;
            this.IndentLevel = 0;
        }

        /// <summary>
        /// Gets or sets category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets TimeSpan from first
        /// </summary>
        public TimeSpan FromFirst { get; set; }

        /// <summary>
        /// Gets or sets TimeSpan from last
        /// </summary>
        public TimeSpan FromLast { get; set; }

        /// <summary>
        /// Gets or sets Indentation level
        /// </summary>
        public int IndentLevel { get; set; }
    }
}
