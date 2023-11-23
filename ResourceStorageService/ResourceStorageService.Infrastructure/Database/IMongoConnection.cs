using MongoDB.Driver;

namespace ResourceStorageService.Infrastructure.Database
{
    public interface IMongoConnection
    {
        IMongoDatabase GetConnection();
    }
}
