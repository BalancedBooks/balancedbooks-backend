using System.Net.Http.Headers;
using BalancedBooks_Integrations_CompanyRegistry.Config;
using Microsoft.Extensions.Options;

namespace BalancedBooks_Integrations_CompanyRegistry;

public class CompanyRegistryHandler(IOptions<CompanyRegistryConfig> companyRegistryConfig) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var base64Hash = companyRegistryConfig.Value.Base64Hash;
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Hash);

        request.Headers.Add("Content", "application/xml");
        request.Headers.Add("Accept", "application/xml");

        return base.SendAsync(request, cancellationToken);
    }
}