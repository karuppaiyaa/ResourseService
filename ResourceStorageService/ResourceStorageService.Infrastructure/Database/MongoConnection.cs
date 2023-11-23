using MongoDB.Driver;

namespace ResourceStorageService.Infrastructure.Database
{
    public class MongoConnection : IMongoConnection, IDisposable
    {
        private readonly MongoClient _client;
        private readonly MongoUrl _mongoUrl;
        private IMongoDatabase _mongoDatabase;

        public MongoConnection(string connection)
        {
            _mongoUrl = new MongoUrl(connection);
            MongoClientSettings _settings = MongoClientSettings.FromUrl(_mongoUrl);
            _settings.AllowInsecureTls = true;
            _client = new MongoClient(_settings);
        }

        public IMongoDatabase GetConnection()
        {
            if (_mongoDatabase == null)
                _mongoDatabase = _client.GetDatabase(_mongoUrl.DatabaseName);
            return _mongoDatabase;            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}