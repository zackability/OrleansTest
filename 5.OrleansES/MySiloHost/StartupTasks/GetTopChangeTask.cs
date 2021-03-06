namespace MySiloHost.StartupTasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GrainInterfaces;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Runtime;

    public class GetTopChangeTask : IStartupTask
    {
        private readonly IGrainFactory grainFactory;
        private readonly ILogger logger;

        public GetTopChangeTask(IGrainFactory grainFactory, ILogger<GetTopChangeTask> logger)
        {
            this.grainFactory = grainFactory;
            this.logger = logger;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {

#if LOG_BASED

            var logEventGrain = grainFactory.GetGrain<ILogStorageBasedEventGrain>(0);
            DisplayTop(nameof(logEventGrain), await logEventGrain.GetTop());

#endif

#if STATE_BASED

            var stateEventGrain = grainFactory.GetGrain<IStateStorageBasedEventGrain>(0);
            DisplayTop(nameof(stateEventGrain), await stateEventGrain.GetTop());
#endif

#if CUSTOM_BASED

            var customEventGrain = grainFactory.GetGrain<ICustomStorageBasedEventGrain>(0);
            DisplayTop(nameof(customEventGrain), await customEventGrain.GetTop());
#endif

        }

        private void DisplayTop(string name, Change top)
        {
            if (null == top)
            {
                logger.LogInformation("{0}没有更新", name);
            }
            else
            {
                logger.LogInformation("{0}最新事件：{1},{2},{3}", name, top.Name, top.Value, top.When);
            }
        }
    }
}