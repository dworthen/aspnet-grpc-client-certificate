﻿using Grpc.Net.Client;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace dworthen.grpc.certificate.client
{
    class Program

    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string basePath = System.AppContext.BaseDirectory;
            string certPath = Path.Combine(basePath!, "Certs", "client.pfx");

            X509Certificate2 certificate = new X509Certificate2(certPath, "1111");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);

            // Not needed if server certificate is valid 
            // Or installed as trusted cert on client machine.
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            var httpClient = new HttpClient(handler);

            string serviceLocation = Configuration.GetValue<string>("ServiceLocation");

            GrpcChannel channel = GrpcChannel.ForAddress(serviceLocation, new GrpcChannelOptions
            {
                HttpClient = httpClient
            });

            var client = new Greeter.GreeterClient(channel);


            var reply = await client.SayHelloAsync(
                                new HelloRequest { Name = "World" });
            Console.WriteLine(reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}


