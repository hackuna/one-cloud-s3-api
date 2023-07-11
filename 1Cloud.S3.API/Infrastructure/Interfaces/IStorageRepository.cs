namespace OneCloud.S3.API.Infrastructure.Interfaces;

/// <summary>
/// Generalized storage repository
/// </summary>
public interface IStorageRepository : IStorageBucketRepository, IStorageObjectRepository;