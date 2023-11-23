using MongoDB.Bson.Serialization.Attributes;

namespace ResourceStorageService.Application.Models
{
    [BsonIgnoreExtraElements]
    public class ResourceDto
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public long SizeInBytes { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class FilterResourceDto
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<ResourceDto> Resources { get; set; }
    }
}
