using MongoDB.Driver;
using ResourceStorageService.Infrastructure.Database;
using ResourceStorageService.Infrastructure.Models;

namespace ResourceStorageService.Infrastructure.DBContext
{
    public class ResourceStorageContext
    {
        private readonly IMongoConnection _mongoConnectionFactory;
        public List<ResourceStorageEntity> Resources { get; set; }
        public List<Guid> ResourceIds { get; set; } = new();
        public ResourceStorageContext(IMongoConnection mongoConnectionFactory)
        {
            _mongoConnectionFactory = mongoConnectionFactory;
        }

        public async Task<bool> SaveDBAsync(CancellationToken cancellationToken = default)
        {
            if (Resources.Any())
            {
                await _mongoConnectionFactory.GetConnection().GetCollection<ResourceStorageEntity>("ResourceStorage").InsertManyAsync(Resources, cancellationToken: cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<string> DeleteRecordAsync(CancellationToken cancellationToken = default)
        {
            if (ResourceIds.Any())
            {
                var filter = Builders<ResourceStorageEntity>.Filter.Where(x => ResourceIds.Contains(x.Id));
                var result = await _mongoConnectionFactory.GetConnection().GetCollection<ResourceStorageEntity>("ResourceStorage").DeleteManyAsync(filter, cancellationToken: cancellationToken);
                return result.DeletedCount.ToString();
            }
            else
                return "0";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
