using MediatR;
using Microsoft.Extensions.Configuration;
using ResourceStorageService.Application.Services;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using ResourceStorageService.Infrastructure.DBContext;

namespace ResourceStorageService.Application.Commands
{
    public class DeleteResourcesCommandHandler : IRequestHandler<DeleteResourcesCommand, string>
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly ResourceStorageContext _context;

        public DeleteResourcesCommandHandler(IMediator mediator, ResourceStorageContext context, IConfiguration configuration)
        {
            _mediator = mediator;
            _context = context;
            _configuration = configuration;
        }
        public async Task<string> Handle(DeleteResourcesCommand request, CancellationToken cancellationToken)
        {
            var resource = await _mediator.Send(new GetResourceQuery(request.ResourceId, request.ResourcePath));
            if (resource == null)
                return null;    

            var result = await (new S3Services(_configuration)).DeleteResource(new DeleteResourceCommand() { ResourcePath = resource.Path });

            if (result == "200")
            {
                _context.ResourceIds = new() { resource.Id };
                return await _context.DeleteRecordAsync(cancellationToken);
            }
            return null;
        }
    }
}
