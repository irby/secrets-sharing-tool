using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace SecretsSharingTool.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Environment.GetEnvironmentVariables()));
            
            var configuredPort = Environment.GetEnvironmentVariable("PORT");
            var ok = int.TryParse(configuredPort, out var port);

            if (!ok)
            {
                throw new ArgumentException("Unable to get the application's port from the environment.");
            }

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(x =>
                {
                    x.AddServerHeader = false;
                    x.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    x.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                })
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseUrls("http://*:" + port + "/")
                .Build();
        }
    }
}