using System.Threading.Tasks;

namespace GraInInterfaces
{
    public interface IGreetingGrain
    {
        Task<string> SendGreetings(string greetings);
    }
}