namespace GrainImplement
{
    using System;
    using System.Threading.Tasks;
    using GrainImplement.States;
    using GrainInterfaces;
    using Microsoft.Extensions.Logging;
    using Orleans;
    using Orleans.Providers;
    using Orleans.Runtime;


    [StorageProvider(ProviderName = "DevStore")]
    public class HelloGrain : Grain<HelloGrainState>, IHello, IRemindable
    {
        private readonly ILogger logger;
        private IDisposable timer;
        private readonly Random random = new System.Random();

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        public override Task OnActivateAsync()
        {
            timer = RegisterTimer(async s =>
            {
                var guid = Guid.Empty;
                var msg = random.Next();
                try
                {
                    var streamProvider = GetStreamProvider("SMSProvider");
                    var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");
                    await stream.OnNextAsync(msg/*, new SimpleStreamSequenceToken(msg)*/);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"SMSProvider-RANDOMDATA-SEND:{ex.Message}");
                    throw;
                }
                finally
                {
                    Console.WriteLine($"SMSProvider-RANDOMDATA-SEND:{msg}");
                }
            }, null, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000));
            return RegisterOrUpdateReminder("r1", TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
        }

        public override Task OnDeactivateAsync()
        {
            timer.Dispose();
            return Task.CompletedTask;
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            logger.LogInformation($"---ReceiveReminder {reminderName}, last message{State.LastMessage}'");
            return Task.CompletedTask;
        }

        async Task<string> IHello.SayHello(string greeting)
        {
            logger.LogInformation("request {0}", RequestContext.Get("TraceId"));
            logger.LogInformation($"+++SayHello message received: greeting = '{greeting}',last message is '{State.LastMessage}'");
            var feedback = $"You said: '{greeting}', I say: {State.LastMessage}!";
            State.LastMessage = greeting;
            await base.WriteStateAsync();
            return feedback;
        }

    }
}