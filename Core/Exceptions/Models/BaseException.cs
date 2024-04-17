using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BalancedBooks_API.Core.Exceptions.Models;

public interface IHttpBaseException
{
    HttpStatusCode HttpStatusCode { get; }
    string Message { get; }
}

/// <summary>
/// Base exception class for our custom exceptions - Serializable
/// Until system.text.json supports https://github.com/dotnet/runtime/issues/30180, we have to use newtonsoft.
/// Otherwise we cannot exclude other fields from the JSON (e.g HelpLink, Source etc from base Exception) 
/// </summary>
[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public abstract class HttpBaseException : Exception, IHttpBaseException
{
    [JsonProperty("httpStatusCode")] public abstract HttpStatusCode HttpStatusCode { get; }

    [JsonProperty("message")] public abstract override string Message { get; }

    /// <summary>
    /// For debug purpose. Log Trace Id
    /// </summary>
    [JsonProperty("traceIdentifier")]
    public string? TraceIdentifier { get; set; }

    /// <summary>
    /// Helper for type cast between the enum and int formats
    /// </summary>
    public int IntegerHttpStatusCode => (int)HttpStatusCode;

    protected HttpBaseException()
    {
    }

    protected HttpBaseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Only called internally from Net
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    [Obsolete("Obsolete")]
    protected HttpBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <summary>
    /// Only called internally from Net
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [Obsolete(
        "This API supports obsolete formatter-based serialization. It should not be called or extended by application code.",
        DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        info.AddValue(nameof(HttpStatusCode), HttpStatusCode);
        info.AddValue(nameof(TraceIdentifier), TraceIdentifier);
        base.GetObjectData(info, context);
    }
}