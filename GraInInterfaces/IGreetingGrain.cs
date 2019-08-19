using System.Threading.Tasks;
using Orleans;

namespace GraInInterfaces
{
    public interface IGreetingGrain:IGrainWithIntegerKey
    {
        Task<string> SendGreetings(string greetings);
    }
}