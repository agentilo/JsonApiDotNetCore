namespace JsonApiDotNetCore.Authorization
{
    public class FilterResourceResult<TResource>
    {
        public AuthorizationResult AuthResult { get; set; }
        public ICollection<TResource> Resources { get; set; }
    }
}
