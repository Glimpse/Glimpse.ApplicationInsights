//-----------------------------------------------------------------------
// <copyright file="ExceptionTelemetryTraceMessage.cs" company="Glimpse">
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
    /// Convert class from Exception Telemetry to Trace Message
    /// </summary>
    public class ExceptionTelemetryTraceMessage : ITraceMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionTelemetryTraceMessage" /> class.
        /// </summary>
        /// <param name="telemetry">Telemetry item to be converted. </param>
        public ExceptionTelemetryTraceMessage(ExceptionTelemetry telemetry)
        {
            this.Category = telemetry.SeverityLevel == null ? "--" : SeverityToGlimpseCategory.SeverityToCategory((SeverityLevel)telemetry.SeverityLevel);
            this.Message = "Exception: " + telemetry.Exception.GetType();
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
