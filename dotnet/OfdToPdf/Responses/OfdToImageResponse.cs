namespace OfdToPdf.Responses;

public class OfdToImageResponse
{
    public int Code { get; set; }
    public string[] ImageUrls { get; set; }
    public int PageCount { get; set; }
}