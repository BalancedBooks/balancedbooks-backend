using BalancedBooks_Integrations_CompanyRegistry;
using MediatR;

namespace BalancedBooks_API.PublicCompanyCertificate.Search;

public class SearchCompanyHandler(CompanyRegistryHttpClient companyRegistryHttpClient)
    : IRequestHandler<SearchCompanyQuery, string>
{
    public async Task<string> Handle(SearchCompanyQuery request,
        CancellationToken cancellationToken)
    {
        var companiesResult = await companyRegistryHttpClient.SearchCompaniesByAttributes(
            companyName: request.CompanyName,
            taxNumber: request.TaxNumber
        );
        
        return companiesResult.Value;
    }
}

public record SearchCompanyQueryResponse(string TaxNumber, string CompanyName);