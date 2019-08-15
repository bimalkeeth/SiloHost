using System;
using System.Net;
using System.Threading.Tasks;
using Orleans.Configuration;
using Orleans.Hosting;

namespace SiloHost
{
    static class Program
    {
        public static async Task<int> Main(string[] args)
        {
           return await RunSilo();
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                await StartSilo();
                Console.WriteLine("Silo Starred");
                Console.WriteLine("Please enter to terminate");
                Console.ReadLine();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        
        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloApp";
                }).UseLocalhostClustering()
                .Configure<EndpointOptions>(options =>
                {
                    options.SiloPort = 11111;
                    options.GatewayPort = 30000;
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                });
            var host = builder.Build();
            await host.StartAsync();
            return host;

        }
    }
}