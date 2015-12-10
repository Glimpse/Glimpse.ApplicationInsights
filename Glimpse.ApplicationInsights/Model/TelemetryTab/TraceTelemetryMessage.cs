namespace Glimpse.ApplicationInsights.Model.TelemetryTab
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;

    public class TraceTelemetryMessage : ITelemetryMessage
    {
        public TraceTelemetryMessage(TraceTelemetry telemetry)
        {
            this.Time = telemetry.Timestamp.DateTime;
            this.Name = "Trace";
            this.Details = telemetry.Message;
            this.Properties = telemetry.Properties.Count > 0 ? telemetry.Properties : null;
            this.Type = "Trace";
            this.Context = telemetry.Context;
        }

        public DateTime Time { get; set; }

        public string Name { get; set; }

        public string Details { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public string Type { get; set; }

        public TelemetryContext Context { get; set; }
    }
}
