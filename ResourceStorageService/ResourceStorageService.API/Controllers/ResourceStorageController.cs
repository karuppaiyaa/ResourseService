using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceStorageService.Application.Commands;
using ResourceStorageService.Application.DataTransferObjects;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Application.Queries;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using System.IO.Compression;
using System.Net;

namespace ResourceStorageService.API.Controllers
{
    [ApiController]
    [Route("/v{apiVersion}/resources")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ResourceStorageController : ControllerBase
    {
        private readonly ILogger<ResourceStorageController> _logger;
        private readonly IMediator _mediator;
        public readonly IConfiguration _configuration;

        public ResourceStorageController(ILogger<ResourceStorageController> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Upload resource to AWS
        /// </summary>
        /// <param name="request">Create resource payload</param>
        /// <returns>201 - Created - resource data</returns>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(ResourceDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadResource([FromForm] UploadResourceCommand request)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if (request.Resource == null)
                return BadRequest("Resource must not be null or empty");
            try
            {
                return Created(Request.Path.Value, (await _mediator.Send(new UploadMultipleResourcesCommand() { ResourcePath = request.ResourcePath, Resources = new() { request.Resource } })).FirstOrDefault());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadResource");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Upload list of resources to storage provider
        /// </summary>
        /// <param name="request">Create bulk resource payload</param>
        /// <returns>201 - Created - list of resource data</returns>
        [HttpPost("bulkupload")]
        [ProducesResponseType(typeof(List<ResourceDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadMultipleResources([FromForm] UploadMultipleResourcesCommand request)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            try
            {
                return Created(Request.Path.Value, await _mediator.Send(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadMultipleResources");
                return StatusCode(500, "Error in processing request");
            }

        }

        /// <summary>
        /// Get list of resources details - if limit is negative then retrieve all resources data
        /// </summary>
        /// <param name="request">Get list of resources data</param>
        /// <returns>200 - Ok - paged response of list of resources data</returns>
        [HttpPost("filter")]
        [ProducesResponseType(typeof(FilterResourceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllResources([FromBody] GetResourceListQuery request)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            try
            {
                return Ok(await _mediator.Send(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllResources");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Get resource details by resource id
        /// </summary>
        /// <param name="resourceId">Get resource details by resource id</param>
        /// <param name="resourcePath">Get resource details by resource path</param>
        /// <returns>200 - Ok - resource details</returns>
        [HttpGet("getresource")]
        [ProducesResponseType(typeof(ResourceDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetResource([FromQuery] Guid resourceId, [FromQuery] string resourcePath)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if (resourceId == Guid.Empty && string.IsNullOrEmpty(resourcePath))
                return BadRequest("ResourceId or ResourcePath should not be null or empty");
            if (resourceId != Guid.Empty && !string.IsNullOrEmpty(resourcePath))
                return BadRequest("Select any one resourceId or resourcePath");
            try
            {
                var resource = await _mediator.Send(new GetResourceQuery(resourceId, resourcePath));
                if (resource == null)
                    return NotFound("Requested resource not found");
                return Ok(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetResource");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Get full resource paths by passing resource-paths or resource-ids for multiple resources
        /// </summary>
        /// <param name="request">Get full resource paths by resource-paths or resource-ids</param>
        /// <returns>200 - Ok - list of full resource paths</returns>
        [HttpPost("fullpaths")]
        [ProducesResponseType(typeof(List<ResourcePathDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFullPaths([FromBody]GetResourceDetailsListQuery request)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if ((request.ResourcePaths == null || request.ResourcePaths.All(x => string.IsNullOrEmpty(x))) && (request.ResourceIds == null || request.ResourceIds.All(x => x == Guid.Empty)))
                return BadRequest("ResourceIds or ResourcePaths should not be null or empty");
            try
            {
                return Ok(await _mediator.Send(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFullPaths");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Delete resource details by resource id
        /// </summary>
        /// <param name="resourceId">Delete resource details by resource id</param>
        /// <param name="resourcePath">Delete resource details by resource path</param>
        /// <returns>204 - No Content</returns>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteResource([FromQuery] Guid resourceId, [FromQuery] string resourcePath)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if (resourceId == Guid.Empty && string.IsNullOrEmpty(resourcePath))
                return BadRequest("ResourceId or ResourcePath should not be null or empty");
            if (resourceId != Guid.Empty && !string.IsNullOrEmpty(resourcePath))
                return BadRequest("Select any one resourceId or resourcePath");
            try
            {
                var resource = await _mediator.Send(new DeleteResourcesCommand(resourceId, resourcePath));
                if (resource == null)
                    return NotFound("Requested resource not found");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteResource");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Delete list of resources by passing resource-paths or resource-ids for multiple resources
        /// </summary>
        /// <param name="request">Delete list of resources by resource-paths or resource-ids</param>
        /// <returns>204 - No Content</returns>
        [HttpDelete("bulkdelete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMultipleResources([FromBody] DeleteMultiResourceCommand request)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if ((request.ResourcePaths == null || request.ResourcePaths.All(x => string.IsNullOrEmpty(x))) && (request.ResourceIds == null || request.ResourceIds.All(x => x == Guid.Empty)))
                return BadRequest("ResourceIds or ResourcePaths should not be null or empty");
            try
            {
                await _mediator.Send(request);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteMultipleResources");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Download resource details by resource id
        /// </summary>
        /// <param name="resourceId">Download resource details by resource id</param>
        /// <param name="resourcePath">Download resource details by resource path</param>
        /// <returns>200 - Ok - resource</returns>
        [HttpGet("download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DownloadResource([FromQuery] Guid resourceId, [FromQuery] string resourcePath)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if (resourceId == Guid.Empty && string.IsNullOrEmpty(resourcePath))
                return BadRequest("ResourceId or ResourcePath should not be null or empty");
            if (resourceId != Guid.Empty && !string.IsNullOrEmpty(resourcePath))
                return BadRequest("Select any one resourceId or resourcePath");
            try
            {
                var resource = await _mediator.Send(new DownloadResourceQuery(resourceId, resourcePath));
                if (resource.fileBytes != null && resource.fileBytes.Any())
                {
                    return new FileContentResult(resource.fileBytes, resource.contentType);
                }
                return NotFound("Requested resource not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DownloadResource");
                return StatusCode(500, "Error in processing request");
            }
        }

        /// <summary>
        /// Download list of resources as zip by passing resource-paths or resource-ids for multiple resources
        /// </summary>
        /// <param name="request">Download list of resources as zip by resource-paths or resource-ids</param>
        /// <returns>200 - Ok - download list of resources as zip</returns>
        [HttpPost("bulkdownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DownloadMultipleResources([FromBody] DownloadMultipleResourcesQuery request)
        {
            if (_configuration["Name"] != Request.Headers["Name"] && _configuration["Password"] != Request.Headers["Password"])
                return Unauthorized("Un-Authorized user");
            if ((request.ResourcePaths == null || request.ResourcePaths.All(x => string.IsNullOrEmpty(x))) && (request.ResourceIds == null || request.ResourceIds.All(x => x == Guid.Empty)))
                return BadRequest("ResourceIds or ResourcePaths should not be null or empty");
            try
            {
                var resources = await _mediator.Send(request);

                if (resources != null && resources.Any())
                {
                    using (MemoryStream ms = new())
                    {
                        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                        {
                            foreach (var resource in resources)
                            {
                                var entry = archive.CreateEntry(resource.fileName, CompressionLevel.Fastest);
                                using (var zipStream = entry.Open())
                                {
                                    zipStream.Write(resource.fileBytes, 0, resource.fileBytes.Length);
                                }
                            }
                        }
                        return new FileContentResult(ms.ToArray(), "application/zip");
                    }
                }
                return NotFound("Requested resources not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DownloadMultipleResources");
                return StatusCode(500, "Error in processing request");
            }
        }
    }
}