using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dworthen.grpc.certificate.AuthorizationPolicies
{
    public class ClientCertificateHandler : AuthorizationHandler<ClientCertificateRequirement>
    {

        private readonly IConfiguration _config;

        public ClientCertificateHandler(IConfiguration config)
        {
            _config = config;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientCertificateRequirement requirement)
        {
            if(!context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            List<string> approvedClientCertificateThumbprints = _config.GetSection("ApprovedClientCertificateThumbprints").Get<List<string>>();
            string clientThumbprint = context.User.FindFirst(ClaimTypes.Thumbprint).Value;

            if (approvedClientCertificateThumbprints.Any(t => t.Equals(clientThumbprint, StringComparison.OrdinalIgnoreCase))) 
            {
                context.Succeed(requirement);
            }
            //foreach(var claim in context.User.Claims)
            //{
            //    Console.WriteLine($"{claim.Type}: {claim.Value}");
            //}
            return Task.CompletedTask;
        }
    }

    public class ClientCertificateRequirement : IAuthorizationRequirement
    {

    }
}
