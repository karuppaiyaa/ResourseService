using MediatR;
using MongoDB.Driver;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Infrastructure.Database;

namespace ResourceStorageService.Application.Queries.GetResourceFromDB
{
    public class GetResourceQueryHandler : IRequestHandler<GetResourceQuery, ResourceDto>
    {
        private readonly IMongoConnection _mongoConnectionFactory;

        public GetResourceQueryHandler(IMongoConnection mongoConnectionFactory)
        {
            _mongoConnectionFactory = mongoConnectionFactory;
        }

        public async Task<ResourceDto> Handle(GetResourceQuery request, CancellationToken cancellationToken)
        {
            var collection = _mongoConnectionFactory.GetConnection().GetCollection<ResourceDto>("ResourceStorage");
            var filter = Builders<ResourceDto>.Filter.Empty;
            if (!string.IsNullOrEmpty(request.ResourcePath))
            {
                filter &= Builders<ResourceDto>.Filter.Where(x => x.Path.ToLower() == request.ResourcePath.ToLower());
            }
            if (request.ResourceId != Guid.Empty)
            {
                filter &= Builders<ResourceDto>.Filter.Where(x => x.Id == request.ResourceId);
            }
            return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}
