using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer.Localisation;
using JsonApiDotNetCore.Authorization;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Queries.Internal.Parsing;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace JsonApiDotNetCore.Controllers
{
    public abstract class AuthorizationJsonApiController<TResource, TId> : JsonApiController<TResource, TId>
where TResource : class, IIdentifiable<TId>
    {
        private IAuthorizationHandler<TId> _authorizationHandler;

        /// <inheritdoc />
        protected AuthorizationJsonApiController(IJsonApiOptions options, IResourceGraph resourceGraph, ILoggerFactory loggerFactory,
            IResourceService<TResource, TId> resourceService, IAuthorizationHandler<TId> authorizationHandler)
            : base(options, resourceGraph, loggerFactory, resourceService)
        {
            _authorizationHandler = authorizationHandler;
        }

        /// <inheritdoc />
        protected AuthorizationJsonApiController(IJsonApiOptions options, IResourceGraph resourceGraph, ILoggerFactory loggerFactory,
            IAuthorizationHandler<TId> authorizationHandler,
            IGetAllService<TResource, TId>? getAll = null, IGetByIdService<TResource, TId>? getById = null,
            IGetSecondaryService<TResource, TId>? getSecondary = null, IGetRelationshipService<TResource, TId>? getRelationship = null,
            ICreateService<TResource, TId>? create = null, IAddToRelationshipService<TResource, TId>? addToRelationship = null,
            IUpdateService<TResource, TId>? update = null, ISetRelationshipService<TResource, TId>? setRelationship = null,
            IDeleteService<TResource, TId>? delete = null, IRemoveFromRelationshipService<TResource, TId>? removeFromRelationship = null)
            : base(options, resourceGraph, loggerFactory, getAll, getById, getSecondary, getRelationship, create, addToRelationship, update, setRelationship,
                delete, removeFromRelationship)
        {
            _authorizationHandler = authorizationHandler;
        }

        /// <inheritdoc />
        [HttpGet]
        [HttpHead]
        public override async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("GET");

            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("GET");

            var result = await base.GetAsync(cancellationToken);

            if (result is OkObjectResult okResult)
            {
                IReadOnlyCollection<TResource> resources = (IReadOnlyCollection<TResource>)okResult.Value;

                if (resources == null || resources.Count == 0)
                {
                    var isAllowedResult = _authorizationHandler.IsAllowed(cred);

                    _CheckResult(isAllowedResult);
                    return result; //Nichts da was authorisiert werden muss.
                }
                else
                {
                    var authResult = _authorizationHandler.FilterResourcesForRead<TResource>(cred, resources);

                    if (authResult == null)
                        throw new UnauthorizedOperationException("GET");

                    _CheckResult(authResult.AuthResult);

                    ICollection<TResource> filteredResourceList = authResult.Resources;
                    if (filteredResourceList == null || filteredResourceList.Count == 0)
                    {
                        throw new ForbiddenOperationException();
                    }
                    return Ok(filteredResourceList);
                }
            }
            else
            {
                throw new UnauthorizedOperationException("GET");
            }
        }

        private void _CheckResult(AuthorizationResult authResult)
        {
            switch (authResult)
            {
                case AuthorizationResult.Expired:
                    throw new BearerTokenExpiredException();
                case AuthorizationResult.Forbidden:
                    throw new ForbiddenOperationException();
                case AuthorizationResult.Unauthorized:
                    throw new UnauthorizedOperationException();
                case AuthorizationResult.NotFound:
                    throw new NotFoundException();
            }
        }


        /// <inheritdoc />
        [HttpGet("{id}")]
        [HttpHead("{id}")]
        public override async Task<IActionResult> GetAsync(TId id, CancellationToken cancellationToken)
        {
            var response = await base.GetAsync(id, cancellationToken);

            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("GET");

            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("GET");
            var authResult = _authorizationHandler.IsAllowedToRead(id, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new ForbiddenOperationException();
            return response;
        }

        /// <inheritdoc />
        [HttpGet("{id}/{relationshipName}")]
        [HttpHead("{id}/{relationshipName}")]
        public override async Task<IActionResult> GetSecondaryAsync(TId id, string relationshipName, CancellationToken cancellationToken)
        {
            var response = await base.GetSecondaryAsync(id, relationshipName, cancellationToken);
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("GET");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("GET");


            if (response is OkObjectResult okResult)
            {
                if (okResult.Value is IEnumerable)
                {
                    var resourceList = okResult.Value as IReadOnlyCollection<IIdentifiable<TId>>;

                    if (resourceList == null || resourceList.Count == 0)
                    {
                        var isAllowedResult = _authorizationHandler.IsAllowed(cred);

                        _CheckResult(isAllowedResult);
                        return response; //Nichts da was authorisiert werden muss.
                    }
                    else
                    {
                        var authResult = _authorizationHandler.FilterResourcesForRead<IIdentifiable<TId>>(cred, resourceList);

                        if (authResult == null)
                            throw new UnauthorizedOperationException("GET");

                        _CheckResult(authResult.AuthResult);

                        ICollection<IIdentifiable<TId>> filteredResourceList = authResult.Resources;
                        if (filteredResourceList == null || filteredResourceList.Count == 0)
                        {
                            throw new ForbiddenOperationException();
                        }
                        return Ok(filteredResourceList);
                    }
                }
                else
                {
                    if (cred == null)
                        throw new UnauthorizedOperationException("GET");

                    if (okResult.Value is IIdentifiable<TId> identifiable)
                    {
                        var authResult = _authorizationHandler.IsAllowedToRead(identifiable.Id, cred);

                        _CheckResult(authResult);
                        if (authResult != AuthorizationResult.OK)
                            throw new ForbiddenOperationException();
                        return response;
                    }
                }
            }
            

            return response;
        }

        /// <inheritdoc />
        [HttpGet("{id}/relationships/{relationshipName}")]
        [HttpHead("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> GetRelationshipAsync(TId id, string relationshipName, CancellationToken cancellationToken)
        {
            var response = await base.GetSecondaryAsync(id, relationshipName, cancellationToken);
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("GET");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("GET");


            if (response is OkObjectResult okResult)
            {
                if (okResult.Value is IEnumerable)
                {
                    var resourceList = okResult.Value as IReadOnlyCollection<IIdentifiable<TId>>;

                    if (resourceList == null || resourceList.Count == 0)
                    {
                        var isAllowedResult = _authorizationHandler.IsAllowed(cred);

                        _CheckResult(isAllowedResult);
                        return response; //Nichts da was authorisiert werden muss.
                    }
                    else
                    {
                        var authResult = _authorizationHandler.FilterResourcesForRead<IIdentifiable<TId>>(cred, resourceList);

                        if (authResult == null)
                            throw new UnauthorizedOperationException("GET");

                        _CheckResult(authResult.AuthResult);

                        ICollection<IIdentifiable<TId>> filteredResourceList = authResult.Resources;
                        if (filteredResourceList == null || filteredResourceList.Count == 0)
                        {
                            throw new ForbiddenOperationException();
                        }
                        return Ok(filteredResourceList);
                    }
                }
                else
                {
                    if (cred == null)
                        throw new UnauthorizedOperationException("GET");

                    if (okResult.Value is IIdentifiable<TId> identifiable)
                    {
                        var authResult = _authorizationHandler.IsAllowedToRead(identifiable.Id, cred);

                        _CheckResult(authResult);
                        if (authResult != AuthorizationResult.OK)
                            throw new ForbiddenOperationException();
                        return response;
                    }
                }
            }


            return response;
        }

        /// <inheritdoc />
        [HttpPost]
        public override async Task<IActionResult> PostAsync([FromBody] TResource resource, CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("POST");
            //if (!_AllowedToManage(HttpContext))
            //    throw new UnauthorizedOperationException("POST");

            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("POST");
            var authResult = _authorizationHandler.IsAllowedToManage(resource, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("POST");
            var result = await base.PostAsync(resource, cancellationToken);

            if (result is CreatedResult createdResult)
            {
                if (createdResult.Value is TResource createdResouce)
                    _authorizationHandler.ResourceCreated(cred, createdResouce.Id);
            }

            return result;
        }

        /// <inheritdoc />
        [HttpPost("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> PostRelationshipAsync(TId id, string relationshipName, [FromBody] ISet<IIdentifiable> rightResourceIds,
            CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("POST");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("POST");

            var authResult = _authorizationHandler.IsAllowedToManage(id, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("POST");
            return await base.PostRelationshipAsync(id, relationshipName, rightResourceIds, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPatch("{id}")]
        public override async Task<IActionResult> PatchAsync(TId id, [FromBody] TResource resource, CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("PATCH");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("PATCH");

            var authResult = _authorizationHandler.IsAllowedToManage(id, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("PATCH");
            return await base.PatchAsync(id, resource, cancellationToken);
        }

        [HttpPut("values")]
        [HttpPut]
        public override async Task<IActionResult> PutAsync([FromBody] IEnumerable<object?> resource, CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("PUT");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("PUT");
            List<TResource> tList = new List<TResource>();

            foreach (var obj in resource)
            {
                if (obj is TResource)
                    tList.Add(obj as TResource);
            }

            var authResult = _authorizationHandler.IsAllowedToWrite<TResource>(tList, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("PUT");
            return await base.PutAsync(resource, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPatch("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> PatchRelationshipAsync(TId id, string relationshipName, [FromBody] object? rightValue,
            CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("PATCH");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("PATCH");
            var authResult = _authorizationHandler.IsAllowedToManage(id, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("PATCH");
            return await base.PatchRelationshipAsync(id, relationshipName, rightValue, cancellationToken);
        }

        /// <inheritdoc />
        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("DELETE");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("DELETE");

            var authResult = _authorizationHandler.IsAllowedToManage(id, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("DELETE");
            return await base.DeleteAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        [HttpDelete("{id}/relationships/{relationshipName}")]
        public override async Task<IActionResult> DeleteRelationshipAsync(TId id, string relationshipName, [FromBody] ISet<IIdentifiable> rightResourceIds,
            CancellationToken cancellationToken)
        {
            if (_authorizationHandler == null)
                throw new UnauthorizedOperationException("DELETE");
            AuthCredentialReader reader = new AuthCredentialReader();

            AuthCredentials? cred = reader.GetAuthCredentials(HttpContext);
            if (cred == null)
                throw new UnauthorizedOperationException("DELETE");

            var authResult = _authorizationHandler.IsAllowedToManage(id, cred);

            _CheckResult(authResult);
            if (authResult != AuthorizationResult.OK)
                throw new UnauthorizedOperationException("DELETE");
            return await base.DeleteRelationshipAsync(id, relationshipName, rightResourceIds, cancellationToken);
        }
    }
}

