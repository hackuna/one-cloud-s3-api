using Microsoft.AspNetCore.Mvc;
using OneCloud.S3.API.Infrastructure.Interfaces;
using OneCloud.S3.API.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace OneCloud.S3.API.Controllers;

/// <summary>
/// Object controller
/// </summary>
[ApiController]
[Route("api/storage/objects")]
[Produces(MediaTypeNames.Application.Json)]
public class ObjectsController(IStorageObjectRepository storageRepository) : ControllerBase
{
    private readonly IStorageObjectRepository _storageRepository = storageRepository;

    /// <summary>
    /// Get object
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="contentType">Object MIME-type</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{bucket}/{filePath}", Name = "GetObject")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<IActionResult> GetObject(string bucket, string filePath, [Required] string contentType, CancellationToken cancellationToken)
    {
        var result = await _storageRepository.GetObjectAsync(bucket, filePath, cancellationToken);
        if(result.Length == 0)
            return NotFound();
        return File(result, contentType, Path.GetFileName(filePath));
    }

    /// <summary>
    /// Get temporary public link to object
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="expires">Date link expires</param>
    /// <returns></returns>
    [HttpGet("url/{bucket}/{filePath}", Name = "GetObjectUrl")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public IActionResult GetObjectUrl(string bucket, string filePath, [Required] DateTime expires)
    {
        var result = _storageRepository.GetPreSignedUrl(bucket, filePath, expires);
        return Ok(new ObjectUrlDto
        {
            Url = result,
            Expires = expires,
        });
    }

    /// <summary>
    /// Upload object
    /// </summary>
    /// <param name="bucket">Target bucket name</param>
    /// <param name="filePath">Target object key</param>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{bucket}/{filePath}", Name = "CreateObject")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<IActionResult> PostObject(string bucket, string filePath, [Required] IFormFile file, CancellationToken cancellationToken)
    {
        await _storageRepository.PutObjectAsync(bucket, filePath, file, cancellationToken);
        return CreatedAtAction("GetObject", new { bucket, filePath, file.ContentType });
    }

    /// <summary>
    /// Change object permissions
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="isPublicRead">Is object public?</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("permission/{bucket}/{filePath}", Name = "ChangeObjectPermissions")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> PutObjectPermission(string bucket, string filePath, bool isPublicRead, CancellationToken cancellationToken)
    {
        await _storageRepository.PutAclAsync(bucket, filePath, isPublicRead, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Copy object
    /// </summary>
    /// <param name="srcBucket">Source bucket name</param>
    /// <param name="srcFilePath">Source object key</param>
    /// <param name="destBucket">Target bucket name</param>
    /// <param name="destFilePath">Target object key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("copy/{srcBucket}/{srcFilePath}", Name = "CopyObject")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> PutObjectCopy(string srcBucket, string srcFilePath, [Required] string destBucket, [Required] string destFilePath, CancellationToken cancellationToken)
    {
        await _storageRepository.CopyObjectAsync(srcBucket, srcFilePath, destBucket, destFilePath, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Delete object
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="filePath">Object key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{bucket}/{filePath}", Name = "DeleteObject")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<IActionResult> DeleteObject(string bucket, string filePath, CancellationToken cancellationToken)
    {
        await _storageRepository.DeleteObjectAsync(bucket, filePath, cancellationToken);
        return Ok();
    }
}
