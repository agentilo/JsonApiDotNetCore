using JetBrains.Annotations;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Diagnostics;
using JsonApiDotNetCore.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace JsonApiDotNetCore.QueryStrings.Internal;

/// <inheritdoc />
[PublicAPI]
public class QueryStringReader : IQueryStringReader
{
    private readonly IJsonApiOptions _options;
    private readonly IRequestQueryStringAccessor _queryStringAccessor;
    private readonly IEnumerable<IQueryStringParameterReader> _parameterReaders;
    private readonly ILogger<QueryStringReader> _logger;

    public QueryStringReader(IJsonApiOptions options, IRequestQueryStringAccessor queryStringAccessor,
        IEnumerable<IQueryStringParameterReader> parameterReaders, ILoggerFactory loggerFactory)
    {
        ArgumentGuard.NotNull(loggerFactory, nameof(loggerFactory));
        ArgumentGuard.NotNull(options, nameof(options));
        ArgumentGuard.NotNull(queryStringAccessor, nameof(queryStringAccessor));
        ArgumentGuard.NotNull(parameterReaders, nameof(parameterReaders));

        _options = options;
        _queryStringAccessor = queryStringAccessor;
        _parameterReaders = parameterReaders;
        _logger = loggerFactory.CreateLogger<QueryStringReader>();
    }

    /// <inheritdoc />
    public virtual void ReadAll(DisableQueryStringAttribute? disableQueryStringAttribute)
    {
        using IDisposable _ = CodeTimingSessionManager.Current.Measure("Parse query string");

        DisableQueryStringAttribute disableQueryStringAttributeNotNull = disableQueryStringAttribute ?? DisableQueryStringAttribute.Empty;

        foreach ((string parameterName, StringValues parameterValue) in _queryStringAccessor.Query)
        {
            var result = AlterFilterQuery(parameterName, parameterValue);

            IQueryStringParameterReader? reader = _parameterReaders.FirstOrDefault(nextReader => nextReader.CanRead(result.parameterName));

            if (reader != null)
            {
                _logger.LogDebug($"Query string parameter '{result.parameterName}' with value '{parameterValue}' was accepted by {reader.GetType().Name}.");

                if (!reader.AllowEmptyValue && string.IsNullOrEmpty(result.parameterValue))
                {
                    throw new InvalidQueryStringParameterException(result.parameterName, "Missing query string parameter value.",
                        $"Missing value for '{result.parameterName}' query string parameter.");
                }

                if (!reader.IsEnabled(disableQueryStringAttributeNotNull))
                {
                    throw new InvalidQueryStringParameterException(result.parameterName,
                        "Usage of one or more query string parameters is not allowed at the requested endpoint.",
                        $"The parameter '{result.parameterName}' cannot be used at this endpoint.");
                }
                reader.Read(result.parameterName, result.parameterValue);
                _logger.LogDebug($"Query string parameter '{result.parameterName}' was successfully read.");
            }
            else if (!_options.AllowUnknownQueryStringParameters)
            {
                throw new InvalidQueryStringParameterException(parameterName, "Unknown query string parameter.",
                    $"Query string parameter '{result.parameterName}' is unknown. Set '{nameof(IJsonApiOptions.AllowUnknownQueryStringParameters)}' " +
                    "to 'true' in options to ignore unknown parameters.");
            }
        }
    }

    private (string parameterName, string parameterValue) AlterFilterQuery(string p_parameterName, StringValues p_parameterValue)
    {
        if (!p_parameterName.StartsWith("filter"))
        {
            return (p_parameterName, p_parameterValue);
        }
        //filter[name][test]
        //filter, name], test]
        var stringParts = p_parameterName.Split("[");
        if (stringParts.Length != 3 || !stringParts[1].EndsWith("]") || !stringParts[2].EndsWith("]"))
        {
            return (p_parameterName, p_parameterValue);
        }

        //filter[name]
        string newName = stringParts[0] + "[" + stringParts[1];
        //test:p_parameterValue
        StringValues newValue = stringParts[2].TrimEnd(']') + ":" + p_parameterValue; 

        return (newName, newValue);
    }
}
