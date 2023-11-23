using MediatR;
using Microsoft.AspNetCore.Http;
using ResourceStorageService.Application.Models;

namespace ResourceStorageService.Application.Commands
{
    public class UploadResourceCommand : IRequest<ResourceDto>
    {
        public string ResourcePath { get; set; }
        public IFormFile Resource { get; set; }
    }
}
