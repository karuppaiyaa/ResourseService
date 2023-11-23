using MongoDB.Bson.Serialization.Attributes;

namespace ResourceStorageService.Infrastructure.Models
{
    [BsonIgnoreExtraElements]
    public class ResourceStorageEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long SizeInBytes { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
