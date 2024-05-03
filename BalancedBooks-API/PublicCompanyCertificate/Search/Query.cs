using MediatR;

namespace BalancedBooks_API.PublicCompanyCertificate.Search;

public record SearchCompanyQuery(string CompanyName, string TaxNumber) : IRequest<SearchCompanyQueryResponse>;