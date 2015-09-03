using Glimpse.Core.Extensibility;

namespace Glimpse.ApplicationInsights.Sample
{
    public class ApplicationInsightsProductionDataClientScript : IStaticClientScript
    {
        public ScriptOrder Order
        {
            get { return ScriptOrder.IncludeAfterClientInterfaceScript; }
        }

        public string GetUri(string version)
        {
            //return "/ClientTabSample/ApplicationInsightsProductionDataScript.js";
             return "http://abaranchtest1.cloudapp.net/scripts/ApplicationInsightsProductionDataScript.js";
        }
    }
}