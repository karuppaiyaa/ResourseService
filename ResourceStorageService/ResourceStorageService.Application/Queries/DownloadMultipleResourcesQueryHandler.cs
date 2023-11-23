using MediatR;
using ResourceStorageService.Application.Services;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using Microsoft.Extensions.Configuration;

namespace ResourceStorageService.Application.Queries
{
    public class DownloadMultipleResourcesQueryHandler : IRequestHandler<DownloadMultipleResourcesQuery, List<(byte[] fileBytes, string contentType, string fileName)>>
    {
        private readonly IMediator _mediator;
        public readonly IConfiguration _configuration;
        public DownloadMultipleResourcesQueryHandler(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }
        public async Task<List<(byte[] fileBytes, string contentType, string fileName)>> Handle(DownloadMultipleResourcesQuery request, CancellationToken cancellationToken)
        {
            var resources = await _mediator.Send(new GetResourceListQuery() { ResourceIds = request.ResourceIds, ResourcePaths = request.ResourcePaths, Limit = -1 });
            if (!resources.Resources.Any())
                return null;

            request.ResourcePaths = resources.Resources.Select(x => x.Path).ToList();
            return await (new S3Services(_configuration)).DownloadMultipleResources(request);
        }
    }
}
