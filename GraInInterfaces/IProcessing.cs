using System.Threading.Tasks;
using Orleans;

namespace GraInInterfaces
{
    public interface IProcessing:IGrainWithGuidKey
    {
        Task<bool> ProcessFile(byte[] fileData);
    }
}