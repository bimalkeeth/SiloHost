using System.Threading.Tasks;
using GraInInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Grains
{
    [LogConsistencyProvider(ProviderName = "StateStorage")]
    public class GreetingGrain:JournaledGrain<GreetingState,GreetingEvent>, IGreetingGrain
    {
        public async Task<string> SendGreetings(string greetings)
        {
            var state = State.Greeting;
            RaiseEvent(new GreetingEvent{Greeting = greetings});
            await ConfirmEvents();
            return greetings;
        }
    }

    public class GreetingEvent
    {
        public string Greeting { get; set; }
    }

    public class GreetingState
    {
        public string Greeting { get; set; }

        public GreetingState Apply(GreetingEvent @event)
        {
            Greeting = @event.Greeting;
            return this;
        }
    }
}