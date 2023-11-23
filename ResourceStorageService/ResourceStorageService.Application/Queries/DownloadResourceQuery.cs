using MediatR;

namespace ResourceStorageService.Application.Queries
{
    public class DownloadResourceQuery : IRequest<(byte[] fileBytes, string contentType, string fileDownloadName, DateTimeOffset? lastModified)>
    {
        public DownloadResourceQuery(Guid resourceId, string resourcePath)
        {
            ResourceId = resourceId;
            ResourcePath = resourcePath;
        }

        public Guid ResourceId { get; set; }
        public string ResourcePath { get; set; }
    }
}





