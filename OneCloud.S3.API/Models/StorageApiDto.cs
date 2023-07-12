namespace OneCloud.S3.API.Models;

public record StorageApiDto(int Id, string ExternalId, string Name, string Password, object SwiftApiConnection, object S3Connection, object FtpConnection, string State, object Tasks);
