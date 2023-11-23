using MediatR;

namespace ResourceStorageService.Application.Queries
{
    public class DownloadMultipleResourcesQuery : IRequest<List<(byte[] fileBytes, string contentType, string fileName)>>
    {
        public List<Guid> ResourceIds { get; set; }
        public List<string> ResourcePaths { get; set; }
    }
}
