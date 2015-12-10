namespace Glimpse.ApplicationInsights.Model.TelemetryTab
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;

    public class RequestTelemetryMessage : ITelemetryMessage
    {
        public RequestTelemetryMessage(RequestTelemetry telemetry)
        {
            this.Time = telemetry.Timestamp.DateTime;
            this.Name = telemetry.Name;
            this.Details = "Response Code: " + telemetry.ResponseCode + "\n\r Succesful Request: " + telemetry.Success +
                            "\n\r Request URL: " + telemetry.Url;
            this.Properties = telemetry.Properties.Count > 0 ? telemetry.Properties : null;
            this.Type = "Request";
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
