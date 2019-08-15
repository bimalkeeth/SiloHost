using System;
using System.Threading.Tasks;
using GraInInterfaces;
using Orleans;
using Orleans.Configuration;

namespace Client
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().GetAwaiter().GetResult();
        }
        static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await StartClient())
                {
                    var grain = client.GetGrain<IHello>(0);
                    var response = await grain.SayHello("Good Morning");
                    Console.WriteLine(response);
                    Console.WriteLine();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        static async Task<IClusterClient> StartClient()
        {
            var client = new ClientBuilder().Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloApp";

                }).UseLocalhostClustering()
                .Build();
            await client.Connect();
            Console.WriteLine("Client Connected");
            return client;

        }
    }
}