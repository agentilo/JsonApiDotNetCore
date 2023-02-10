using JetBrains.Annotations;
using JsonApiDotNetCore.Controllers.Annotations;
/// <summary>
/// Used on an ASP.NET Core controller class to indicate the PUT verb must be blocked.
/// </summary>
/// <example><![CDATA[
/// [NoHttpPut]
/// public class ArticlesController : BaseJsonApiController<Article>
/// {
/// }
/// ]]></example>
[PublicAPI]
public sealed class NoHttpPutAttribute : HttpRestrictAttribute
{
    protected override string[] Methods { get; } =
    {
            "PUT"
    };
}
