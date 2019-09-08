using dworthen.grpc.certificate.AuthorizationPolicies;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dworthen.grpc.certificate.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddClientCertificateAuthorization(this IServiceCollection services, string policyName = "ClientCertificatePolicy")
        {
            services.AddSingleton<IAuthorizationHandler, ClientCertificateHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    policyName,
                    policy =>
                    {
                        policy.AuthenticationSchemes.Add(CertificateAuthenticationDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.AddRequirements(new ClientCertificateRequirement());
                    });
            });
            return services;
        }
    }
}
