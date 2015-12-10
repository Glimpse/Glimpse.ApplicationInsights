namespace Glimpse.ApplicationInsights.Model.TelemetryTab
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;

    public class ExceptionTelemetryMessage : ITelemetryMessage
    {
        public ExceptionTelemetryMessage(ExceptionTelemetry telemetry)
        {
            this.Time = telemetry.Timestamp.DateTime;
            this.Name = telemetry.Exception.Message;
            this.Details = "Exception of type: " + telemetry.Exception.Message + "\n\r Happened in: " + telemetry.Exception.StackTrace;
            this.Properties = telemetry.Properties.Count > 0 ? telemetry.Properties : null;
            this.Type = "Exception";
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
