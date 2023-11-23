using MediatR;
using ResourceStorageService.Application.DataTransferObjects;

namespace ResourceStorageService.Application.Queries
{
    public class GetResourceDetailsListQuery : IRequest<List<ResourcePathDto>>
    {
        public List<Guid> ResourceIds { get; set; }
        public List<string> ResourcePaths { get; set; }
    }
}
