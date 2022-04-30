namespace OneCloud.S3.API.Infrastructure.Interfaces
{
    /// <summary>
    /// Репозиторий для взаимодействия с хранилищем
    /// </summary>
    public interface IStorageRepository : IStorageBucketRepository, IStorageObjectRepository
    {
    }
}
