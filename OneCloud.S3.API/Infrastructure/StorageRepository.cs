using Amazon.S3;
using Amazon.S3.Model;
using OneCloud.S3.API.Infrastructure.Interfaces;

namespace OneCloud.S3.API.Infrastructure;

public class StorageRepository : IStorageRepository
{
    private readonly IAmazonS3 _client;

    public StorageRepository(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentException.ThrowIfNullOrEmpty(configuration["S3_ACCESS_KEY"]);
        ArgumentException.ThrowIfNullOrEmpty(configuration["S3_SECRET_KEY"]);
        ArgumentException.ThrowIfNullOrEmpty(configuration["S3_SERVICE_URL"]);

        _client = new AmazonS3Client(configuration["S3_ACCESS_KEY"], configuration["S3_SECRET_KEY"], new AmazonS3Config
        {
            ServiceURL = configuration["S3_SERVICE_URL"],
            ForcePathStyle = true, // HACK: Don't work without this property!
        });
    }

    #region IStorageBucketRepository

    public async Task<List<S3Bucket>> ListBucketsAsync(CancellationToken cancellationToken)
    {
        var result = await _client.ListBucketsAsync(cancellationToken);
        return result.Buckets;
    }

    public async Task<List<S3Object>> ListBucketContentAsync(string bucket, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));

        var request = new ListObjectsRequest { BucketName = bucket };
        var response = await _client.ListObjectsAsync(request, cancellationToken);
        return response.S3Objects;
    }

    public async Task<bool> PutBucketAsync(string bucket, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));

        var request = new PutBucketRequest { BucketName = bucket };
        var response = await _client.PutBucketAsync(request, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> DeleteBucketAsync(string bucket, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        var request = new DeleteBucketRequest { BucketName = bucket };
        var response = await _client.DeleteBucketAsync(request, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    #endregion

    #region IStorageObjectRepository

    public async Task<Stream> GetObjectAsync(string bucket, string objectKey, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        ArgumentException.ThrowIfNullOrEmpty(objectKey, nameof(objectKey));

        var request = new GetObjectRequest { BucketName = bucket, Key = objectKey };
        var response = await _client.GetObjectAsync(request, cancellationToken);
        return response.ResponseStream;
    }

    public async Task<bool> GetObjectToFileAsync(string bucket, string objectKey, string localFilePath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        ArgumentException.ThrowIfNullOrEmpty(objectKey, nameof(objectKey));
        ArgumentException.ThrowIfNullOrEmpty(localFilePath, nameof(localFilePath));
        if(!Path.IsPathFullyQualified(localFilePath))
            throw new ArgumentException("Path must be Qualified", nameof(localFilePath));

        var request = new GetObjectRequest { BucketName = bucket, Key = objectKey };
        var response = await _client.GetObjectAsync(request, cancellationToken);
        await response.WriteResponseStreamToFileAsync(localFilePath, true, cancellationToken);
        return true;
    }

    public async Task<bool> PutObjectAsync(string bucket, string objectKey, IFormFile file, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        ArgumentException.ThrowIfNullOrEmpty(objectKey, nameof(objectKey));
        ArgumentNullException.ThrowIfNull(file, nameof(file));

        await using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = bucket,
            Key = objectKey,
            ContentType = file.ContentType,
            InputStream = stream,
            AutoCloseStream = true,
            UseChunkEncoding = false,
        };

        var response = await _client.PutObjectAsync(request, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> DeleteObjectAsync(string bucket, string filePath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

        var request = new DeleteObjectRequest { BucketName = bucket, Key = filePath };
        var response = await _client.DeleteObjectAsync(request, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> CopyObjectAsync(string srcBucket, string srcFilePath, string destBucket, string destFilePath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(srcBucket, nameof(srcBucket));
        ArgumentException.ThrowIfNullOrEmpty(srcFilePath, nameof(srcFilePath));
        ArgumentException.ThrowIfNullOrEmpty(destBucket, nameof(destBucket));
        ArgumentException.ThrowIfNullOrEmpty(destFilePath, nameof(destFilePath));

        var request = new CopyObjectRequest
        {
            SourceBucket = srcBucket,
            SourceKey = srcFilePath,
            DestinationBucket = destBucket,
            DestinationKey = destFilePath,
        };

        var response = await _client.CopyObjectAsync(request, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public string GetPreSignedUrl(string bucket, string filePath, DateTime expires)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = filePath,
            Expires = expires,
            Protocol = Protocol.HTTPS
        };

        var response = _client.GetPreSignedURL(request);
        return response;
    }

    public async Task<bool> PutAclAsync(string bucket, string filePath, bool isPublicRead, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(bucket, nameof(bucket));
        ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

        var request = new PutACLRequest
        {
            BucketName = bucket,
            Key = filePath,
            CannedACL = isPublicRead
                ? S3CannedACL.PublicRead
                : S3CannedACL.Private,
        };

        var response = await _client.PutACLAsync(request, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    #endregion
}