namespace Glimpse.ApplicationInsights.Model.TelemetryTab
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;

    public class EventTelemetryMessage : ITelemetryMessage
    {
        public EventTelemetryMessage(EventTelemetry telemetry)
        {
            this.Time = telemetry.Timestamp.DateTime;
            this.Name = telemetry.Name;
            this.Details = null;
            this.Properties = telemetry.Properties.Count > 0 ? telemetry.Properties : null;
            this.Type = "Event";
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
