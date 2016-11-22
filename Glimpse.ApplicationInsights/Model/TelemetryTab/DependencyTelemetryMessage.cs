namespace Glimpse.ApplicationInsights.Model.TelemetryTab
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;

    public class DependencyTelemetryMessage : ITelemetryMessage
    {
        public DependencyTelemetryMessage(DependencyTelemetry dependency)
        {
            this.Time = dependency.Timestamp.DateTime;
            this.Name = dependency.Type + ": " + dependency.Name.Split('|')[0];
            this.Details = dependency.Data;
            this.Properties = dependency.Properties.Count > 0 ? dependency.Properties : null;
            this.Type = "Dependency";
            this.Context = dependency.Context;
        }

        public DateTime Time { get; set; }

        public string Name { get; set; }

        public string Details { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public string Type { get; set; }

        public TelemetryContext Context { get; set; }
    }
}
