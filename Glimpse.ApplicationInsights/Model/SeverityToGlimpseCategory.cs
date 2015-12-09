//-----------------------------------------------------------------------
// <copyright file="SeverityToGlimpseCategory.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Glimpse.ApplicationInsights.Model
{
    using Microsoft.ApplicationInsights.DataContracts;
    
    /// <summary>
    /// Convert from telemetry severity to glimpse trace category
    /// </summary>
    public static class SeverityToGlimpseCategory
    {
        /// <summary>
        /// Convert the severity to a recognizable String category
        /// of Glimpse.
        /// </summary>
        /// <param name="severity">Severity level of telemetry. </param>
        /// <returns> Glimpse category </returns>
        public static string SeverityToCategory(SeverityLevel severity) 
        {
            switch (severity)
            {
                case SeverityLevel.Error:
                    return "error";
                case SeverityLevel.Information:
                    return "info";
                case SeverityLevel.Warning:
                    return "warn";
                default:
                    return severity.ToString();
            }
        }
    }
}
