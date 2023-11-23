using MediatR;

namespace ResourceStorageService.Application.Commands
{
    public class DeleteResourceCommand : IRequest<string>
    {
        public string ResourcePath { get; set; }
    }
}
