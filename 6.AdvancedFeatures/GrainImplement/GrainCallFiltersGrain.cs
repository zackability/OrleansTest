namespace GrainImplement
{
    using GrainInterfaces;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Runtime;
    using System.Threading.Tasks;

    public class GrainCallFiltersGrain : Grain, IGrainCallFiltersGrain
    {
        private readonly ILogger logger;

        public GrainCallFiltersGrain(ILogger<GrainCallFiltersGrain> logger)
        {
            this.logger = logger;
        }

        public Task<int> Call(int num = 86)
        {
            logger.LogInformation("call filter:{0}", RequestContext.Get("intercepted value"));
            return Task.FromResult(num);
        }
    }
}