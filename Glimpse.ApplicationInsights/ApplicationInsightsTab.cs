//-----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTab.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Glimpse.ApplicationInsights
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using Glimpse.Core.Extensibility;
    using Glimpse.Core.Extensions;
    using Glimpse.Core.Tab.Assist;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Glimpse.ApplicationInsights.Model.TelemetryTab;

    /// <summary>
    /// Trace tab
    /// </summary>
    public class ApplicationInsightsTab : ITab, ITabSetup, IDocumentation, ITabLayout, IKey
    {
        /// <summary>
        /// sets value to cells.
        /// </summary>
        private static readonly object Layout = TabLayout.Create()
                .Row(r =>
                {
                    r.Cell("time").WithTitle("Time");
                    r.Cell("name").WithTitle("Name");
                    r.Cell("details").WithTitle("Details");
                    r.Cell("properties").WithTitle("Custom properties");
                    r.Cell("type").WithTitle("Type");
                    r.Cell("context").WithTitle("Context");
                }).Build();

        /// <summary>
        /// MessageBroker for subscribing the messages to Glimpse
        /// </summary>
        private IMessageBroker messageBroker;

        /// <summary>
        /// Gets the name that will show in the tab.
        /// </summary>
        public string Name
        {
            get { return "Application Insights"; }
        }

        /// <summary>
        /// Gets the documentation URI.
        /// </summary>
        public string DocumentationUri
        {
            get { return "https://github.com/Glimpse/Glimpse.ApplicationInsights/wiki/Application-Insights-Tab"; }
        }

        /// <summary>
        /// Gets when the <see cref="ITab.GetData" /> method should run.
        /// </summary>
        public RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.EndRequest; }
        }

        /// <summary>
        /// Gets the type of the request context that the Tab relies on. If
        /// returns null, the tab can be used in any context.
        /// </summary>
        public Type RequestContextType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key
        {
            get { return "glimpse_ai"; }
        }
        
        /// <summary>
        /// Gets the layout which controls the layout.
        /// </summary>
        /// <returns>Object that dictates the layout.</returns>
        public object GetLayout()
        {
            return Layout;
        }
        
        /// <summary>
        /// Gets the data that should be shown in the UI.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Object that will be shown.</returns>
        public object GetData(ITabContext context)
        {
            var data = new List<object>();

            var telemetryMessages = context.GetMessages<ITelemetry>();
            foreach (var telemetry in telemetryMessages)
            {
                if (telemetry is TraceTelemetry)
                {
                    var trace = new TraceTelemetryMessage(telemetry as TraceTelemetry);
                    data.Add(trace);
                }

                if (telemetry is DependencyTelemetry) 
                {
                    var dependency = new DependencyTelemetryMessage(telemetry as DependencyTelemetry);
                    data.Add(dependency);
                }

                if (telemetry is EventTelemetry)
                {
                    var tevent = new EventTelemetryMessage(telemetry as EventTelemetry);
                    data.Add(tevent);
                }

                if (telemetry is ExceptionTelemetry)
                {
                    var exception = new ExceptionTelemetryMessage(telemetry as ExceptionTelemetry);
                    data.Add(exception);
                }

                if (telemetry is RequestTelemetry)
                {
                    var request = new RequestTelemetryMessage(telemetry as RequestTelemetry);
                    data.Add(request);
                }
            }

            // This is a temporary fix that allows to show request telemetry object inside Application Insights tab
            // The issue here is that ApplicationInsights's End callback that produces request telemetry object will be called after 
            // Glimpse's EndRequest method that will populate all request details. See github issue #17 for details:
            // https://github.com/Glimpse/Glimpse.ApplicationInsights/issues/17
            // Long term we might need to switch Application Insights tab from showing request-specific telemetry to show all telemetry
            // like history tab does with the ability to 
            var requestTelemetry = System.Web.HttpContext.Current.Items["Microsoft.ApplicationInsights.RequestTelemetry"] as RequestTelemetry;
            if (requestTelemetry != null)
            {
                HttpContext ctx = HttpContext.Current;
                if (ctx != null)
                {
                    requestTelemetry.ResponseCode = ctx.Response.StatusCode.ToString(CultureInfo.InvariantCulture);
                    requestTelemetry.Success =
                        (ctx.Response.StatusCode < 400) ||
                        (ctx.Response.StatusCode == 401);
                    requestTelemetry.Url = ctx.Request.Unvalidated.Url;
                    requestTelemetry.HttpMethod = ctx.Request.HttpMethod;
                }

                foreach (var initializer in TelemetryConfiguration.Active.TelemetryInitializers)
                {
                    initializer.Initialize(requestTelemetry);
                }

                data.Add(new RequestTelemetryMessage(requestTelemetry));
            }

            return data;
        }

        /// <summary>
        /// Setups the targeted tab using the specified context.
        /// </summary>
        /// <param name="context">The context which should be used.</param>
        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<ITelemetry>();
        }
    }
}
