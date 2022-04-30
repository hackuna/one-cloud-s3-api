using Amazon.S3.Model;

namespace OneCloud.S3.API.Infrastructure.Interfaces
{
    /// <summary>
    /// Репозиторий для взаимодействия с контейнерами
    /// </summary>
    public interface IStorageBucketRepository
    {
        /// <summary>
        /// Получить список всех контейнеров
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<S3Bucket>> ListBucketsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получить содержимое контейнера
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<S3Object>> ListBucketContentAsync(string bucket, CancellationToken cancellationToken);

        /// <summary>
        /// Создать контейнер
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> PutBucketAsync(string bucket, CancellationToken cancellationToken);

        /// <summary>
        /// Удалить контейнер (должен быть предварительно очищен)
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteBucketAsync(string bucket, CancellationToken cancellationToken);
    }
}
