namespace ResourceStorageService.Application.DataTransferObjects
{
    public class ResourcePathDto
    {
        public Guid ResourceId { get; set; }
        public string ResourcePath { get; set; }
        public string FullResourcePath { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
