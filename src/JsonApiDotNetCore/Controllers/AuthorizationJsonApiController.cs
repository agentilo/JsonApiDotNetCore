using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiDotNetCore.Authorization;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Queries.Internal.Parsing;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JsonApiDotNetCore.Controllers
{
    public abstract class AuthorizationJsonApiController<TResource, TId> : JsonApiController<TResource, TId>
where TResource : class, IIdentifiable<TId>
    {
        private IAuthorizationHandler m_AuthorizationHandler;

        /// <inheritdoc />
        protected AuthorizationJsonApiController(IJsonApiOptions options, IResourceGraph resourceGraph, ILoggerFactory loggerFactory,
            IResourceService<TResource, TId> resourceService, IAuthorizationHandler authorizationHandler)
            : base(options, resourceGraph, loggerFactory, resourceService)
        {
            m_AuthorizationHandler = authorizationHandler;
        }

        /// <inheritdoc />
        protected AuthorizationJsonApiController(IJsonApiOptions options, IResourceGraph resourceGraph, ILoggerFactory loggerFactory,
            IAuthorizationHandler authorizationHandler,
            IGetAllService<TResource, TId>? getAll = null, IGetByIdService<TResource, TId>? getById = null,
            IGetSecondaryService<TResource, TId>? getSecondary = null, IGetRelationshipService<TResource, TId>? getRelationship = null,
            ICreateService<TResource, TId>? create = null, IAddToRelationshipService<TResource, TId>? addToRelationship = null,
            IUpdateService<TResource, TId>? update = null, ISetRelationshipService<TResource, TId>? setRelationship = null,
            IDeleteService<TResource, TId>? delete = null, IRemoveFromRelationshipService<TResource, TId>? removeFromRelationship = null)
            : base(options, resourceGraph, loggerFactory, getAll, getById, getSecondary, getRelationship, create, addToRelationship, update, setRelationship,
                delete, removeFromRelationship)
        {
            m_AuthorizationHandler = authorizationHandler;
        }

        /// <inheritdoc />
        [HttpGet]
        [HttpHead]
        public override async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            if (!_AllowedToRead(HttpContext))
                throw new UnauthorizedOperationException("GET");
            return await base.GetAsync(cancellationToken);
        }

        private bool _AllowedToRead(HttpContext context)
        {
            bool authorized = false;
            string authHeader = context?.Request?.Headers["Authorization"];
            if (authHeader?.StartsWith("Bearer") == true)
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                authorized = m_AuthorizationHandler.CanRead(token);
            }

            if (authHeader?.StartsWith("Basic") == true)
            {
                string encoded = authHeader.Substring("Basic ".Length).Trim();
                var credentials = _GetUserCredentials(encoded);
                if (credentials.user != null && credentials.pw != null)
                    authorized = m_AuthorizationHandler.CanRead(credentials.user, credentials.pw);
            }

            return authorized;
        }

        private bool _AllowedToWrite(HttpContext context)
        {
            bool authorized = false;
            string authHeader = context?.Request?.Headers["Authorization"];
            if (authHeader?.StartsWith("Bearer") == true)
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                authorized = m_AuthorizationHandler.CanWrite(token);
            }

            if (authHeader?.StartsWith("Basic") == true)
            {
                string encoded = authHeader.Substring("Basic ".Length).Trim();
                var credentials = _GetUserCredentials(encoded);
                if (credentials.user != null && credentials.pw != null)
                    authorized = m_AuthorizationHandler.CanWrite(credentials.user, credentials.pw);
            }

            return authorized;
        }

        private bool _AllowedToManage(HttpContext context)
        {
            bool authorized = false;
            string authHeader = context?.Request?.Headers["Authorization"];
            if (authHeader?.StartsWith("Bearer") == true)
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                authorized = m_AuthorizationHandler.CanManage(token);
            }

            if (authHeader?.StartsWith("Basic") == true)
            {
                string encoded = authHeader.Substring("Basic ".Length).Trim();
                var credentials = _GetUserCredentials(encoded);
                if (credentials.user != null && credentials.pw != null)
                    authorized = m_AuthorizationHandler.CanManage(credentials.user, credentials.pw);
            }

            return authorized;
        }

        private (string user, string pw) _GetUserCredentials(string encodedUser)
        {
            
            string user = null;
            string pw = null;
            try
            {
                string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUser));
                string[] splitted = usernamePassword.Split(":");
                user = splitted[0];
                pw = splitted[1];
            }
            catch { }
            return (user, pw);
        }


        /// <inheritdoc />
        [HttpGet("{id}")]
        [HttpHead("{id}")]
        public override async Task<IActionResult> GetAsync(TId id, CancellationToken cancellationToken)
        {
            if (!_AllowedToRead(HttpContext))
                throw new UnauthorizedOperationException("GET");
            return await base.GetAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        [HttpGet("{id}/{relationshipName}")]
        [HttpHead("{id}/{relationshipName}")]
        public override async Task<IActionResult> GetSecondaryAsync(TId id, string relationshipName, CancellationToken cancellationToken)
        {
            if (!_AllowedToRead(HttpContext))
                throw new UnauthorizedOperationException("GET");
            return await base.GetSecondaryAsync(id, relationshipName, cancellationToken);
        }

        /// <inheritdoc />
        [HttpGet("{id}/relationships/{relationshipName}")]
        [HttpHead("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> GetRelationshipAsync(TId id, string relationshipName, CancellationToken cancellationToken)
        {
            if (!_AllowedToRead(HttpContext))
                throw new UnauthorizedOperationException("GET");
            return await base.GetRelationshipAsync(id, relationshipName, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost]
        public override async Task<IActionResult> PostAsync([FromBody] TResource resource, CancellationToken cancellationToken)
        {
            if (!_AllowedToManage(HttpContext))
                throw new UnauthorizedOperationException("POST");
            return await base.PostAsync(resource, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> PostRelationshipAsync(TId id, string relationshipName, [FromBody] ISet<IIdentifiable> rightResourceIds,
            CancellationToken cancellationToken)
        {
            if (!_AllowedToManage(HttpContext))
                throw new UnauthorizedOperationException("POST");
            return await base.PostRelationshipAsync(id, relationshipName, rightResourceIds, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPatch("{id}")]
        public override async Task<IActionResult> PatchAsync(TId id, [FromBody] TResource resource, CancellationToken cancellationToken)
        {
            if (!_AllowedToWrite(HttpContext))
                throw new UnauthorizedOperationException("PATCH");
            return await base.PatchAsync(id, resource, cancellationToken);
        }

        [HttpPut("values")]
        [HttpPut]
        public override async Task<IActionResult> PutAsync([FromBody] IEnumerable<object?> resource, CancellationToken cancellationToken)
        {
            if (!_AllowedToWrite(HttpContext))
                throw new UnauthorizedOperationException("PUT");
            return await base.PutAsync(resource, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPatch("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> PatchRelationshipAsync(TId id, string relationshipName, [FromBody] object? rightValue,
            CancellationToken cancellationToken)
        {
            if (!_AllowedToWrite(HttpContext))
                throw new UnauthorizedOperationException("PATCH");
            return await base.PatchRelationshipAsync(id, relationshipName, rightValue, cancellationToken);
        }

        /// <inheritdoc />
        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            if (!_AllowedToManage(HttpContext))
                throw new UnauthorizedOperationException("DELETE");
            return await base.DeleteAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        [HttpDelete("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> DeleteRelationshipAsync(TId id, string relationshipName, [FromBody] ISet<IIdentifiable> rightResourceIds,
            CancellationToken cancellationToken)
        {
            if (!_AllowedToManage(HttpContext))
                throw new UnauthorizedOperationException("DELETE");
            return await base.DeleteRelationshipAsync(id, relationshipName, rightResourceIds, cancellationToken);
        }
    }
}

