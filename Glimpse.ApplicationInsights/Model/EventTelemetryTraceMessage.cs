using Glimpse.Core.Message;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ApplicationInsights.Model
{
    /// <summary>
    /// Convertion class from Event Telemetry to Trace Message
    /// </summary>
    class EventTelemetryTraceMessage : ITraceMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTelemetryTraceMessage" /> class.
        /// </summary>
        /// <param name="telemetry">Telemetry item to be converted. </param>
        public EventTelemetryTraceMessage(EventTelemetry telemetry)
        {
            this.Category = "info";
            this.Message = "Custom event: " +  telemetry.Name;
            this.IndentLevel = 0;
        }
        public string Category { get; set; }

        public string Message { get; set; }

        public TimeSpan FromFirst { get; set; }

        public TimeSpan FromLast { get; set; }

        public int IndentLevel { get; set; }
    }
}
