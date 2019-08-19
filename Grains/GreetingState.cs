namespace Grains
{
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