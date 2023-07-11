namespace OneCloud.S3.API.Models.Dto;

public class ObjectUrlDto
{
    public string Url { get; set; } = null!;
    public DateTime Expires { get; set; }
}
