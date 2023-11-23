using MediatR;
using ResourceStorageService.Application.Models;

namespace ResourceStorageService.Application.Queries.GetResourceFromDB
{
    public class GetResourceQuery : IRequest<ResourceDto>
    {
        public Guid ResourceId { get; set; }
        public string ResourcePath { get; set; }

        public GetResourceQuery(Guid resourseId, string resourcePath)
        {
            ResourceId = resourseId;
            ResourcePath = resourcePath ?? "";
        }
    }
}
