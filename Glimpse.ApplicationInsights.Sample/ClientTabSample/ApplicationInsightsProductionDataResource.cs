using System.Collections.Generic;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Glimpse.Core.ResourceResult;

namespace Glimpse.ApplicationInsights.Sample
{
    public class ApplicationInsightsProductionDataResource : IResource
    {
        private const string ParamIdKey = "paramIdKey";

        public string Name
        {
            get { return "MyCustomTab"; }
        }

        public IEnumerable<ResourceParameterMetadata> Parameters
        {
            get
            {
                return new[]
                {
                    new ResourceParameterMetadata(ParamIdKey)
                };
            }
        }

        public IResourceResult Execute(IResourceContext context)
        {
            var clientParameter = context.Parameters[ParamIdKey];

            var data = new
                           {
                               A = "A " + clientParameter,
                               B = "B " + clientParameter,
                           };

            return new CacheControlDecorator(0, CacheSetting.NoCache, new JsonResourceResult(data, null)); ;
        }
    }
}