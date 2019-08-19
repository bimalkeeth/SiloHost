using System;
using System.Threading.Tasks;
using GraInInterfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Polly;

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
                using (var client =  StartClient())
                {
                    RequestContext.Set("traceId",Guid.NewGuid());
                    var grain = client.GetGrain<IHello>(0);
                    var response = await grain.SayHello("Good Morning");
                    Console.WriteLine(response);
                    Console.WriteLine();


                    var greetGrain = client.GetGrain<IGreetingGrain>(0);
                    await greetGrain.SendGreetings("Hi There");
                    
                    var greetGrain2 = client.GetGrain<IGreetingGrain>(0);
                    await greetGrain.SendGreetings("Hi Morning");
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        //static async Task<IClusterClient> StartClient()
        static IClusterClient StartClient()
        {
            return Policy<IClusterClient>.Handle<SiloUnavailableException>()
                .Or<OrleansMessageRejectionException>()
                .WaitAndRetry(new []{TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)}).Execute(() =>
                {
                    var client = new ClientBuilder().Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloApp";

                        }).UseLocalhostClustering()
                        .Build();
                    client.Connect().GetAwaiter().GetResult();
                    Console.WriteLine("Client Connected");
                    return client;
                });
        }
    }
}