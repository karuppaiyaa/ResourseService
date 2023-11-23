using MediatR;

namespace ResourceStorageService.Application.Commands
{
    public class DeleteResourcesCommand : IRequest<string>
    {
        public DeleteResourcesCommand(Guid resourceId, string resourcePath)
        {
            ResourceId = resourceId;
            ResourcePath = resourcePath ?? "";
        }

        public Guid ResourceId { get; set; }
        public string ResourcePath { get; set; }
    }
}
