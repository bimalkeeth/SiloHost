using System;
using System.Threading.Tasks;
using GraInInterfaces;
using Orleans;

namespace Grains
{
    public class HelloGrain:Grain,IHello
    {
        public Task<string> SayHello(string greetings)
        {
            return Task.FromResult<string>($"You Said {greetings}, I say Hello");
        }
    }
}