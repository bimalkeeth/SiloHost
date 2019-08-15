using System.Threading.Tasks;
using Orleans;

namespace GraInInterfaces
{
    public interface IHello:IGrainWithIntegerKey
    {
        Task<string> SayHello(string greetings);
    }
}