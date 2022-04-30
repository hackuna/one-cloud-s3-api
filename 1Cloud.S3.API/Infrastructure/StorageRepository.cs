using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using OneCloud.S3.API.Infrastructure.Interfaces;
using OneCloud.S3.API.Models.Configuration;

namespace OneCloud.S3.API.Infrastructure
{
    public class StorageRepository : IStorageRepository
    {
        private readonly ILogger<StorageRepository> _logger;
        private readonly IAmazonS3 _client;

        public StorageRepository(ILogger<StorageRepository> logger, IOptions<StorageOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _client = new AmazonS3Client(options.Value.AccessKey, options.Value.SecretKey, new AmazonS3Config
            {
                ServiceURL = options.Value.ServiceUrl,

                // HACK: Без этого не сработает!
                ForcePathStyle = true,
            });
        }

        #region IStorageBucketRepository

        public async Task<List<S3Bucket>> ListBucketsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _client.ListBucketsAsync(cancellationToken);

                return result.Buckets;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error listing buckets: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<S3Object>> ListBucketContentAsync(string bucket, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));

            try
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucket,
                };

                var response = await _client.ListObjectsAsync(request, cancellationToken);

                return response.S3Objects;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Unable to list objects in {Bucket}: {Message}", bucket, ex.Message);
                throw;
            }
        }

        public async Task<bool> PutBucketAsync(string bucket, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));

            try
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucket,
                };

                var response = await _client.PutBucketAsync(request, cancellationToken);

                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error creating bucket {Bucket}: {Message}", bucket, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteBucketAsync(string bucket, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));

            try
            {
                var request = new DeleteBucketRequest
                {
                    BucketName = bucket,
                };

                var response = await _client.DeleteBucketAsync(request, cancellationToken);

                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Unable to delete {Bucket}: {Message}", bucket, ex.Message);
                throw;
            }
        }

        #endregion

        #region IStorageObjectRepository

        public async Task<Stream> GetObjectAsync(string bucket, string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = filePath,
                };

                var response = await _client.GetObjectAsync(request, cancellationToken);

                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error to get {Object}: {Message}", filePath, ex.Message);
                throw;
            }
        }

        public async Task<bool> GetObjectToFileAsync(string bucket, string filePath, string localFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrEmpty(localFilePath)) throw new ArgumentNullException(nameof(localFilePath));
            if (!Path.IsPathFullyQualified(localFilePath)) throw new ArgumentException("Path must be Qualified", nameof(localFilePath));

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = filePath,
                };

                var response = await _client.GetObjectAsync(request, cancellationToken);

                await response.WriteResponseStreamToFileAsync(localFilePath, true, cancellationToken);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error to get {Object}: {Message}", filePath, ex.Message);
                throw;
            }
        }

        public async Task<bool> PutObjectAsync(string bucket, string filePath, IFormFile file, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (file is null) throw new ArgumentNullException(nameof(file));

            try
            {
                await using var stream = file.OpenReadStream();

                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = filePath,
                    ContentType = file.ContentType,
                    InputStream = stream,
                    AutoCloseStream = true,
                    UseChunkEncoding = false,
                };

                var response = await _client.PutObjectAsync(request, cancellationToken);

                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Unable to upload file: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteObjectAsync(string bucket, string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = filePath,
                };

                var response = await _client.DeleteObjectAsync(request, cancellationToken);

                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Unable to delete object: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> CopyObjectAsync(string srcBucket, string srcFilePath, string destBucket, string destFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(srcBucket)) throw new ArgumentNullException(nameof(srcBucket));
            if (string.IsNullOrEmpty(srcFilePath)) throw new ArgumentNullException(nameof(srcFilePath));
            if (string.IsNullOrEmpty(destBucket)) throw new ArgumentNullException(nameof(destBucket));
            if (string.IsNullOrEmpty(destFilePath)) throw new ArgumentNullException(nameof(destFilePath));

            try
            {
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
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error copying object {SourceObject} from {SourceBucket} to bucket {DestinationBucket} in object {DestinationObject}: '{Message}'", srcFilePath, srcBucket, destBucket, destFilePath, ex.Message);
                return false;
            }
        }

        public string GetPreSignedUrl(string bucket, string filePath, DateTime expires)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            try
            {
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
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error to get {FilePath} preSigned url: {Message}", filePath, ex.Message);
                throw;
            }
        }

        public async Task<bool> PutAclAsync(string bucket, string filePath, bool isPublicRead, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentNullException(nameof(bucket));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            try
            {
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
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error to change ACL on {FilePath}: {Message}", filePath, ex.Message);
                throw;
            }
        }

        #endregion
    }
}
