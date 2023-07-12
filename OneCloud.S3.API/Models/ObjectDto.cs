namespace OneCloud.S3.API.Models;

public record ObjectDto(string BucketName, string Key, string ETag, DateTime LastModified, long Size);
