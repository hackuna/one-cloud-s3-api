namespace OneCloud.S3.API.Models.Dto
{
    public class ObjectDto
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string ETag { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}
