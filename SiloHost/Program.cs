using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using GraInInterfaces;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using SiloHost.Context;
using SiloHost.Filters;

namespace SiloHost
{
    static class Program
    {
        private static readonly ManualResetEvent _siloStop=new ManualResetEvent(false);
        private static bool siloStopping;
        static readonly  object syncLock=new object();
        private static ISiloHost host;
        
        public static async Task<int> Main(string[] args)
        {
           return await RunSilo();
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                SetUpApplicationShutDown();
                await StartSilo();
                Console.WriteLine("Silo Starred");
                Console.WriteLine("Please enter to terminate");
                Console.ReadLine();
                _siloStop.WaitOne();
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
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IOrleansRequestContext, OrleansRequestContext>();
                    services.AddSingleton(s => CreateGrainMethodList());
                    services.AddSingleton(s => new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.None,
                        TypeNameHandling = TypeNameHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    services.AddSingleton(x => CreateEventStoreConnection());
                })
                .AddCustomStorageBasedLogConsistencyProvider("CustomStorage")
                .AddIncomingGrainCallFilter<LoggingFilter>()
                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.Invariant = "MySql.Data.MySqlClient";
                    options.ConnectionString = "Server=localhost;Uid=root;Pwd=root;Persist Security Info=true;Database=OrleansHelloWorld;SslMode=none";
                    options.UseJsonFormat = true;
                })
                .ConfigureApplicationParts(parts=>parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging=>logging.AddConsole());
            host = builder.Build();
            await host.StartAsync();
            return host;
        }
        private static GrainInfo CreateGrainMethodList()
        {
            var grainInterfaces = typeof(IHello).Assembly.GetTypes().Where(type => type.IsInterface)
                .SelectMany(type => type.GetMethods().Select(methodInfo => methodInfo.Name)).Distinct();
            return new GrainInfo{Methods = grainInterfaces.ToList()};
        }

        private static IEventStoreConnection CreateEventStoreConnection()
        {
            var conn = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));
            conn.ConnectAsync().GetAwaiter().GetResult();
            return conn;
        }

        static void SetUpApplicationShutDown()
        {
            Console.CancelKeyPress += (s, a) =>
            {
                a.Cancel = true;
                lock (syncLock)
                {
                    if (!siloStopping)
                    {
                        siloStopping = true;
                        Task.Run(StopSilo).Ignore();
                    }
                }
            };
        }
        static async Task StopSilo()
        {
            await host.StopAsync();
            _siloStop.Set();
        }
        
    }
}