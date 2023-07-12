using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using OneCloud.S3.API.Models;
using System.Net;

namespace OneCloud.S3.API.EndPoints;

public static class BucketsEndPoints
{
    public static IEndpointRouteBuilder UseBucketsEndpoints(this IEndpointRouteBuilder builder)
    {
        var buckets = builder
            .MapGroup("storage/buckets")
            .WithTags("Buckets");

        buckets.MapGet("", async (AmazonS3Client client, CancellationToken cancellationToken) =>
                await client.ListBucketsAsync(cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK } response
                ? TypedResults.Ok(response.Buckets.Select(s => new BucketDto(s.BucketName, s.CreationDate)))
                : Results.NotFound())
            .Produces<IEnumerable<BucketDto>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetBuckets")
            .WithSummary("Buckets list");

        buckets.MapGet("{bucketName}", async (AmazonS3Client client, string bucketName, CancellationToken cancellationToken) =>
                await client.ListObjectsAsync(new ListObjectsRequest { BucketName = bucketName }, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK } response
                ? TypedResults.Ok(response.S3Objects.Select(s => new ObjectDto(s.BucketName, s.Key, s.ETag, s.LastModified, s.Size)))
                : Results.NotFound())
            .Produces<IEnumerable<ObjectDto>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetBucketContent")
            .WithSummary("Bucket content");

        buckets.MapPost("{bucketName}", async (AmazonS3Client client, string bucketName, CancellationToken cancellationToken) =>
                await client.PutBucketAsync(bucketName, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK }
                    ? TypedResults.CreatedAtRoute("GetBucketContent", new { bucketName })
                : Results.BadRequest())
            .Produces<CreatedAtRoute>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .WithName("CreateBucket")
            .WithSummary("Create bucket");

        buckets.MapDelete("{bucketName}", async (AmazonS3Client client, string bucketName, CancellationToken cancellationToken) =>
                await client.DeleteBucketAsync(bucketName, cancellationToken)
                is { HttpStatusCode: HttpStatusCode.OK }
                    ? TypedResults.NoContent()
                : Results.NotFound())
            .Produces<NoContent>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteBucket")
            .WithSummary("Delete bucket");

        return builder;
    }
}
