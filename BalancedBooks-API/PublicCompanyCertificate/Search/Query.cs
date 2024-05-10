using MediatR;

namespace BalancedBooksAPI.PublicCompanyCertificate.Search;

public record SearchCompanyQuery(string CompanyName, string TaxNumber) : IRequest<SearchCompanyQueryResponse>;