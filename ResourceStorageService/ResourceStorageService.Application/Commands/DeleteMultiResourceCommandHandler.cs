using MediatR;
using Microsoft.Extensions.Configuration;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using ResourceStorageService.Application.Services;
using ResourceStorageService.Infrastructure.DBContext;

namespace ResourceStorageService.Application.Commands
{
    public class DeleteMultiResourceCommandHandler : IRequestHandler<DeleteMultiResourceCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ResourceStorageContext _context;
        private readonly IConfiguration _configuration;

        public DeleteMultiResourceCommandHandler(ResourceStorageContext context, IMediator mediator, IConfiguration configuration)
        {
            _context = context;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<string> Handle(DeleteMultiResourceCommand request, CancellationToken cancellationToken)
        {
            var resources = await _mediator.Send(new GetResourceListQuery() { ResourceIds = request.ResourceIds, ResourcePaths = request.ResourcePaths, Limit = -1 });
            if (!resources.Resources.Any())
                return null;

            var result = await (new S3Services(_configuration)).DeleteMultipleResources(resources.Resources.Select(x => x.Path).ToList());

            if (result == "200")
            {
                _context.ResourceIds = resources.Resources.Select(x => x.Id).ToList();
                return await _context.DeleteRecordAsync(cancellationToken);
            }
            return null;
        }
    }
}
