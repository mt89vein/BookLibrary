using Sstv.DomainExceptions;
using Sstv.DomainExceptions.DebugViewer;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Api.ProblemDetails;

/// <summary>
/// Enriches error code with it's HTTP status code.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Not used in request processing")]
internal sealed class StatusCodeEnricher : IDomainExceptionDebugEnricher
{
    /// <summary>
    /// Erich <paramref name="domainExceptionCodeDebugVm"/>.
    /// </summary>
    /// <param name="domainExceptionCodeDebugVm">Domain exception debug view model.</param>
    public void Enrich(DomainExceptionCodeDebugVm domainExceptionCodeDebugVm)
    {
        if (DomainExceptionSettings.Instance.ErrorCodesDescriptionSource is null)
        {
            return;
        }

        var desciption =
            DomainExceptionSettings
                .Instance
                .ErrorCodesDescriptionSource.GetDescription(domainExceptionCodeDebugVm.Code);

        if (desciption is null)
        {
            return;
        }

        domainExceptionCodeDebugVm.AdditionalData ??= [];
        domainExceptionCodeDebugVm.AdditionalData["HttpStatusCode"] = ErrorCodeMapping.MapToStatusCode(desciption).ToString();
    }
}