using MediatR;
using Microsoft.AspNetCore.Http;
using ResourceStorageService.Application.Models;

namespace ResourceStorageService.Application.Commands
{
    public class UploadMultipleResourcesCommand : IRequest<List<ResourceDto>>
    {
        public string ResourcePath { get; set; }

        public List<IFormFile> Resources { get; set; }
    }
}
