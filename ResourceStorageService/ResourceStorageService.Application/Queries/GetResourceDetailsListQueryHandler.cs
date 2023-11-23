using MediatR;
using ResourceStorageService.Application.DataTransferObjects;
using ResourceStorageService.Application.Services;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using Microsoft.Extensions.Configuration;

namespace ResourceStorageService.Application.Queries
{
    public class GetResourceDetailsListQueryHandler : IRequestHandler<GetResourceDetailsListQuery, List<ResourcePathDto>>
    {
        private readonly IMediator _mediator;
        public readonly IConfiguration _configuration;
        public GetResourceDetailsListQueryHandler(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<List<ResourcePathDto>> Handle(GetResourceDetailsListQuery request, CancellationToken cancellationToken)
        {
            var resources = await _mediator.Send(new GetResourceListQuery() { ResourceIds = request.ResourceIds, ResourcePaths = request.ResourcePaths }, cancellationToken);
            if (resources.Resources.Any())
            {
                return await GetPresignedPathFromS3(resources.Resources);
            }
            return null;
        }

        private async Task<List<ResourcePathDto>> GetPresignedPathFromS3(List<ResourceDto> resources)
        {
            List<ResourcePathDto> resourcePaths = new();
            foreach (var resource in resources)
            {
                var result = await (new S3Services(_configuration)).GetFullUrl(resource.Path);
                resourcePaths.Add(new()
                {
                    ResourceId = resource.Id,
                    ResourcePath = resource.Path,
                    FullResourcePath = result.ResourcePath,
                    ExpiresAt = result.ExpiresAt
                });
            }
            return resourcePaths;
        }
    }
}
