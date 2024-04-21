namespace Api.Dto;

public class BaseDto
{
    public ErrorInfo? ErrorInfo { get; set; }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorInfo?.Message);
}