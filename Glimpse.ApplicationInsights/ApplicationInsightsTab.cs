//-----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTab.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Glimpse.ApplicationInsights
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Glimpse.Core.Extensibility;
    using Glimpse.Core.Extensions;
    using Glimpse.Core.Message;
    using Glimpse.Core.Tab.Assist;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    
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
            get { return "http://getglimpse.com/Help/Trace-Tab"; }
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
                    var trace = telemetry as TraceTelemetry;
                    data.Add(new
                        {
                            time = trace.Timestamp.DateTime,
                            name = trace.Message,
                            details = trace.Sequence,
                            properties = trace.Properties,
                            type = "Trace",
                            context = trace.Context
                        });
                }

                if (telemetry is DependencyTelemetry) 
                {
                    var dependency = telemetry as DependencyTelemetry;
                    data.Add(new
                        {
                            time = dependency.Timestamp.DateTime,
                            name = dependency.DependencyKind + ": " + dependency.Name.Split('|')[0],
                            details = dependency.Name,
                            properties = dependency.Properties,
                            type = "Dependency",
                            context = dependency.Context
                        });
                }

                if (telemetry is EventTelemetry)
                {
                    var tevent = telemetry as EventTelemetry;
                    data.Add(new
                        {
                            time = tevent.Timestamp.DateTime,
                            name = tevent.Name,
                            details = "Device ID: " + telemetry.Context.Device.Id,
                            properties = tevent.Properties,
                            type = "Event",
                            context = tevent.Context
                        });
                }

                if (telemetry is ExceptionTelemetry)
                {
                    var exception = telemetry as ExceptionTelemetry;
                    data.Add(new
                        {
                            time = exception.Timestamp.DateTime,
                            name = exception.Exception.Message,
                            details = "Exception of type: " + exception.Exception.Message + "\n\r Happened in: " + exception.Exception.StackTrace,
                            properties = exception.Properties,
                            type = "Exception",
                            context = exception.Context
                        });
                }

                if (telemetry is RequestTelemetry)
                {
                    var request = telemetry as RequestTelemetry;
                    data.Add(new
                        {
                            time = request.Timestamp.DateTime,
                            name = request.Name,
                            details = "Response Code: " + request.ResponseCode + "\n\r Succesful Request: " + request.Success +
                            "\n\r Request URL: " + request.Url + "\n\r Device ID: " + request.Context.Device.Id,
                            properties = request.Properties,
                            type = "Request",
                            context = request.Context
                        });
                }
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