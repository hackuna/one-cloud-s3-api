using Microsoft.AspNetCore.Mvc;
using OneCloud.S3.API.Infrastructure.Interfaces;
using OneCloud.S3.API.Models.Dto;
using System.Net.Mime;

namespace OneCloud.S3.API.Controllers
{
    /// <summary>
    /// Взаимодействие с контейнером
    /// </summary>
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/storage/bucket")]
    [Produces(MediaTypeNames.Application.Json)]
    public class BucketController : ControllerBase
    {
        private readonly ILogger<BucketController> _logger;
        private readonly IStorageBucketRepository _storageRepository;

        public BucketController(ILogger<BucketController> logger, IStorageBucketRepository storageRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
        }

        /// <summary>
        /// Список всех контейнеров
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Вернет список контейнеров</response>
        /// <response code="400">Неправильные параметры запроса</response>
        /// <response code="500">Что-то пошло не так</response>
        [HttpGet(Name = "GetBuckets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBuckets(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Получаем список контейнеров");

                var buckets = await _storageRepository.ListBucketsAsync(cancellationToken);

                if (buckets is null || !buckets.Any())
                {
                    _logger.LogWarning("Контейнеры отсутствуют");
                    return NotFound();
                }

                var result = buckets.Select(s => new BucketDto
                {
                    BucketName = s.BucketName,
                    CreationDate = s.CreationDate,
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ModelState.AddModelError(string.Empty, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        /// <summary>
        /// Содержимое контейнера
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Вернет содержимое контейнера</response>
        /// <response code="400">Неправильные параметры запроса</response>
        /// <response code="500">Что-то пошло не так</response>
        [HttpGet("content/{bucket}", Name = "GetBucketContent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBucketContent(string bucket, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Get bucket {Bucket} content", bucket);

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
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ModelState.AddModelError(string.Empty, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Создать контейнер
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="201">Контейнер создан</response>
        /// <response code="400">Неправильные параметры запроса</response>
        /// <response code="500">Что-то пошло не так</response>
        [HttpPost("{bucket}", Name = "CreateBucket")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostBucket(string bucket, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Create bucket {Bucket}", bucket);

                await _storageRepository.PutBucketAsync(bucket, cancellationToken);

                return CreatedAtAction("GetBucketContent", new { bucket });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ModelState.AddModelError(string.Empty, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Удалить контейнер
        /// </summary>
        /// <param name="bucket">Наименование контейнера</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Контейнер удален</response>
        /// <response code="400">Неправильные параметры запроса</response>
        /// <response code="500">Что-то пошло не так</response>
        [HttpDelete("{bucket}", Name = "DeleteBucket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBucket(string bucket, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Delete bucket {Bucket}", bucket);

                await _storageRepository.DeleteBucketAsync(bucket, cancellationToken);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ModelState.AddModelError(string.Empty, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
