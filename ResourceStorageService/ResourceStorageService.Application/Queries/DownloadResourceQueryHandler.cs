using MediatR;
using ResourceStorageService.Application.Services;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using Microsoft.Extensions.Configuration;

namespace ResourceStorageService.Application.Queries
{
    public class DownloadResourceQueryHandler : IRequestHandler<DownloadResourceQuery, (byte[] fileBytes, string contentType, string fileDownloadName, DateTimeOffset? lastModified)>
    {
        public readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        public DownloadResourceQueryHandler(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }
        public async Task<(byte[] fileBytes, string contentType, string fileDownloadName, DateTimeOffset? lastModified)> Handle(DownloadResourceQuery request, CancellationToken cancellationToken)
        {
            var resource = await _mediator.Send(new GetResourceQuery(request.ResourceId, request.ResourcePath));
            if (resource == null)
                return new();

            request.ResourcePath = resource.Path;
            return await (new S3Services(_configuration)).DownloadResource(request);
        }
    }
}


