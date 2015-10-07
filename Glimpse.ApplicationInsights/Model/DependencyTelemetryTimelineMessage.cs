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
    /// Convertion class from Dependency Telemetry to Timeline Message
    /// </summary>
    public class DependencyTelemetryTimelineMessage : ITimelineMessage
    {
        public DependencyTelemetryTimelineMessage(DependencyTelemetry telemetry)
        {
            this.EventName = telemetry.Name;
            this.EventCategory = new TimelineCategoryItem("Application Insights", "red", "orange");
            this.EventSubText = telemetry.CommandName;
            this.Duration = telemetry.Duration;
            this.StartTime = telemetry.StartTime.DateTime;
        }

        public string EventName { get; set; }

        public TimelineCategoryItem EventCategory { get; set; }

        public string EventSubText { get; set; }

        public TimeSpan Offset { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime StartTime { get; set; }

        public Guid Id { get; private set; }
    }

}
