using System.Xml;
using System.Xml.Serialization;
using BalancedBooks_Integrations_CompanyRegistry.Models;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Flurl;
using NullValueHandling = Flurl.NullValueHandling;

namespace BalancedBooks_Integrations_CompanyRegistry;

public class CompanyRegistryHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CompanyRegistryHttpClient> _logger;
    private readonly IOptions<CompanyRegistryConfig> _config;
    private const string BasePath = "https://occsztest.e-cegjegyzek.hu/IMOnline?outformat=xml&lang=hu";

    public CompanyRegistryHttpClient(HttpClient httpClient, ILogger<CompanyRegistryHttpClient> logger, IOptions<CompanyRegistryConfig> config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = config;
        httpClient.BaseAddress = config.Value.ApiUrlAsUrl;
    }

    public async Task<Result<string>> SearchCompaniesByAttributes(
        string? companyName = null,
        string? taxNumber = null,
        string? registrationNumber = null,
        List<LegalForm>? legalForms = null,
        List<StateCourts>? courts = null,
        string? postalCode = null,
        string? settlement = null,
        string? street = null,
        string? houseNumber = null,
        //
        bool onlyActive = false,
        bool searchInMiddleOfName = true,
        // TODO VDAT
        int maxNum = 100
    )
    {
        var legalFormsFormatted = legalForms != null ? string.Join(",", legalForms.ToArray()) : null;
        var courtsFormatted = courts != null ? string.Join(",", courts.ToArray()) : null;

        var url = BasePath
                .AppendQueryParam("cegnev", companyName, NullValueHandling.Ignore)
                .AppendQueryParam("adoszam", taxNumber, NullValueHandling.Ignore)
                .AppendQueryParam("cegjegyzekszam", registrationNumber, NullValueHandling.Ignore)
                .AppendQueryParam("cegforma", legalFormsFormatted, NullValueHandling.Ignore)
                .AppendQueryParam("megye", courtsFormatted, NullValueHandling.Ignore)
                .AppendQueryParam("iranyitoszam", postalCode, NullValueHandling.Ignore)
                .AppendQueryParam("telepules", settlement, NullValueHandling.Ignore)
                .AppendQueryParam("utca", street, NullValueHandling.Ignore)
                .AppendQueryParam("hazszam", houseNumber, NullValueHandling.Ignore)
                // search types
                .AppendQueryParam("hatalyos", onlyActive ? 1 : 0)
                .AppendQueryParam("nevkozep", searchInMiddleOfName ? 1 : 0)
                // query related
                .AppendQueryParam("maxnum", maxNum)
                .AppendQueryParam("reqtip", "adat")
                .AppendQueryParam("stype", "Cegkereses")
            ;

        var serializer = new XmlSerializer(typeof(Cegjegyzek));

        var responseMessage = await _httpClient.GetAsync(url);
        var stream = await responseMessage.Content.ReadAsStreamAsync();
        var asString = await responseMessage.Content.ReadAsStringAsync();

        _logger.LogTrace(asString);

        if (serializer.Deserialize(stream) is not Cegjegyzek xmlResult)
        {
            throw new Exception("BAD_GATEWAY");
        }

        if (xmlResult.Kiadmany?.CegAdatlapok.Count == 0)
        {
            var f = "";
        }

        if (xmlResult.Hibajegyzek != null)
        {
            var errorCode = xmlResult.Hibajegyzek.Hiba.Kod;
            /*return Result.Fail(ne)*/
        }

    /*if (xmlResult.Kiadmany != null)
    {
        return xmlResult.Kiadmany;
    }*/


        XmlDocument doc = new XmlDocument();
        doc.LoadXml(asString);

        return Result.Ok(JsonConvert.SerializeXmlNode(doc));

        // throw HandleXmlErrors(xmlResult.Hibajegyzek?.Hiba);
    }

    /// <summary>
    /// Handle XML Errors
    /// </summary>
    /// <param name="hiba"></param>
    /// <returns></returns>
    /// <exception cref="BadGatewayException"></exception>
    /*private static HttpBaseException HandleXmlErrors(Hiba? hiba)
    {
        HttpBaseException exception = hiba?.Kod switch
        {
            HibaKod.NoResultsByGivenFilters or HibaKod.NoResultsByGivenFilters or HibaKod.NoResultsByTaxNumber =>
                new NotFoundException(hiba.Kod.ToString(), hiba.Text),
            HibaKod.ResultTooBigOrSystemDoesNotAllowIt => new BadRequestException(hiba.Kod.ToString(), hiba.Text),
            HibaKod.TooShortQueryFilters => new BadRequestException(hiba.Kod.ToString(), hiba.Text),
            HibaKod.MissingRequestParams => new BadRequestException("MISSING_PARAMS", "Missing params"),
            _ => throw new BadGatewayException("", "")
        };

        return exception;
    }*/
}