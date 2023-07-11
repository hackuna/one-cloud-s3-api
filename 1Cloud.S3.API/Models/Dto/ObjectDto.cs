namespace OneCloud.S3.API.Models.Dto;

public class ObjectDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string ETag { get; set; } = null!;
    public DateTime LastModified { get; set; }
    public long Size { get; set; }
}
