using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace dworthen.grpc.certificate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((hostingContext, options) =>
                    {
                        options.ListenLocalhost(hostingContext.Configuration.GetValue<int>("Port"), listenOptions =>
                        {
                            string basePath = System.AppContext.BaseDirectory;
                            string certPath = Path.Combine(basePath!, "Certs", "server.pfx");

                            X509Certificate2 certificate = new X509Certificate2(certPath, "1111");
                            listenOptions.UseHttps(certificate, o =>
                            {
                                o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                                // Allow for self-signed certificates
                                o.AllowAnyClientCertificate();
                            });
                            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                        });
                    });
                });

    }
}
