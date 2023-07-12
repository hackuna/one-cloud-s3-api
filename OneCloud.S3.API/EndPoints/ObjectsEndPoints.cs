using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;
using OneCloud.S3.API.Models;
using System.Net;

namespace OneCloud.S3.API.EndPoints;

public static class ObjectsEndPoints
{
    public static IEndpointRouteBuilder UseObjectsEndpoints(this IEndpointRouteBuilder builder)
    {
        var objects = builder
            .MapGroup("storage/objects")
            .WithTags("Objects");

        objects.MapGet("{bucketName}", async (AmazonS3Client client, string bucketName, string filePath, CancellationToken cancellationToken) =>
            await client.GetObjectAsync(new GetObjectRequest { BucketName = bucketName, Key = filePath }, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK } response
                ? TypedResults.Stream(response.ResponseStream, response.Headers.ContentType, response.Key, response.LastModified, EntityTagHeaderValue.Parse(response.ETag), true)
                : Results.NotFound())
            .Produces<FileStreamHttpResult>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetObject")
            .WithSummary("Get object");

        objects.MapGet("{bucketName}/url", (AmazonS3Client client, string bucketName, string filePath, DateTime expires) =>
                client.GetPreSignedURL(new GetPreSignedUrlRequest { BucketName = bucketName, Key = filePath, Expires = expires, Protocol = Protocol.HTTPS })
                is { } url
                ? TypedResults.Created(url, new ObjectUrlDto(url, expires))
                : Results.NotFound())
            .Produces<ObjectUrlDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetObjectUrl")
            .WithSummary("Temporary public URL to object");

        objects.MapPost("{bucketName}", async (AmazonS3Client client, string bucketName, string filePath, IFormFile file, CancellationToken cancellationToken) =>
        {
            await using var stream = file.OpenReadStream();
            return await client.PutObjectAsync(new PutObjectRequest { BucketName = bucketName, Key = filePath, ContentType = file.ContentType, InputStream = stream, AutoCloseStream = true, UseChunkEncoding = false }, cancellationToken)
            is { HttpStatusCode: HttpStatusCode.OK }
                ? TypedResults.CreatedAtRoute("GetObject", new { bucketName, filePath, file.ContentType })
            : Results.BadRequest();
        })
            .Produces<PutObjectResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .WithName("CreateObject")
            .WithSummary("Create object");

        objects.MapPut("{bucketName}/permission", async (AmazonS3Client client, string bucketName, string filePath, bool isPublicRead, CancellationToken cancellationToken) =>
                await client.PutACLAsync(new PutACLRequest { BucketName = bucketName, Key = filePath, CannedACL = isPublicRead ? S3CannedACL.PublicRead : S3CannedACL.Private }, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK }
                    ? TypedResults.NoContent()
                : Results.NotFound())
            .Produces<NoContent>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("ChangeObjectPermissions")
            .WithSummary("Change object permissions");

        objects.MapPut("{bucketName}/copy", async (AmazonS3Client client, string bucketName, string filePath, string destBucket, string destFilePath, CancellationToken cancellationToken) =>
                await client.CopyObjectAsync(new CopyObjectRequest { SourceBucket = bucketName, SourceKey = filePath, DestinationBucket = destBucket, DestinationKey = destFilePath }, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK }
                    ? TypedResults.NoContent()
                : Results.NotFound())
            .Produces<NoContent>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("CopyObject")
            .WithSummary("Copy object");

        objects.MapDelete("{bucketName}", async (AmazonS3Client client, string bucketName, string filePath, CancellationToken cancellationToken) =>
                await client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = bucketName, Key = filePath }, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK }
                    ? TypedResults.NoContent()
                : Results.NotFound())
            .Produces<NoContent>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteObject")
            .WithSummary("Delete object");

        return builder;
    }
}
