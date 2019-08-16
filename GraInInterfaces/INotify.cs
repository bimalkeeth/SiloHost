using Orleans;

namespace GraInInterfaces
{
    public interface INotify:IGrainObserver
    {
        void ReceiveMessage(string message);
    }
}