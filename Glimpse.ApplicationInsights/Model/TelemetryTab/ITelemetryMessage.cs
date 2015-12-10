//-----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTab.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Glimpse.ApplicationInsights.Model.TelemetryTab
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;

    public interface ITelemetryMessage
    {
        DateTime Time { get; }

        string Name { get; }

        string Details { get; }

        IDictionary<string, string> Properties { get; }

        string Type { get; }

        TelemetryContext Context { get; }
    }
}
