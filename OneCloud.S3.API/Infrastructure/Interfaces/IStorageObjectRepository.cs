namespace OneCloud.S3.API.Infrastructure.Interfaces;

/// <summary>
/// Object repository
/// </summary>
public interface IStorageObjectRepository
{
    /// <summary>
    /// Get object
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> GetObjectAsync(string bucket, string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Get object and write to file
    /// </summary>
    /// <param name="bucket">Source bucket name</param>
    /// <param name="filePath">Source object key</param>
    /// <param name="localPath">Target local file path</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> GetObjectToFileAsync(string bucket, string filePath, string localPath, CancellationToken cancellationToken);

    /// <summary>
    /// Upload object
    /// </summary>
    /// <param name="file">File</param>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> PutObjectAsync(string bucket, string filePath, IFormFile file, CancellationToken cancellationToken);

    /// <summary>
    /// Delete object
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteObjectAsync(string bucket, string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Copy object
    /// </summary>
    /// <param name="srcBucket">Source bucket name</param>
    /// <param name="srcFilePath">Source object key</param>
    /// <param name="destBucket">Target bucket name</param>
    /// <param name="destFilePath">Target object key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CopyObjectAsync(string srcBucket, string srcFilePath, string destBucket, string destFilePath, CancellationToken cancellationToken);

    /// <summary>
    /// Get object public link
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="expires">Date link expires</param>
    /// <returns></returns>
    string GetPreSignedUrl(string bucket, string filePath, DateTime expires);

    /// <summary>
    /// Change object permissions
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="isPublicRead">Is object public</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> PutAclAsync(string bucket, string filePath, bool isPublicRead, CancellationToken cancellationToken);
}