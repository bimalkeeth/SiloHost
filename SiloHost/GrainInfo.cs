using System.Collections.Generic;

namespace SiloHost
{
    public class GrainInfo
    {
        public GrainInfo()
        {
            Methods=new List<string>();
        }
        public List<string> Methods { get; set; }
    }
}