using System.Threading.Tasks;
using GraInInterfaces;
using Orleans;

namespace Grains
{
    public class ProcessingGrain:Grain,IProcessing
    {
        public async Task<bool> ProcessFile(byte[] fileData)
        {
            var data = fileData;
            return false;
        }
    }
}