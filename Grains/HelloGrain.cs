using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraInInterfaces;
using Orleans;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider]
    public class HelloGrain:Grain<GreetingArchive>,IHello
    {
        public override Task OnActivateAsync()
        {
            Console.WriteLine("On Activate Called");
            return base.OnActivateAsync();
        }
        public async Task<string> SayHello(string greetings)
        {
            State.Greetings.Add(greetings);
            await WriteStateAsync(); 
            DeactivateOnIdle();
            return $"You Said {greetings}, I say Hello";
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine("Grain Deactivating");
            return base.OnDeactivateAsync();
        }
    }

    public class GreetingArchive
    {
        public List<string> Greetings { get; set; }=new List<string>();
    }
}