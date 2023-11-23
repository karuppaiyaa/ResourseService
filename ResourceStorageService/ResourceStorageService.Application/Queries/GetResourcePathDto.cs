namespace ResourceStorageService.Application.Queries
{
    public class GetResourcePathDto
    {
        public string ResourcePath { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

