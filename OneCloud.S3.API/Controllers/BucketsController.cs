using Microsoft.AspNetCore.Mvc;
using OneCloud.S3.API.Infrastructure.Interfaces;
using OneCloud.S3.API.Models.Dto;
using System.Net.Mime;

namespace OneCloud.S3.API.Controllers;

/// <summary>
/// Buckets controller
/// </summary>
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Route("api/storage/buckets")]
[Produces(MediaTypeNames.Application.Json)]
public class BucketsController(IStorageBucketRepository storageRepository) : ControllerBase
{
    private readonly IStorageBucketRepository _storageRepository = storageRepository;

    /// <summary>
    /// Get list of buckets
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet(Name = "GetBuckets")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<IActionResult> GetBuckets(CancellationToken cancellationToken)
    {
        var buckets = await _storageRepository.ListBucketsAsync(cancellationToken);
        if(!buckets.Any())
            return NotFound();
        var result = buckets.Select(s => new BucketDto
        {
            BucketName = s.BucketName,
            CreationDate = s.CreationDate,
        });
        return Ok(result);
    }

    /// <summary>
    /// List of bucket objects
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("content/{bucket}", Name = "GetBucketContent")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<IActionResult> GetBucketContent(string bucket, CancellationToken cancellationToken)
    {
        var result = await _storageRepository.ListBucketContentAsync(bucket, cancellationToken);
        return Ok(result.Select(s => new ObjectDto
        {
            BucketName = s.BucketName,
            Key = s.Key,
            ETag = s.ETag,
            Size = s.Size,
            LastModified = s.LastModified,
        }));
    }

    /// <summary>
    /// Create bucket
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{bucket}", Name = "CreateBucket")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<IActionResult> PostBucket(string bucket, CancellationToken cancellationToken)
    {
        await _storageRepository.PutBucketAsync(bucket, cancellationToken);
        return CreatedAtAction("GetBucketContent", new { bucket });
    }

    /// <summary>
    /// Delete bucket
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{bucket}", Name = "DeleteBucket")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<IActionResult> DeleteBucket(string bucket, CancellationToken cancellationToken)
    {
        await _storageRepository.DeleteBucketAsync(bucket, cancellationToken);
        return Ok();
    }
}
