using System.Threading.Tasks;
using EventStore.ClientAPI;
using GraInInterfaces;
using Newtonsoft.Json;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Grains
{
    [LogConsistencyProvider(ProviderName = "CustomStorage")]
    public class GreetingGrain:EventSourceGrain<GreetingState,GreetingEvent>, IGreetingGrain
    {
        public async Task<string> SendGreetings(string greetings)
        {
            var state = State.Greeting;
            RaiseEvent(new GreetingEvent{Greeting = greetings});
            await ConfirmEvents();
            return greetings;
        }

        public GreetingGrain(IEventStoreConnection eventStoreConnection, JsonSerializerSettings jsonSerializerSettings) : base(eventStoreConnection, jsonSerializerSettings)
        {
        }

        protected override string GetGrainKey()
        {
            return this.GetPrimaryKeyLong().ToString();
        }
    }

    public class GreetingEvent
    {
        public string Greeting { get; set; }
    }

    
}