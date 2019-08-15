using System;
using System.Threading.Tasks;
using GraInInterfaces;
using Orleans;

namespace Grains
{
    public class HelloGrain:Grain,IHello
    {
        public override Task OnActivateAsync()
        {
            Console.WriteLine("On Activate Called");
            return base.OnActivateAsync();
        }
        public Task<string> SayHello(string greetings)
        {
            DeactivateOnIdle();
            return Task.FromResult<string>($"You Said {greetings}, I say Hello");
            
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine("Grain Deactivating");
            return base.OnDeactivateAsync();
        }
    }
}