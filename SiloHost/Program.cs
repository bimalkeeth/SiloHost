using System;
using System.Net;
using System.Threading.Tasks;
using Grains;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

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
                })
                .UseDashboard()
                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.Invariant = "MySql.Data.MySqlClient";
                    options.ConnectionString = "Server=localhost;Uid=root;Pwd=root;Persist Security Info=true;Database=OrleansHelloWorld;SslMode=none";
                    options.UseJsonFormat = true;
                })
                .ConfigureApplicationParts(parts=>parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging=>logging.AddConsole())
                ;
            var host = builder.Build();
            await host.StartAsync();
            return host;

        }
    }
}