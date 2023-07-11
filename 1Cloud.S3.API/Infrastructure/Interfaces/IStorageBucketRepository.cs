using Amazon.S3.Model;

namespace OneCloud.S3.API.Infrastructure.Interfaces;

/// <summary>
/// Buckets repository
/// </summary>
public interface IStorageBucketRepository
{
    /// <summary>
    /// Get list of buckets
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<S3Bucket>> ListBucketsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get bucket objects list
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<S3Object>> ListBucketContentAsync(string bucket, CancellationToken cancellationToken);

    /// <summary>
    /// Create bucket
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> PutBucketAsync(string bucket, CancellationToken cancellationToken);

    /// <summary>
    /// Delete bucket (need to be empty)
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteBucketAsync(string bucket, CancellationToken cancellationToken);
}
