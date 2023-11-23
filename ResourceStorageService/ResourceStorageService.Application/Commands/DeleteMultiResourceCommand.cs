using MediatR;

namespace ResourceStorageService.Application.Commands
{
    public class DeleteMultiResourceCommand : IRequest<string>
    {
        public List<Guid> ResourceIds { get; set; }
        public List<string> ResourcePaths { get; set; }
    }
}
