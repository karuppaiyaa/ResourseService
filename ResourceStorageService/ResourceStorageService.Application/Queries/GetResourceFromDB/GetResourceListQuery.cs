using MediatR;
using ResourceStorageService.Application.Models;

namespace ResourceStorageService.Application.Queries.GetResourceFromDB
{
    public class GetResourceListQuery : IRequest<FilterResourceDto>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string SearchResourceName { get; set; }
        public string SearchResourcePath { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<string> ResourcePaths { get; set; }
        public List<Guid> ResourceIds { get; set; }
    }
}
