namespace OneCloud.S3.API.Models.Configuration
{
    public record StorageOptions
    {
        public string AccessKey { get; init; }
        public string SecretKey { get; init; }
        public string ServiceUrl { get; init; }
    }
}
