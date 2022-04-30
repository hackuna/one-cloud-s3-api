namespace OneCloud.S3.API.Infrastructure.Interfaces
{
    /// <summary>
    /// Репозиторий для взаимодействия с объектами
    /// </summary>
    public interface IStorageObjectRepository
    {
        /// <summary>
        /// Получить объект
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="filePath">Путь к объекту или наименование</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> GetObjectAsync(string bucket, string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Получить объект и записать в локальный файл
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="filePath">Наименование или путь к объекту в контейнере</param>
        /// <param name="localPath">Путь к локальному файлу</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> GetObjectToFileAsync(string bucket, string filePath, string localPath, CancellationToken cancellationToken);

        /// <summary>
        /// Опубликовать объект
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="filePath">Путь к объекту или наименование</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> PutObjectAsync(string bucket, string filePath, IFormFile file, CancellationToken cancellationToken);

        /// <summary>
        /// Удалить объект
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="filePath">Путь к объекту или наименование</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteObjectAsync(string bucket, string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Копировать объект
        /// </summary>
        /// <param name="srcBucket">Наименование исходного контейнера</param>
        /// <param name="srcFilePath">Наименование или путь копируемого объекта</param>
        /// <param name="destBucket">Наименование контейнера назначения</param>
        /// <param name="destFilePath">Путь назначения</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CopyObjectAsync(string srcBucket, string srcFilePath, string destBucket, string destFilePath, CancellationToken cancellationToken);

        /// <summary>
        /// Получить ссылку на объект
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="filePath">Путь к объекту или наименование</param>
        /// <param name="expires">Дата истечения срока действия ссылки</param>
        /// <returns></returns>
        string GetPreSignedUrl(string bucket, string filePath, DateTime expires);

        /// <summary>
        /// Изменить права доступа к объекту
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="filePath">Путь к объекту или наименование</param>
        /// <param name="isPublicRead">Признак доступности</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> PutAclAsync(string bucket, string filePath, bool isPublicRead, CancellationToken cancellationToken);
    }
}
