using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using ResourceStorageService.Application.Services;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Infrastructure.DBContext;
using ResourceStorageService.Infrastructure.Models;

namespace ResourceStorageService.Application.Commands
{
    public class UploadResourceCommandHandler : IRequestHandler<UploadMultipleResourcesCommand, List<ResourceDto>>
    {
        public readonly IConfiguration _configuration;
        private readonly ResourceStorageContext _context;
        private readonly IMapper _mapper;
        public UploadResourceCommandHandler(ResourceStorageContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<List<ResourceDto>> Handle(UploadMultipleResourcesCommand request, CancellationToken cancellationToken)
        {
            List<ResourceDto> resources = new();
            List<ResourceStorageEntity> resourcesData = new();
            foreach (var resource in request.Resources)
            {
                var path = (_configuration["Name"] ?? "") + (string.IsNullOrEmpty(request.ResourcePath) ? '/' : ('/' + request.ResourcePath + '/')) + Guid.NewGuid().ToString() + '/' + resource.FileName;

                var IsSuccess = await (new S3Services(_configuration)).UploadResource(new UploadResourceCommand() { Resource = resource, ResourcePath = path });

                if (IsSuccess == "200")
                {
                    resourcesData.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Path = path,
                        Name = resource.FileName,
                        SizeInBytes = resource.Length,
                        Type = Path.GetExtension(resource.FileName),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            if (resourcesData.Any())
            {
                _context.Resources = resourcesData;
                await _context.SaveDBAsync(cancellationToken);
                resources = _mapper.Map<List<ResourceDto>>(resourcesData);
            }
            return resources;
        }
    }
}






