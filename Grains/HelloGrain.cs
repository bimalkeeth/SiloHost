using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GraInInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;

namespace Grains
{
    [StorageProvider]
    public class HelloGrain:Grain<GreetingArchive>,IHello
    {
        private readonly ILogger<HelloGrain> _logger;
        private GrainObserverManager<INotify> _subsManager;
        public HelloGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
        }
        public override Task OnActivateAsync()
        {
            Console.WriteLine("On Activate Called");
            return base.OnActivateAsync();
        }
        public async Task<string> SayHello(string greetings)
        {
            State.Greetings.Add(greetings);

           var dd= GrainFactory.GetGrain<IProcessing>(Guid.NewGuid());
           
           var file = System.IO.File.ReadAllBytes("/Users/bimalkeeth/RiderProjects/SiloHost/SiloHost/19.pdf");

           await dd.ProcessFile(file);
            
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