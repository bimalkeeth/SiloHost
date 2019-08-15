using System;
using System.Threading.Tasks;
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
                    Console.WriteLine($"Client {client.IsInitialized}");
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