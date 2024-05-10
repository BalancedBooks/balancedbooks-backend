using BalancedBooks_Integrations_CompanyRegistry;
using MediatR;

namespace BalancedBooksAPI.PublicCompanyCertificate.Search;

public class SearchCompanyHandler(CompanyRegistryHttpClient companyRegistryHttpClient)
    : IRequestHandler<SearchCompanyQuery, SearchCompanyQueryResponse>
{
    public async Task<SearchCompanyQueryResponse> Handle(SearchCompanyQuery request,
        CancellationToken cancellationToken)
    {
        var companiesResult = await companyRegistryHttpClient.SearchCompaniesByAttributes(
            companyName: request.CompanyName,
            taxNumber: request.TaxNumber
        );
        
        return new("", "");
    }
}

public record SearchCompanyQueryResponse(string TaxNumber, string CompanyName);